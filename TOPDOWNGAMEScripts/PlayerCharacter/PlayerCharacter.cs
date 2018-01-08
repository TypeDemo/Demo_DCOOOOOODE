
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerCharacter : Character
{
    public enum ControlMode
    {
        KeyBoard,
        Joystick,
    }

    public GameObject buildSys;
    public GameObject buildButton;
    public GameObject finishButton;

    public ControlMode controlMode = ControlMode.KeyBoard;

    public ShootWeapon shootWeapon;
    public GameObject buildPrefab;
    public GameObject buildings;
    public PlayerSensor sensor;
    private GameObjectController controller;
    private GameObject buildList;

    private KnapSackManager knapSack;

    public Slider bloodLine;

    private AudioSource footStep;

    private Vector3 nowPos;

    //[ContextMenu("TestAForce")]
    //public void TestAForce()
    //{
    //    Vector3 b = transform.TransformDirection(Vector3.forward);
    //    GetComponent<Rigidbody>().AddForce(b * 28);
    //}


    //单例
    private static PlayerCharacter _player;
    private PlayerCharacter()
    {

    }

    public static PlayerCharacter player
    {
        get
        {
            if (_player == null)
            {
                _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
            }
            return _player;
        }
    }

    //private bool isCollecting = false;
    private bool openKanpSack = false;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        nowPos = transform.position;

        footStep = GetComponent<AudioSource>();
        if (bloodLine != null)
        {

            bloodLine.maxValue = maxHealth;
            bloodLine.value = health;
        }

        currentMeleeWeapon = GetComponentInChildren<MeleeWeapon>();
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameObjectController>();
        buildList = GameObject.Find("MiniMapCanvas").transform.Find("BuildList").gameObject;
        knapSack = KnapSackManager.instance;

        rigid.constraints = RigidbodyConstraints.None |
           RigidbodyConstraints.FreezeRotationX |
           RigidbodyConstraints.FreezeRotationY |
           RigidbodyConstraints.FreezeRotationZ |
           RigidbodyConstraints.FreezePositionY;

    }

    #region 控制器
    Vector3 move = Vector3.zero;
    protected override void UpdateControl()
    {

        #region 判断是否可操作角色

        if (controller.controlState != GameObjectController.ControlState.Player)
            return;



        if (Input.GetButtonDown("KnapSack"))
        {
            OnOpenBag();
        }

        if (openKanpSack) return;

        #endregion

        base.UpdateControl();

        if (controlMode == ControlMode.KeyBoard)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            move = v * Vector3.forward + h * Vector3.right;
        }
        else if (controlMode == ControlMode.Joystick)
        {
            move = JoyStickMoveMent.instance.move;
        }

        Movement(move);

        //攻击
        if (controlMode == ControlMode.KeyBoard)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();

            }

            //切换行动状态
            else if (Input.GetButtonDown("StandOrCrouch"))
            {
                if (walkState == WalkState.Stand)
                {
                    walkState = WalkState.Crouch;

                }
                else if (walkState == WalkState.Crouch)
                {
                    walkState = WalkState.Stand;
                }
            }

            //捡道具
            else if (Input.GetButtonDown("Collect"))
            {
                Collect();
            }

            //清空trigger
            if (Input.GetButtonUp("Collect"))
            {
                animator.ResetTrigger("Collect");
            }
            else if (Input.GetButtonUp("Fire1"))
            {

                //if (attackMethod == AttackMethod.Melee)
                //    animator.ResetTrigger("Melee");

            }

            //建造 
            else if (Input.GetButtonDown("Build"))
            {
                Building();
            }

        }



    }

    #endregion

    //近战攻击
    private void Melee()
    {
        animator.SetTrigger("Melee");
    }

    //远程攻击
    private void Shoot(GameObject target)
    {
        if (shootWeapon != null)
        {

            shootWeapon.OnAttackOnce(target);

        }

    }


    //拾取道具
    public void Collect()
    {
        if (controller.controlState != GameObjectController.ControlState.Player)
            return;
        if (isGrounded)
        {
            PlayerSensor.sensor.ItemInView();
            animator.SetTrigger("Collect");
        }
    }

    //动画相关
    protected override void UpdateAnimator()
    {
        animator.SetFloat("Forward", forwardAmount, 0.01f, Time.deltaTime);
        animator.SetBool("OnGround", isGrounded);
        animator.SetBool("Crouching", walkState == WalkState.Crouch);

    }

    //切换远程武器
    public void GetShootWeapon()
    {
        shootWeapon = GetComponentInChildren<ShootWeapon>();
        attackMethod = AttackMethod.Shoot;
        animator.SetBool("OnShootWeapon", attackMethod == AttackMethod.Shoot);
    }

    public override void TakeDamage(DamageEventData damageData)
    {
        base.TakeDamage(damageData);
        bloodLine.value = health;
    }


    private bool CheckAmmo()
    {
        if (knapSack.itemBox.ContainsKey(10002))
        {
            if (knapSack.itemNum[10002] > 0)
            {
                knapSack.itemNum[10002] -= 1;
                knapSack.itemInGrid[10002].GetComponentInChildren<Text>().text = knapSack.itemNum[10002].ToString();
                return true;
            }
        }
        return false;
    }


    //处理攻击
    public void Attack()
    {
        if (controller.controlState != GameObjectController.ControlState.Player)
            return;
        if (attackMethod == AttackMethod.Melee)
        {
            Melee();
        }

        if (attackMethod == AttackMethod.Shoot)
        {
            animator.Play("Shoot");
            PlayerSensor.sensor.TargetInView();
            if (sensor.target != null)
            {

                Shoot(sensor.target);
            }
        }
    }

    float audioPlayTime = 0;

    public override void Movement(Vector3 move)
    {
        base.Movement(move);
        if ((nowPos - transform.position).magnitude > 0.3f)
        {
            if (audioPlayTime < Time.time)
            {
                
                footStep.Play();
                audioPlayTime = Time.time + 0.4f;
                nowPos = transform.position;
            }
           
        }
        
    }

    //处理打开背包
    public void OnOpenBag()
    {
        if (controller.controlState != GameObjectController.ControlState.Player)
            return;
        var root = GameObject.Find("KnapSack");
        //关闭背包
        if (openKanpSack)
        {
            root.transform.Find("BackGroundUI").gameObject.SetActive(false);
            root.GetComponent<KnapSackManager>().CloseTip();
            root.GetComponent<KnapSackManager>().isOpen = false;
            root.GetComponent<KnapSackManager>().CameraOpen(false);
            GameObject.Find("CameraBox").transform.Find("Main Camera").gameObject.SetActive(true);
            openKanpSack = false;
        }
        //打开背包
        else
        {
            rightAmount = 0;
            forwardAmount = 0;
            turnAmount = 0;
            root.transform.Find("BackGroundUI").gameObject.SetActive(true);
            root.GetComponent<KnapSackManager>().isOpen = true;
            root.GetComponent<KnapSackManager>().CameraOpen(true);
            Camera.main.gameObject.SetActive(false);

            openKanpSack = true;
        }
    }

    //处理建造
    public void Building()
    {
        if (controller.controlState != GameObjectController.ControlState.Player || !enterBuild)
            return;

        //玩家所有移动改为0，控制对象改为物品
        rightAmount = 0;
        forwardAmount = 0;
        turnAmount = 0;
        controller.controlState = GameObjectController.ControlState.Builder;
        //var go = Instantiate(buildPrefab, transform.position, Quaternion.identity, buildings.transform);

        //激活建造平台
        buildSys.SetActive(true);

        //var go = GameObjectPool.inventoryPool.GetObject(buildPrefab);
        //go.transform.position = transform.position;
        //go.transform.SetParent(buildings.transform);
        //go.SetActive(true);
        buildList.SetActive(true);
        buildButton.SetActive(false);
        finishButton.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        rigid.isKinematic = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        rigid.isKinematic = false;
    }


    public override void Die()
    {
        base.Die();
        GameObjectController.gameControl.ShowEndUI();
    }
}
