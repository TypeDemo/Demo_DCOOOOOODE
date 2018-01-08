
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceTower : Character
{
    public GameObject root;

    public ShootWeapon shootWeapon;

    public float scanDist = 10.0f;

    public float sightDistance = 10.0f;

    private LayerMask layerMask;

    public string targetTag = "Target";

    public TowerRotate towerHead;

    [HideInInspector]
    public Transform target;

    #region 重载函数清空
    protected override void Awake()
    {

    }

    protected override void FixedUpdate()
    {

    }

    protected override void UpdateAnimator()
    {
    }

    protected override void UpdateControl()
    {
    }

    protected override void UpdateStatus()
    {
    }

    protected override void UpdateMovement()
    {

    }

    public override bool InAnimatorStateWithTag(string tag)
    {
        return false;
    }

    public override void Movement(Vector3 move)
    {

    }


    public override void RefreshAnimatorState()
    {

    }

    #endregion


    public override void Die()
    {
        if (isDead) return;
        health = 0.0f;
        isDead = true;
        GetEffect();
        Invoke("ResetState",1.0f);
        
        
    }

    protected override void Update()
    {
        if (!isDead)
        {
            FiledOfView();
        }
       
    }

    private void FiledOfView()
    {
        Vector3 offSet = Vector3.zero;

        target = null;
       
        Collider[] targets = Physics.OverlapSphere(transform.position, scanDist, LayerMask.GetMask(targetTag));
        if (targets.Length > 0)
        {
            foreach (var n in targets)
            {
                //offSet = n.transform.position + Vector3.up * n.GetComponent<CapsuleCollider>().height / 2 - transform.position;
                //RaycastHit hitInfo;

                //if (Physics.Raycast(transform.position, offSet, out hitInfo, scanDist))
                //{
                    if (n.CompareTag("Enemy")&&n.gameObject.GetComponent<Character>().isDead==false)
                    {
                        target = n.transform;
                        towerHead.findTarget = true;

                        shootWeapon.OnAttackOnce(n.gameObject);

                        break;
                    }

                //}
            }
        }
        else
        {
            towerHead.findTarget = false;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10.0f);

    }

    protected  virtual void ResetState()
    {
        health = 100;
        isDead = false;
        root.GetComponent<BuildItem>().isBuilded = false;
        root.GetComponent<BuildItem>().enabled = true;
        root.GetComponent<BuildItem>().onBuildGround = true;
        root.transform.Find("Field").gameObject.SetActive(true);
        GetComponent<Collider>().enabled = false;
        GetComponent<TowerRotate>().enabled = false;
        GetComponent<DefenceTower>().enabled = false;
        GameObjectPool.inventoryPool.ReturnObject(root);
    }

    private void GetEffect()
    {
        var go = Resources.Load<GameObject>("Effects/TowerDestroyed");
        var tower = GameObjectPool.inventoryPool.GetObject(go);

        tower.transform.SetParent(hitPosition);
        tower.transform.localPosition = Vector3.zero;
        tower.SetActive(true);
    }
}
