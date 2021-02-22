using RPG.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Jobs;
using UnityEngine;

public class QuestTooltipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Transform objectiveContainer;
    [SerializeField] GameObject objectivePrefab;
    [SerializeField] GameObject objectiveIncompletePrefab;
    [SerializeField] TextMeshProUGUI rewardText;

    public void Setup(QuestStatus status)
    {
        Quest quest = status.GetQuest();
        title.text = quest.GetTitle();
        foreach(var obj in quest.GetObjectives())
        {

            GameObject objectivePrefab = status.IsObjectiveComplete(obj.reference) ? this.objectivePrefab : objectiveIncompletePrefab;
            GameObject currentObjectivePrefab = Instantiate(objectivePrefab, objectiveContainer);
            currentObjectivePrefab.GetComponentInChildren<TextMeshProUGUI>().text = obj.description;
            

        }
        rewardText.text = GetRewardText(quest);


    }

    private string GetRewardText(Quest quest)
    {
        string rewardText = "";
        foreach(var reward in quest.GetRewards())
        {
            if(rewardText != "")
            {
                rewardText += ", ";
            }
            if (reward.number > 1)
            {
                rewardText += reward.number + " ";
            }
            rewardText += reward.item.GetDisplayName();
            
        }
        if(rewardText == "")
        {
            rewardText = "No reward";
        }
        rewardText += ".";
        return rewardText;
    }
}
