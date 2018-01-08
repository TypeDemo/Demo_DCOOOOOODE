using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{


    //物理参数
    
    protected Rigidbody rigid;
    protected CapsuleCollider capsule;

    public float maxHealth = 100.0f;
    public float health = 100.0f;


    //行动相关参数
    protected Vector3 velocity;
    protected float groundCheckDistance = 0.4f;
    protected Vector3 groundNormal;     //为检测到的地面的法向量hitinfo.normal
    protected float turnAmount;
    protected float rightAmount;
    protected float forwardAmount;

    public Transform hitPosition;

    [SerializeField]
    protected float speed = 7;

    [SerializeField]
    protected float angularSpeed = 360;


    //状态参数
    protected bool isGrounded = false;
    [HideInInspector]
    public bool isDead = false;
    protected bool enterBuild = false;

    //武器
    public GameObject weaponSocket;

    [SerializeField]
    protected MeleeWeapon currentMeleeWeapon;

    protected enum WalkState
    {
        Stand,
        Crouch,
    }

    protected enum AttackMethod
    {
        Melee,
        Shoot,
    }
    [SerializeField]
    protected AttackMethod attackMethod = AttackMethod.Melee;

    protected WalkState walkState = WalkState.Stand;

    //动画相关
    protected Animator animator;
    protected AnimatorStateInfo baseLayerInfo;
    public AnimatorStateInfo fullBodyInfo;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateStatus();

        if (!isDead)
        {
            UpdateControl();

        }
        UpdateMovement();
        UpdateAnimator();
    }


    protected virtual void FixedUpdate()
    {
        if (InAnimatorStateWithTag("RootMotion")) return;
        rigid.MovePosition(rigid.position + velocity * Time.fixedDeltaTime);
    }

    //移动相关

    private void CheckGround()
    {
        RaycastHit hitInfo;
        Debug.DrawLine(transform.position + Vector3.up * 0.2f, transform.position + Vector3.down * groundCheckDistance);
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out hitInfo, groundCheckDistance))
        {
            isGrounded = true;
            groundNormal = hitInfo.normal;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }

        //进入建造区域
        if (Physics.Raycast(transform.position + Vector3.up * 1.0f, Vector3.down, out hitInfo, 2.0f,LayerMask.GetMask("Build")))
        {
            enterBuild = true;
           
        }
        else
        {
            enterBuild = false;
            
        }
    }



    public virtual void Movement(Vector3 move)
    {

        //当模大于1时，要进行归一化，防止在斜方向移动时，移动速度加快
        if (move.magnitude > 1f) move.Normalize();

        //将move从世界空间转向本地空间
        move = transform.InverseTransformDirection(move);
        //将move投影在地板的2D平面上
        move = Vector3.ProjectOnPlane(move, groundNormal);
        //返回值为x轴和一个（在零点起始，在(x, y)结束）的2D向量的之间夹角
        turnAmount = Mathf.Atan2(move.x, move.z);
        rightAmount = move.x;
        forwardAmount = move.z;
    }

    protected virtual void UpdateMovement()
    {
        if (InAnimatorStateWithTag("RootMotion"))
            turnAmount = 0.0f;
        transform.Rotate(0, turnAmount * angularSpeed * Time.deltaTime, 0);
        velocity = transform.forward * forwardAmount * speed;

        switch (walkState)
        {
            case WalkState.Stand:
                break;
            case WalkState.Crouch:
                velocity *= 0.5f;
                break;
            default:
                break;

        }
        velocity.y = rigid.velocity.y;


    }



    //控制相关
    protected virtual void UpdateControl()
    {

    }

    //武器相关

    public virtual void SetActiveMelee(bool active)
    {
        
        if (currentMeleeWeapon != null)
        {
        currentMeleeWeapon.SetActiveDamage(active);
        }

    }

    //状态相关
    protected virtual void UpdateStatus()
    {
        CheckGround();
    }

    public virtual void TakeDamage(DamageEventData damageData)
    {
        if (damageData == null) return;

        if (health > Mathf.Abs(damageData.delta) || damageData.delta > 0)
        {
            health += damageData.delta;

            //animator.Play("Hit");
        }
        else
        {
            
            if (!isDead) Die();
        }
    }

    public virtual void Die()
    {

        if (isDead) return;
        health = 0.0f;
        isDead = true;
        Movement(Vector3.zero);
        animator.Play("Die");
        rigid.velocity = Vector3.zero;
        capsule.height = 0.2f;
        capsule.center = new Vector3(0, 0.3f, 0);
               
    }

    //动画相关

    protected virtual void UpdateAnimator()
    {

    }

    public int BaseLayer { get { return animator.GetLayerIndex("Base Layer"); } }
    public int FullBody { get { return animator.GetLayerIndex("Full Body"); } }

    public virtual void RefreshAnimatorState()
    {
        if (animator == null || !animator.enabled) return;
        baseLayerInfo = animator.GetCurrentAnimatorStateInfo(BaseLayer);
        fullBodyInfo = animator.GetCurrentAnimatorStateInfo(FullBody);

    }

    public virtual bool InAnimatorStateWithTag(string tag)
    {
        RefreshAnimatorState();

        if (animator == null) return false;
        if (baseLayerInfo.IsTag(tag)) return true;
        if (fullBodyInfo.IsTag(tag)) return true;


        return false;
    }

    public void GetCurrentMeleeWeapon()
    {
        currentMeleeWeapon = GetComponentInChildren<MeleeWeapon>();
        attackMethod = AttackMethod.Melee;
        animator.SetBool("OnShootWeapon", attackMethod == AttackMethod.Shoot);
    }
        
        
}
