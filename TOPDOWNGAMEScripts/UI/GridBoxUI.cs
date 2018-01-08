using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoxUI : MonoBehaviour
{

    public Transform[] gridList;

    public Transform GetEmptyGrid()
    {
        for (int i = 0; i < gridList.Length; i++)
        {
            if (gridList[i].childCount == 0)
                return gridList[i];
        }
        return null;
    }

}
