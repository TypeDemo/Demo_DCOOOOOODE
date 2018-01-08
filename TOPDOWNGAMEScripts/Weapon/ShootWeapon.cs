using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootWeapon : Weapon
{

    public Transform firePosition;
    public LayerMask mask;

    private KnapSackManager knapSack;

    public enum FireMode
    {
        //半自动
        SemiAuto,
        //全自动
        FullAuto
    }

    [SerializeField]
    private FireMode fireMode;

    public float damage = 10.0f;


    [SerializeField]
    private int shotsPerMinute = 450;
    [SerializeField]
    private float shotDuration = 0.22f;

    private float timeBetweenShotsMin;
    private float nextTimeCanFire;
    private AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        knapSack = KnapSackManager.instance;
        mask = ~(1 << LayerMask.NameToLayer("HitBox") | 1 << LayerMask.NameToLayer("Item") | 1 << LayerMask.NameToLayer("Build") | 1 << LayerMask.NameToLayer("Player"));

        if (fireMode == FireMode.SemiAuto)
            timeBetweenShotsMin = shotDuration;
        else
            timeBetweenShotsMin = 60f / shotsPerMinute;
    }

    //射击一次
    public bool OnAttackOnce(GameObject target)
    {

        if (Time.time < nextTimeCanFire)
            return false;
        nextTimeCanFire = Time.time + timeBetweenShotsMin;
        Shoot(target);
        return true;
    }

    //连续射击
    public bool OnAttackContinuously(GameObject target)
    {
        if (fireMode == FireMode.SemiAuto)
            return false;

        return OnAttackOnce(target);
    }

    //射击逻辑
    protected void Shoot(GameObject target)
    {
        if (target == null)

            return;


        var firePos = new Vector3(user.transform.position.x,
            user.transform.position.y + user.GetComponent<CapsuleCollider>().height / 2,
            user.transform.position.z);

        Vector3 dir = target.transform.position + Vector3.up * target.GetComponent<CapsuleCollider>().height / 2 - firePos;

        RaycastHit hitInfo;

        //射线检测是否与目标之间有障碍物
        if (Physics.Raycast(firePos, dir, out hitInfo, dir.magnitude, mask.value))
        {

            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Target"))
            {
                var damageAble = hitInfo.collider.GetComponent<IDamageable>();
                if (damageAble != null)
                {
                    

                    if (user.CompareTag("Player"))
                    {
                        if (!knapSack.CastItem(10002))
                        {
                            return;
                        }
                    }
                    audioSource.Play();
                    //生成特效
                    var go = Resources.Load<GameObject>("Effects/Fire");
                    var fire = GameObjectPool.inventoryPool.GetObject(go);

                    fire.transform.SetParent(firePosition);
                    fire.transform.localPosition = Vector3.zero;
                    fire.SetActive(true);

                    Vector3 temp = new Vector3(target.transform.position.x, user.transform.position.y, target.transform.position.z);
                    user.transform.LookAt(temp);

                    var damageData = new DamageEventData(-damage, user, hitInfo.point, dir);
                    damageAble.TakeDamage(damageData);
                    if (enemyInfoUI != null)
                    {
                        //生成信息框
                        var info = hitInfo.collider.GetComponent<AICharacter>();

                        var tipUI = enemyInfoUI.GetComponent<EnemyInfoUI>();

                        tipUI.ShowInfo(hitInfo.collider.name, info.maxHealth, info.health);
                    }
                }

            }

        }

    }
}
