using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClickStart()
    {
        ChangeLevel();
    }

    private  void  ChangeLevel()
    {
         SceneManager.LoadScene("Loading", LoadSceneMode.Single);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
