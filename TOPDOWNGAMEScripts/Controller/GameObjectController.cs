using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameObjectController : MonoBehaviour
{

    public GameObject closeCanvas;
    public GameObject openCanvas;

    public enum ControlState
    {
        Player,
        Builder,
    }

   //单例
    private static GameObjectController instance;
    private GameObjectController()
    {

    }
    public static GameObjectController gameControl
    {
        get
        {
            if(instance==null)
            {
                instance = GameObject.Find("ControllerManager").GetComponent<GameObjectController>();
            }

            return instance;
        }
    }

    //private void Awake()
    //{
    //    instance = this;
    //}

    private void Start()
    {
        Time.timeScale = 1;
    }



    public ControlState controlState = ControlState.Player;

    public void ControlPlayer()
    {
        controlState = ControlState.Player;
    }

    public void ShowEndUI()
    {
        
        closeCanvas.SetActive(false);
        openCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    public void ReStartScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

    }

    public void EndGame()
    {
        Application.Quit();
    }
}
