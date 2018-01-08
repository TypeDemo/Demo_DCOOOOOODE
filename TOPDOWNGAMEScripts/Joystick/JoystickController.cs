using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : MonoBehaviour {

    private Vector3 origin;
    private Transform mTrans;

    private Vector3 deltaPos;
    private bool drag = false;

    private Vector3 deltaPosition;

    private float distance;
    [SerializeField]
    private float moveMaxDistance = 80;//最大拖动距离

    [HideInInspector]
    public Vector3 fixedMovePositon;//固定8个角度移动的距离

    [HideInInspector]
    public Vector3 movePositionNorm;//标准化移动的距离

    [SerializeField]
    private float activeMoveDistance = 1;   //激活移动的最低距离

    private void Awake()
    {
        EventTriggerListener.Get(gameObject).onDrag = OnDrag;
        EventTriggerListener.Get(gameObject).onDragOut = OnDragOut;
        EventTriggerListener.Get(gameObject).onDown = OnMoveStart;
    }

    private void Start()
    {
        origin = transform.localPosition;//设置原点
        mTrans = transform;
    }

    private void Update()
    {
        //拖动距离，这不是最大的拖动距离，是根据初末位置算出来的
        distance = Vector3.Distance(transform.localPosition, origin);
        if(distance>=moveMaxDistance)
        {
            //求圆上的一点：（目标点-原点）*半径/原点到目标点的距离
            Vector3 vec3 = origin + (transform.localPosition - origin) * moveMaxDistance / distance;
            transform.localPosition = vec3;
        }
        if (Vector3.Distance(transform.localPosition, origin) > activeMoveDistance)//距离大于激活移动的距离
        {
            movePositionNorm = (transform.localPosition - origin).normalized;
            movePositionNorm = new Vector3(movePositionNorm.x, 0, movePositionNorm.y);

        }
        else
        {
            movePositionNorm = Vector3.zero;
        }
       
    }

    void MouseDown()
    {
        if ((Input.touchCount) > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {

        }
        else
        {
            mTrans.localPosition = origin;
        }
    }

    Vector3 result;

    private Vector3 checkPosition(Vector3 movePos,Vector3 offsetPos)
    {
        result = movePos += offsetPos;
        return result;
    }

    private void OnDrag(GameObject go,Vector2 delta)
    {
        if(!drag)
        {
            drag = true;
        }
        deltaPos = delta;
        mTrans.localPosition += new Vector3(deltaPos.x, deltaPos.y, 0);
    }

    private void OnDragOut(GameObject go)
    {
        drag = false;
        mTrans.localPosition = origin;
        if(JoystickMove.moveEnd!=null)
        {
            JoystickMove.moveEnd();
        }
    }

    private void OnMoveStart(GameObject go)
    {
        if(JoystickMove.moveStart!=null)
        {
            JoystickMove.moveStart();
        }
    }
}
