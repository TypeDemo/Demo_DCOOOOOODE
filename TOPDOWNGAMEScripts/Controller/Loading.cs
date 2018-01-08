using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour {

    public Slider progressSlider;
    public Text percentText;

    // Use this for initialization
    void Start() {
        StartCoroutine(LoadingScene());
    }

    private IEnumerator LoadingScene()
    {
        int disPlayProress = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync("Level");
        op.allowSceneActivation = false;

        while (op.progress<0.9f)
        {
            disPlayProress = (int)(op.progress * 100);
            SetPercent(disPlayProress);
            yield return new WaitForEndOfFrame();
        }
        while(disPlayProress<100)
        {
            disPlayProress++;
            SetPercent(disPlayProress);
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;

    }
	
    private void SetPercent(int displayProgerss)
    {
        progressSlider.value = displayProgerss * 0.01f;
        percentText.text = displayProgerss.ToString() + "%";
    }
}
