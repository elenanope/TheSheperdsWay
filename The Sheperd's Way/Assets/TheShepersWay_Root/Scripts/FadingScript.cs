using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingScript : MonoBehaviour
{
    [SerializeField] Image fadePanel;
    public float fadeInSpeed = 3f;

    private void Start()
    {
        StartCoroutine(StartFade());
    }

    IEnumerator StartFade()
    {
        Color actualColor = fadePanel.color;

        yield return new WaitForSecondsRealtime(1.5f);
        while (actualColor.a > 0f)
        {
            actualColor.a -= fadeInSpeed * Time.deltaTime;
            actualColor.a = Mathf.Clamp01(actualColor.a);

            fadePanel.color = actualColor;

            yield return null;
        }
        gameObject.SetActive(false);
    }
}