using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStickMoveMent : ScrollRect {


    //半径
    private float maxRadius = 0.0f;
    [HideInInspector]
    public Vector3 move=Vector3.zero;

    Vector2 v2;

    //单例
    public static JoyStickMoveMent instance;
    
    //距离
    private const float distance = 1.0f;

    protected override void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        maxRadius = content.sizeDelta.x * distance;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        //获取摇杆，根据锚点位置
        var contentPosition = content.anchoredPosition;

        //判断摇杆位置是否大于半径
        if(contentPosition.magnitude> maxRadius)
        {
            //设置摇杆最远的位置
            contentPosition = contentPosition.normalized* maxRadius;
            SetContentAnchoredPosition(contentPosition);
        }

        // 最后 v2.x/y 就跟 Input中的 Horizontal Vertical 获取的值一样 
        Vector2 v2 = content.anchoredPosition.normalized;
        move = v2.y * Vector3.forward + v2.x * Vector3.right;
        
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        move = Vector3.zero;
        content.anchoredPosition = Vector2.zero;
    }


}
