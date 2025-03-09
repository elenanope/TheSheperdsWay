using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingScript : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    public float fadeOutDuration = 3f;
    public float fadeInDuration = 3f;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(nameof(StartGameFadeOut));
    }
    public void FadeOut()
    {
        StartCoroutine(FadeOutCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeOutDuration));
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeInDuration));
    }

    private IEnumerator FadeOutCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }
    private IEnumerator FadeInCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    private IEnumerator StartGameFadeOut()
    {
        yield return new WaitForSeconds(1);
        FadeOut();
        yield return null;
    }
}