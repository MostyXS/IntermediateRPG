using GameDevTV.Saving;
using System;
using System.Collections.Generic;

namespace RPG.Quests
{
    public class QuestStatus
    {
        Quest quest;
        List<string> completedObjectives = new List<string>();

        

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            if (state == null) return;
            quest = Quest.GetByName(state.questName);
            completedObjectives = state.completedObjectives;
        }

        public Quest GetQuest()
        {
            return quest;
        }
        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsComplete()
        {
            return completedObjectives.Count == quest.GetObjectiveCount();
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            if (quest.HasObjective(objective))
            {
                completedObjectives.Add(objective);
            }
        }

        public object CaptureState()
        {
            return new QuestStatusRecord(quest.GetTitle(), completedObjectives);
        }
        [System.Serializable]
        class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;

            public QuestStatusRecord(string questName, List<string> completedObjectives)
            {
                this.questName = questName;
                this.completedObjectives = completedObjectives;
            }

        }
    }
}