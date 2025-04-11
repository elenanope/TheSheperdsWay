using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadingScript : MonoBehaviour
{
    [SerializeField] Image fadePanel;
    [SerializeField] GameObject panel;
    [SerializeField] float fadeInSpeed = 3f;
    [SerializeField] float fadeOutSpeed = 3f;
     

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color actualColor = fadePanel.color;
        actualColor.a = 1f;

        yield return new WaitForSecondsRealtime(1.5f);
        while (actualColor.a > 0f) //si fuera FadeOut sería <1f
        {
            actualColor.a -= fadeInSpeed * Time.deltaTime; // es "menos" para FadeIn pq el panel tiene el alpha a tope 
            actualColor.a = Mathf.Clamp01(actualColor.a);

            fadePanel.color = actualColor;

            yield return null;
        }
        panel.gameObject.SetActive(false);
    }
     IEnumerator FadeOut (int sceneToLoad, float waitTime = 0.25f)
    { 
        Color actualColor = fadePanel.color;
        actualColor.a = 0f;
        yield return new WaitForSecondsRealtime(waitTime);
        panel.gameObject.SetActive(true);
        while (actualColor.a < 1f)
        {
            actualColor.a += fadeOutSpeed * Time.deltaTime;
            actualColor.a = Mathf.Clamp01(actualColor.a);

            fadePanel.color = actualColor;
            yield return null;
        }
        SceneManager.LoadScene(sceneToLoad);
    }
    public void FadingOut(int sceneToLoad)
    {
        StartCoroutine(FadeOut(sceneToLoad));
    }
}