using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildItem : MonoBehaviour
{
    protected Button finishButton;
    protected Button cancelButton;
    protected GameObject buildButton;
    private GameObject buttonCover;

    [HideInInspector]
    public bool isBuilded = false;
    [HideInInspector]
    public bool onBuildGround;
    protected bool clear = true;

    protected GameObject buildSys;
    protected GameObjectController controller;
    private float timeBetween = 0.4f;
    private float moveNextTime;
    protected float groundCheckDistance = 1.0f;

    private Vector3 move;

    protected int vt;
    protected int hz;

    private Vector3 groundNormal;

    protected void Awake()
    {
        
        buildSys = GameObject.Find("Level/AllBuild").transform.Find("BuilidngSys").gameObject;
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameObjectController>();

        var go = GameObject.Find("Canvases/MiniMapCanvas/ControlButton");
        finishButton = go.transform.Find("FinishBuildButton").GetComponent<Button>();
        buildButton = GameObject.Find("Canvases/MiniMapCanvas/ControlButton").transform.Find("BuildButton").gameObject;
        cancelButton = GameObject.Find("Canvases/MiniMapCanvas/BuildList").transform.Find("BuildCancelButton").GetComponent<Button>();
        buttonCover = GameObject.Find("ControlButton").transform.Find("ButtonCover").gameObject;

    }
    protected void OnEnable()
    {
        vt = 0;
        hz = 0;
        transform.position = Build.buildTransforms[hz, vt].position;

        finishButton.onClick.AddListener(FinishBuild);
        cancelButton.onClick.AddListener(CancelBuild);
    }

    // Use this for initialization
    protected void Start()
    {
        vt = 0;
        hz = 0;
        transform.position = Build.buildTransforms[hz, vt].position;

    }

    protected void Update()
    {
        CheckGround();
        UpdateControl();
        UpdateMovement();

    }


    protected void UpdateControl()
    {
        if (controller.controlState == GameObjectController.ControlState.Builder && isBuilded == false)
        {

            MoveControl();

            //结束建造，转移控制对象，调整组件状态
            if (PlayerCharacter.player.controlMode == PlayerCharacter.ControlMode.KeyBoard)
            {
                if (Input.GetButtonDown("Build") && onBuildGround && clear)
                {
                    FinishBuild();
                }
            }

        }

    }

    protected void UpdateMovement()
    {
        if (Time.time < moveNextTime)
            return;
        moveNextTime = Time.time + timeBetween;

        //X轴移动
        if (move.x > 0.5)
        {
            if (vt + 1 < Build.buildTransforms.GetLength(1))
            {
                vt += 1;
                transform.position = Build.buildTransforms[hz, vt].position;

            }
        }
        else if (move.x < -0.5)
        {
            if (vt - 1 >= 0)
            {
                vt -= 1;
                transform.position = Build.buildTransforms[hz, vt].position;

            }
        }

        //Z轴移动
        if (move.z > 0.5)
        {
            if (hz + 1 < Build.buildTransforms.GetLength(0))
            {
                hz += 1;
                transform.position = Build.buildTransforms[hz, vt].position;
            }

        }
        else if (move.z < -0.5)
        {
            if (hz - 1 >= 0)
            {
                hz -= 1;
                transform.position = Build.buildTransforms[hz, vt].position;

            }
        }
    }

    protected void Movement(Vector3 move)
    {
        if (move.magnitude > 1) move.Normalize();
        //将move从世界空间转向本地空间
        move = transform.InverseTransformDirection(move);
        //将move投影在地板的2D平面上
        move = Vector3.ProjectOnPlane(move, groundNormal);


    }

    protected void CheckGround()
    {
        RaycastHit hitInfo;
        Debug.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + Vector3.down * groundCheckDistance);
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hitInfo, groundCheckDistance, LayerMask.GetMask("Build")))
        {
            onBuildGround = true;
            groundNormal = hitInfo.normal;
        }
        else
        {
            onBuildGround = false;
            groundNormal = Vector3.up;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Builder"))
        {
            clear = false;
        }
        else
        {
            clear = true;
        }
    }


    //结束建造
    protected virtual void FinishBuild()
    {
        if (Build.buildBox.ContainsKey(Build.buildTransforms[hz, vt]))
            return;
        if(!KnapSackManager.instance.CastItem(10001))
        {
            GameObjectPool.inventoryPool.ReturnObject(this.gameObject);
            return;
        }
        //转换控制对象，激活和关闭相关组件
        controller.controlState = GameObjectController.ControlState.Player;
        isBuilded = true;
        GetComponentInChildren<CapsuleCollider>().enabled = true;
        GetComponentInChildren<TowerRotate>().enabled = true;
        GetComponentInChildren<DefenceTower>().enabled = true;
        transform.Find("Field").gameObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        GetComponent<BuildItem>().enabled = false;
        buildSys.SetActive(false);

        Build.buildBox.Add(Build.buildTransforms[hz, vt], this.gameObject);

        //从按钮中移除自己的事件不再被调用
        finishButton.onClick.RemoveListener(FinishBuild);
        finishButton.gameObject.SetActive(false);
        buildButton.SetActive(true);
        cancelButton.onClick.RemoveListener(CancelBuild);
        cancelButton.gameObject.SetActive(false);
    }

    protected void MoveControl()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h < 0)
            h = -1;
        else if (h > 0)
            h = 1;
        if (v < 0)
            v = -1;
        else if (v > 0)
            v = 1;

        if (PlayerCharacter.player.controlMode == PlayerCharacter.ControlMode.KeyBoard)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            move = v * Vector3.forward + h * Vector3.right;
        }
        else if (PlayerCharacter.player.controlMode == PlayerCharacter.ControlMode.Joystick)
        {
            move = JoyStickMoveMent.instance.move;
        }
        Movement(move);
        ShowCover();
    }

    protected void CancelBuild()
    {
        cancelButton.gameObject.SetActive(false);
        cancelButton.onClick.RemoveListener(CancelBuild);
        finishButton.onClick.RemoveListener(FinishBuild);
        GameObjectPool.inventoryPool.ReturnObject(this.gameObject);
       
    }

    private void ShowCover()
    {
        
        if (Build.buildBox.ContainsKey(Build.buildTransforms[hz,vt]))
        {
            buttonCover.SetActive(true);
        }
        else if(!Build.buildBox.ContainsKey(Build.buildTransforms[hz, vt])&& KnapSackManager.instance.itemNum.ContainsKey(10001))
        {
            buttonCover.SetActive(false);
        }
    }

    
}