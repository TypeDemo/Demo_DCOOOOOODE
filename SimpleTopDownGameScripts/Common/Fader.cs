using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    CanvasGroup canvas;

    public bool isFading =true;

    float fadeDuration = 1.0f;

    // Use this for initialization
    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 1.0f;
        StartCoroutine(Fade(1.0f));
    }

    // Update is called once per frame
    void Update()
    {

    }



    public IEnumerator Fade(float finalAlpha)
    {
        isFading = true;
        canvas.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(canvas.alpha - finalAlpha) / fadeDuration;      //控制渐变速度
        while (!Mathf.Approximately(canvas.alpha, finalAlpha))                      //不断将值改变到finalAlpha
        {
            canvas.alpha = Mathf.MoveTowards(canvas.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        isFading = false;
        canvas.blocksRaycasts = false;

    }
}
