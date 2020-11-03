using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        Text healthDisplay;

        private void Awake()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            healthDisplay = GetComponent<Text>();
        }

        private void Update()
        {
            healthDisplay.text = string.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
