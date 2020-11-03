using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.SceneManagement
{
    [RequireComponent(typeof(Image))]
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActiveFade = null;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        /* IEnumerator FadeOutIn()
         {
             yield return FadeOut();

             yield return FadeIn();
         }*/


        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0f, time);
        }
        public Coroutine FadeOut(float time)
        {
            return Fade(1f, time);
        }
        public Coroutine Fade(float target, float time)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFade;
        }
        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

    }
} 