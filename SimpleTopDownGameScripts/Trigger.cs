using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour {
    public UnityEvent onEnter;
    public UnityEvent onExit;

    public string tagName = "Player";

    public Color color;
    public Renderer[] renders;
    string colorName = "_EmissionColor";

    public AudioSource[] sounds;

    public Animator[] animates;
    public string animateEnterName = "TriggerEnter";
    public string animateExitName = "TriggerExit";


    public bool isTriggered = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered == true) { return; }
        isTriggered = true;
        if(!other.CompareTag(tagName))
        { return;}

        
        foreach(var render in renders)                  //设置颜色
        {
            render.material.SetColor(colorName, color);
        }

        foreach(var sound in sounds)                //设置音乐
        {
            sound.Play();
        }

        foreach(var animate in animates)            //判断是否播放动画
        {
            animate.SetBool(animateEnterName, true);
            animate.SetBool(animateExitName, false);

        }

        onEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagName))
        { return; }

       
        foreach(Renderer render in renders)
        {
            render.material.SetColor(colorName, Color.black);
        }
        
        foreach(var sound in sounds)
        {
            sound.Stop();
        }

        foreach(var animate in animates)
        {
            animate.SetBool(animateEnterName, false);
            animate.SetBool(animateExitName, true);
        }

        onExit.Invoke();

    }
}
