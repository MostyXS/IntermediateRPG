using GameDevTV.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu (fileName = "Quest" , menuName = "RPG Project/Quest",order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>();


        [Serializable]
        public class Reward
        {
            [Min(1)]
            public int number;
            public InventoryItem item;
        }

        [Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }

        public string GetTitle()
        {
            return name;
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public bool HasObjective(string objectiveReference)
        {
            return objectives.Any((x) => x.reference == objectiveReference);
        }

        public static Quest GetByName(string questName)
        {
            return Resources.LoadAll<Quest>("").Where((quest) => quest.name == questName).FirstOrDefault();
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }
        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }
    }

}