﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;


public enum MovementMode
{
    Rigidibody,
    NavMeshBody,

}

public class Character : MonoBehaviour, IDamageable
{



    public UnityEvent onDead;
    public AudioClip dieSound;
    public Weapon equippedWeapon;

    //状态
    protected bool isCrouching = false;
    protected bool isGrounded = false;
    protected bool isDead = false;
    protected bool isAiming = false;

    [Range(0.1f, 5)]
    [SerializeField]
    protected float speed = 5;

    [Range(1f, 500)]
    [SerializeField]
    protected float angularSpeed = 360;

    [SerializeField]
    protected float groundCheckDistance = 0.3f;

    protected Rigidbody rigid;
    protected Animator anim;
    protected CapsuleCollider capsule;
    protected float turnAmount;
    protected float forwardAmount;
    protected float rightAmount;
    protected Vector3 velocity;
    protected Vector3 groundNormal;
    protected float defaultCapsuleHeight;
    protected Vector3 defaultCapsuleCenter;

    #region Public

    public bool Fire(bool continuously)
    {
        if (!isAiming) return false;

        if (equippedWeapon == null)
            return false;

        bool attackWasSuccessful;

        if (continuously)
        {
            attackWasSuccessful = equippedWeapon.AttackContinuouslyHandle();
        }

        else
            attackWasSuccessful = equippedWeapon.AttackOnceHandle();

        //if (attackWasSuccessful)
        //{
        //    anim.SetTrigger("Fire");
        //}

        return attackWasSuccessful;
    }

    public void EquipWeapon(Weapon Weapon)
    {
        if (Weapon)
        {
            SetCurrentWeapon(Weapon, equippedWeapon);
        }
    }

    public void UnEquipWeapon(Weapon Weapon)
    {
        if (Weapon && Weapon == equippedWeapon)
        {
            SetCurrentWeapon(null, Weapon);
        }
    }

    public void Movement(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        //将move从世界空间转向本地空间
        move = transform.InverseTransformDirection(move);
        //将move投影在地板的2D平面上
        move = Vector3.ProjectOnPlane(move, groundNormal);
        //返回值为x轴和一个（在零点起始，在(x, y)结束）的2D向量的之间夹角
        turnAmount = Mathf.Atan2(move.x, move.z);
        forwardAmount = move.z;
        rightAmount = move.x;
    }

    public void Crouching(bool crouch)
    {
        if (isGrounded && crouch)
        {
            if (isCrouching) return;
            capsule.height = capsule.height / 2f;
            capsule.center = capsule.center / 2f;
            isCrouching = true;
        }
        else
        {
            //限制头顶有遮挡时，必须蹲下
            Ray crouchRay = new Ray(rigid.position + Vector3.up * capsule.radius * 0.5f, Vector3.up);
            float crouchRayLength = defaultCapsuleHeight - capsule.radius * 0.5f;
            if (Physics.SphereCast(crouchRay, capsule.radius * 0.5f, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                isCrouching = true;
                return;
            }
            capsule.height = defaultCapsuleHeight;
            capsule.center = defaultCapsuleCenter;
            isCrouching = false;
        }
    }

    public void Aiming(bool aiming)
    {
        if (isGrounded && aiming)
        {
            equippedWeapon.gameObject.SetActive(true);
            isAiming = true;
        }
        else
        {
            equippedWeapon.gameObject.SetActive(false);
            isAiming = false;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Movement(Vector3.zero);
        anim.Play("Die");
        capsule.height = 0.2f;
        capsule.center = new Vector3(0, 0.3f, 0);
        AudioSource.PlayClipAtPoint(dieSound, transform.position);
        var agent = GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        //停止行为树
        var bt = GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>();
        if (bt)
        {
            bt.StopBehaviour();
        }

        onDead.Invoke();
    }
    #endregion

    #region Interface

    public virtual void TakeDamage(DamageEvent damageData)
    {
        Die();

    }

    #endregion

    #region Private

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;

        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));

        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
        {
            groundNormal = hitInfo.normal;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }
    }

    void SetCurrentWeapon(Weapon NewWeapon, Weapon LastWeapon /*= NULL*/)
    {
        Weapon LocalLastWeapon = null;

        if (LastWeapon != null)
        {
            LocalLastWeapon = LastWeapon;
        }
        else if (NewWeapon != equippedWeapon)
        {
            LocalLastWeapon = equippedWeapon;
        }

        if (LocalLastWeapon)
        {
            LocalLastWeapon.OnUnEquip();
        }

        equippedWeapon = NewWeapon;

        if (NewWeapon)
        {

            NewWeapon.OnEquip();
        }
    }

    #endregion

    #region Cycle

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        defaultCapsuleHeight = capsule.height;
        defaultCapsuleCenter = capsule.center;
        rigid.drag = 8;
        rigid.mass = 30;
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            UpdateControl();
        }
        CheckGroundStatus();
        UpdateMovement();
        UpdateAnimator();
    }

    protected virtual void UpdateControl() { }

    protected virtual void UpdateMovement()
    {
        //转向控制
        transform.Rotate(0, turnAmount * angularSpeed * Time.deltaTime, 0);

        //移动控制
        velocity = transform.forward * forwardAmount * speed;
        if (isCrouching || isAiming) velocity *= 0.5f;
        velocity.y = rigid.velocity.y;
        rigid.velocity = velocity;
    }

    protected virtual void UpdateAnimator()
    {
        anim.SetFloat("Forward", forwardAmount, 0.01f, Time.deltaTime);
        anim.SetFloat("Right", rightAmount, 0.01f, Time.deltaTime);
        anim.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        anim.SetBool("Crouch", isCrouching);
        anim.SetBool("OnGround", isGrounded);
        anim.SetBool("Aiming", isAiming);
    }
    #endregion
}

