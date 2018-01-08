using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class GridUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler,IPointerClickHandler
{

    public static Action<Transform> beginDrag;
    public static Action<Transform, Transform> endDrag;
    public static Action<Transform> onClick;
    

    public int idInGrid;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (beginDrag != null)
                beginDrag(transform);

        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (endDrag != null)
                if (eventData.pointerEnter == null)
                {
                    endDrag(transform, null);
                }
                else
                {
                    endDrag(transform, eventData.pointerEnter.transform);

                }

        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick(transform);
    }
}
