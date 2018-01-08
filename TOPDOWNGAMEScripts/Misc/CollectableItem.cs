using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour {

    public  int id;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GetComponent<MeshRenderer>().enabled = true;

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<MeshRenderer>().enabled = false;

        }
    }
}
