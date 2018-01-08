using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBuildCharacter : DefenceTower {

    public override void Die()
    {
        base.Die();
        bomb.SetActiveDamage(true);
    }

    protected MeleeWeapon bomb;

    protected override void Start()
    {
        base.Start();
        bomb = GetComponent<MeleeWeapon>();
    }
    protected override void ResetState()
    {
        health = 100;
        isDead = false;
        root.GetComponent<BuildItem>().isBuilded = false;
        root.GetComponent<BuildItem>().enabled = true;
        root.GetComponent<BuildItem>().onBuildGround = true;
               GetComponent<Collider>().enabled = false;
               GetComponent<NormalBuildCharacter>().enabled = false;
        GameObjectPool.inventoryPool.ReturnObject(root);
    }

    // Update is called once per frame
    protected override void Update () {
		
	}
}
