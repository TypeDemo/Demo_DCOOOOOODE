using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRotate : MonoBehaviour
{
    public float rotatespeed = 10.0f;
    [HideInInspector]
    public bool findTarget = false;
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!findTarget)
        {
            transform.Rotate(Vector3.up * rotatespeed * Time.deltaTime);

        }
        
    }
}
