using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        List<QuestStatus> statuses = new List<QuestStatus>();
        public event Action onUpdate;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            statuses.Add(new QuestStatus(quest));
            if (onUpdate != null) onUpdate();
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            if (status != null)
            {
                
                status.CompleteObjective(objective);
                if (status.IsComplete())
                {
                    GiveReward(quest);
                }
                if (onUpdate != null) onUpdate();
            }
        }

        private void GiveReward(Quest quest)
        {
            foreach(var reward in quest.GetRewards())
            {
                GameObject player = GameObject.FindWithTag("Player");
                
                bool success = player.GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);
                if(!success)
                {
                    player.GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                }
                
            }

        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            return statuses.Where((x) => x.GetQuest() == quest).FirstOrDefault();
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach(QuestStatus status in statuses)
            {
                state.Add(status.CaptureState());
            }
            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if (stateList == null) return;
            statuses.Clear();
            foreach(object objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));
            }
            
        }

        public bool? Evaluate(string predicate, string[] parametrs)
        {
            switch(predicate)
            {
                case "HasQuest":
                    {
                        return HasQuest(Quest.GetByName(parametrs[0]));
                    }
                case "CompletedQuest":
                    {
                        return GetQuestStatus(Quest.GetByName(parametrs[0])).IsComplete();
                    }
                default: return null;

            }
        }
        
    }
}
