using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActiveFade = null;

        public void FadeOutInmediate()
        {
            canvasGroup.alpha = 1;
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }     

        public Coroutine Fade(float alphaTarget, float time)
        {
            if(currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }

            currentActiveFade = StartCoroutine(FadeRoutine(alphaTarget, time));
            return currentActiveFade;
        }

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        IEnumerator FadeRoutine(float alphaTarget, float time)
        {
            while(!Mathf.Approximately(canvasGroup.alpha, alphaTarget))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alphaTarget, Time.unscaledDeltaTime / time);
                yield return null;
            }
        }
    }
}

