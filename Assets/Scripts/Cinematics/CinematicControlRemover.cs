using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using UnityEngine.XR;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;
        PlayableDirector pd;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            pd = GetComponent<PlayableDirector>();
        }
        private void OnEnable()
        {
            pd.played += DisableControl;
            pd.stopped += EnableControl;
        }
        private void OnDisable()
        {
            pd.played -= DisableControl;
            pd.stopped -= EnableControl;
        }
        void DisableControl(PlayableDirector director)
        {
           player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector director)
        {
            if(player)
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}