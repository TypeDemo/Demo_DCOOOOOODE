using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour {

    public GameObject prefab;
    private Button button;
    private GameObject buttonCover;

	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();
        buttonCover = GameObject.Find("ControlButton").transform.Find("ButtonCover").gameObject;

        button.onClick.AddListener(GetBuilding);

    }
		

    private void GetBuilding()
    {
        var go =GameObjectPool.inventoryPool.GetObject(prefab);
        go.SetActive(true);
        if(!KnapSackManager.instance.itemNum.ContainsKey(10001))
        {
            buttonCover.SetActive(true);
        }
    }

}
