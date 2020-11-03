using RPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health health;
        Text healthDisplay;

        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
            healthDisplay = GetComponent<Text>();
        }

        private void Update()
        {
            if (!fighter.GetTarget())
            {
                healthDisplay.text = " N/A";
                return;
            }
            health = fighter.GetTarget();
            healthDisplay.text = string.Format("{0:0}/{1:0}",health.GetHealthPoints(),health.GetMaxHealthPoints());
        }
    }
}
