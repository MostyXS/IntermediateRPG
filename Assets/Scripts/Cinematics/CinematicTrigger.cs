using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool triggered;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<PlayerController>() || triggered) return;
                TriggerCinematic();
        }

        private void TriggerCinematic()
        {
            GetComponent<PlayableDirector>().Play();
            triggered = true;   
        }
    }
}