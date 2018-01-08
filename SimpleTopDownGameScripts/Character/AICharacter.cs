using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacter : Character
{
    NavMeshAgent agent;
    public int startHP = 1000;

    protected override void Start()
    {
        base.Start();
                
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void UpdateControl()
    {
        base.UpdateControl();
        Movement(agent.velocity);
       
    }

    public override void TakeDamage(DamageEvent damageData)
    {
        if(startHP <= 0)
        {
            Die();

            Destroy(gameObject, 4);
        }

        startHP -= 10;

    }

    protected override void UpdateMovement()
    {
        //转向控制
        transform.Rotate(0, turnAmount * angularSpeed * Time.deltaTime, 0);

        //移动控制
        speed = agent.speed;
        angularSpeed = agent.angularSpeed;
        velocity = agent.velocity;
    }
}
