using System.Collections;
using UnityEngine;

[System.Serializable]
public class CrossFade : SceneTransition
{
    public CanvasGroup crossFade;
    public float transitionDuration = 1f;

    public override IEnumerator AnimateTransitionIn()
    {
        // Fade in (from transparent to opaque)
        float elapsedTime = 0;
        float startAlpha = crossFade.alpha;

        while (elapsedTime < transitionDuration)
        {
            crossFade.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we end at exactly 1
        crossFade.alpha = 1f;
    }

    public override IEnumerator AnimateTransitionOut()
    {
        // Fade out (from opaque to transparent)
        float elapsedTime = 0;
        float startAlpha = crossFade.alpha;

        while (elapsedTime < transitionDuration)
        {
            crossFade.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we end at exactly 0
        crossFade.alpha = 0f;
    }
}