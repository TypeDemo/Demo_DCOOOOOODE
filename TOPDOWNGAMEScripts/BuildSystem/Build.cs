using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{

    public static Dictionary<Transform, GameObject> buildBox = new Dictionary<Transform, GameObject>();
    public GameObject buildBasePrefab;

    public static  Transform[,] buildTransforms = new Transform[3, 6];

    private GameObject buildBase;

    private void Awake()
    {
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 6; i++)
            {
                var go = Resources.Load<GameObject>("Prefabs/BuildBase");
                buildBase = Instantiate(buildBasePrefab, transform);
                buildBase.transform.localPosition = new Vector3(1.5f + 3 * i, 0.02f, 1.5f + 3 * j);
                buildTransforms[j, i] = buildBase.transform;
            }
        }
    }
   
    
}
