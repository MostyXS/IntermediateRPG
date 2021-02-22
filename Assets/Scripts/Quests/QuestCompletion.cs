using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest quest;
        [SerializeField] private string objective;

        private void Start()
        {
        }
        public void CompleteObjective()
        {
            QuestList questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
            questList.CompleteObjective(quest, objective);
        }

    }
}