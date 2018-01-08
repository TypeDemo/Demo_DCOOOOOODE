using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public float timer = 90.0f;
    private float nextTime=0.0f;
    private Text text;
    private string timeinfo;
    [HideInInspector]
    public bool timeOut=false;
    
	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if(timer<=0&&!timeOut)
        {
            timeOut = true;
            BgmControl.bgm.ChangeBGM();
        }
        if (!timeOut)
        {
            if (nextTime < Time.time)
            {
                timer--;
                timeinfo = string.Format("{0:d2}:{1:d2}", (int)(timer / 60.0f),(int)(timer % 60.0f));
                text.text = timeinfo;
                nextTime = Time.time + 1;
            }
        }
		        
	}
}
