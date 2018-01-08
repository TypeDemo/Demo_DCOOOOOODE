using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmControl : MonoBehaviour {

    public  AudioClip[] audioClips;
    private  AudioSource myAudio;

    private static BgmControl bgmInstance;
    public static BgmControl bgm;
	// Use this for initialization
	void Start () {
        bgm = this;
        myAudio = GetComponent<AudioSource>();
        myAudio.clip = audioClips[0];
        myAudio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public  void ChangeBGM()
    {
        myAudio.clip = audioClips[1];
        myAudio.Play();
    }
}
