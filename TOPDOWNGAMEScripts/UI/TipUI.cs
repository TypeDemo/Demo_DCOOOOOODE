using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipUI : MonoBehaviour {

    public Text contentText;

    public void UpdateText(string description)
    {
        GetComponent<Text>().text = description;
        contentText.text = description;
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
