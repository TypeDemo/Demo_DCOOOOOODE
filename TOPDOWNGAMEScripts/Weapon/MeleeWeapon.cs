using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public LayerMask playerLayer;
    public LayerMask buildLayer;

    private int layerNum;

    public float damage = 15.0f;

    [Tooltip("配置此武器的所有HitBoxes")]
    public List<HitBox> hitBoxes;

    private Dictionary<HitBox, List<GameObject>> hitObjectCache;

    private bool canApplyDamage;

    public bool debugVisual;


    // Use this for initialization
    protected virtual void Start()
    {
        hitObjectCache = new Dictionary<HitBox, List<GameObject>>();
        if (hitBoxes.Count > 0)
        {
            foreach (HitBox hitBox in hitBoxes)
            {
                hitBox.weapon = this;

                hitObjectCache.Add(hitBox, new List<GameObject>());

            }

        }
        else
        {
            this.enabled = false;
        }
    }



    public virtual void SetActiveDamage(bool value)
    {
        canApplyDamage = value;
        for (int i = 0; i < hitBoxes.Count; i++)
        {
            var hitCollider = hitBoxes[i];
            hitCollider.trigger.enabled = value;
            if (value == false && hitObjectCache != null)
                hitObjectCache[hitCollider].Clear();
        }
    }

    public virtual void OnHit(HitBox hitBox, Collider other)
    {

        layerNum = 1 << other.gameObject.layer;

        if (layerNum != playerLayer.value&&layerNum != buildLayer.value)
            return;
        if (canApplyDamage &&
            !hitObjectCache[hitBox].Contains(other.gameObject) &&
            (user != null && other.gameObject != user.gameObject))
        {
            hitObjectCache[hitBox].Add(other.gameObject);
            var damageAble = other.GetComponent<IDamageable>();
            if (damageAble != null)
            {

                var damageData = new DamageEventData(-damage, user);
                damageAble.TakeDamage(damageData);

            }
        }
    }
}
