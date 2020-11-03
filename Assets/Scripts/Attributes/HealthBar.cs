using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Canvas canvas;
        [SerializeField] Health healthComponent;
        [SerializeField] RectTransform foreground;

        void UpdateHealth()
        {

            Vector3 newScale = new Vector3(healthComponent.GetFracton(),
                foreground.localScale.y, foreground.localScale.z);
            foreground.localScale = newScale;
        }
        private void Update()
        {
            if(Mathf.Approximately(healthComponent.GetFracton(), 0) 
                || Mathf.Approximately(healthComponent.GetFracton(), 1))
            {
                canvas.enabled = false;
                return;
            }
            canvas.enabled = true;

            UpdateHealth();
        }

    
        
    }

}