using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    BaseStats stats;
    Text levelDisplay;
    private void Awake()
    {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        levelDisplay = GetComponent<Text>();
    }
    private void Update()
    {
        levelDisplay.text = stats.GetLevel().ToString();
    }
}
