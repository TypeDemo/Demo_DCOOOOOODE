using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public int startHP = 100;

    bool canControl = true;

    public GameObject AimTargetVirsual;
    public Transform AimFinalPos;
    Vector3 finalPos;

    
    protected override void Update()
    {
        MouseTargetPos();
        base.Update();
    }

    protected override void UpdateMovement()
    {
        //移动控制
        if (!isAiming)
        {
            transform.Rotate(0, turnAmount * angularSpeed * Time.deltaTime, 0);
            velocity = transform.forward * forwardAmount * speed;
        }
        else
        {
            velocity = transform.forward * forwardAmount * speed + transform.right * rightAmount * speed;
            transform.LookAt(finalPos);
        }

        if (isCrouching || isAiming) velocity *= 0.5f;
        velocity.y = rigid.velocity.y;
        rigid.velocity = velocity;

    }

    protected override void UpdateControl()
    {
        if (!canControl)
            return;

        //得到坐标轴的输入值
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        //得到改变的向量值
        var moveInput = Vector3.forward * vertical + Vector3.right * horizontal;

        //得到蹲下的输入
        bool crouching = Input.GetKey(KeyCode.C);

        Movement(moveInput);
        Crouching(crouching);
        Aiming(Input.GetButton("Fire2"));

        if (Input.GetButton("Fire1") && isAiming)
        {
            Fire(true);
        }
    }

    public void EndPlay()
    {
        canControl = false;
        Movement(Vector3.zero);
        StartCoroutine(OnRestartLevel());
    }

    IEnumerator OnRestartLevel()
    {
        yield return new WaitForSeconds(3);
        GameController.Instance.ReloadScene();
    }



    private void MouseTargetPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 2000f, 9))
        {
            finalPos = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);

            AimTargetVirsual.transform.position = finalPos;
            AimTargetVirsual.transform.LookAt(transform.position);


        }

    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject ==null)
        {
            return;

        }
        StartCoroutine(CostHP(col));
        
    }

    IEnumerator CostHP(Collision col)
    {
        yield return new WaitForSeconds(1);
        
        if (col.gameObject.CompareTag("Enemy"))
        {
            startHP -= 5;
        }

        //else if (col.gameObject.CompareTag("Boss"))
        //{
        //    startHP -= 15;
        //}

        if (startHP <= 0)
        {
            Die();

            
        }
        
             
    }
}
