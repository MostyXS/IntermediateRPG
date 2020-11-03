using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class XPDisplay : MonoBehaviour
    {
        Experience experience;
        Text experienceDisplay;
        private void Awake()
        {
            experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
            experienceDisplay = GetComponent<Text>();
        }
        private void Update()
        {
            experienceDisplay.text = experience.GetPoints().ToString();
        }
    }

}