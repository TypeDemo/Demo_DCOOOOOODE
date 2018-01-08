using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodLineSet : MonoBehaviour {

    private Slider slider;
	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
		if(slider.value<=0.01f)
        {
            this.gameObject.SetActive(false);
        }
	}
}
