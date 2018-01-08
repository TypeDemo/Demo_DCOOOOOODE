using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject enemy;
    public int enemyNum;
	// Use this for initialization
	void Start () {
        for(int i=0; i< enemyNum; i++)
        {
            SetEnemy();

        }

    }

    public static void EndGame()
    {
        Time.timeScale = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetEnemy()
    {
        GameObjectPool.inventoryPool.GetObject(enemy);
    }
}
