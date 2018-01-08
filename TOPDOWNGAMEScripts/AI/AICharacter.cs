using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AISensor))]
public class AICharacter : Character
{
    Vector3 hitPos;
    Transform hitTransform;
    private Timer timer;

    private GameObject player;

    NavMeshAgent agent;
    AISensor sensor;
    bool hit = false;
    int currentIndex = 0;

    private List<Transform> points;
    Vector3 targetPos;

    private void OnEnable()
    {
        animator.SetBool("Dead", isDead);
    }

    protected override void Start()
    {
        base.Start();
        timer = GameObject.Find("TimeText").GetComponent<Timer>();
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        sensor = GetComponent<AISensor>();

        points = GameObject.Find("WayPointGroup").GetComponent<WaypointGroup>().waypoints;

    }

    protected override void FixedUpdate()
    {
        if (InAnimatorStateWithTag("RootMotion"))
        {
            agent.velocity = Vector3.zero;
        }
    }

    protected override void UpdateControl()
    {
        base.UpdateControl();
        if (sensor.find)
        {
            speed = 5.0f;
            UpdateMoveToPlayer();
        }
        else if (timer.timeOut)
        {
            speed = 4.0f;
            if (!agent.SetDestination(player.transform.position))
            {
                return;
            }
        }
        else if (!sensor.find)
        {
            if (hit)
            {
                speed = 4.0f;
                if (!agent.SetDestination(hitPos))
                {
                    return;
                }
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    hit = false;
                }
            }
            else
            {
                speed = 2.0f;
                UpdatePatrol();
            }
        }
        Movement(agent.velocity);

    }

    protected override void UpdateMovement()
    {

        //转向控制
        transform.Rotate(0, turnAmount * angularSpeed * Time.deltaTime, 0);

        //移动控制
        agent.speed = speed;
        agent.angularSpeed = angularSpeed;
        velocity = agent.velocity;
    }


    public override void Die()
    {
        if (isDead) return;
        health = 0.0f;
        isDead = true;
        Movement(Vector3.zero);
        animator.SetBool("Dead", isDead);
        animator.Play("Die");
        capsule.height = 0.2f;
        capsule.center = new Vector3(0, 0.3f, 0);

        if (agent)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        Invoke("Restart", 2.0f);
    }

    private void Restart()
    {

        GameObjectPool.inventoryPool.ReturnObject(this.gameObject);
    }


    //巡逻
    void UpdatePatrol()
    {


        targetPos = points[currentIndex].transform.position;


        if (!agent.SetDestination(targetPos))
        {
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {

            currentIndex = Random.Range(0, points.Count);

        }
    }


    //走向玩家
    void UpdateMoveToPlayer()
    {


        if (!agent.SetDestination(sensor.target.position))
        {

            return;
        }


        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.Play("MeleeAttack");

        }


    }




    //产生伤害
    public override void TakeDamage(DamageEventData damageData)
    {

        if (damageData == null) return;

        if (health > Mathf.Abs(damageData.delta) || damageData.delta > 0)
        {
            health += damageData.delta;
            hitTransform = damageData.attacker.transform;
            hitPos = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z);
            hit = true;
            GetEffect();

        }
        else
        {
            if (!isDead) Die();
        }
    }


    //动画相关
    protected override void UpdateAnimator()
    {
        animator.SetFloat("Forward", forwardAmount, 0.01f, Time.deltaTime);
        animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
    }

    public override void RefreshAnimatorState()
    {
        if (animator == null || !animator.enabled) return;
        baseLayerInfo = animator.GetCurrentAnimatorStateInfo(BaseLayer);
    }


    public override bool InAnimatorStateWithTag(string tag)
    {
        RefreshAnimatorState();

        if (animator == null) return false;
        if (baseLayerInfo.IsTag(tag)) return true;

        return false;
    }

    //重置
    public void ResetObject()
    {

        agent.enabled = true;
        sensor.target = null;
        sensor.find = false;

        isDead = false;
        health = 30;
        hit = false;
        capsule.height = 4.8f;
        capsule.center = new Vector3(0, 2.4f, 0);

        agent.updatePosition = true;
        agent.updateRotation = true;



    }

    private void GetEffect()
    {
        var go = Resources.Load<GameObject>("Effects/Blood");
        var blood = GameObjectPool.inventoryPool.GetObject(go);

        blood.transform.SetParent(hitPosition);
        blood.transform.localPosition = Vector3.zero;
        blood.SetActive(true);
    }

    //private void OnEnable()
    //{
    //    ResetObject();
    //}
    //[ContextMenu("Test")]
    //public void Test()
    //{
    //    this.gameObject.SetActive(true);
    //    this.isDead = false;

    //    GameObjectPool.inventoryPool.GetObject(this.gameObject);
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            currentIndex = Random.Range(0, points.Count);

        }
    }
}
