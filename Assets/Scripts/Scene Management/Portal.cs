using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using GameDevTV.Saving;
using UnityEngine.UIElements;
using RPG.Control;

namespace RPG.SceneManagement
{

    public class Portal : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float FadeOutTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;

        

        enum DestinationIdentifier
        {
            A,B,C,D,E
        }
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] DestinationIdentifier destination =DestinationIdentifier.A;


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                StartCoroutine(Transition());
        }
        IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set");
                yield break;

            }

            DontDestroyOnLoad(gameObject);
            
            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            
            playerController.enabled = false;
            yield return fader.FadeOut(FadeOutTime);

            savingWrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            
            PlayerController newPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            
            savingWrapper.Load();
            
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            savingWrapper.Save();
            
            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);
            newPlayerController.enabled = true;
            Destroy(gameObject);
        }


        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;

        }

        private Portal GetOtherPortal()
        {
           
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this || destination != portal.destination ) continue;
                    return  portal;
            }
            return null;
        }
    }
}