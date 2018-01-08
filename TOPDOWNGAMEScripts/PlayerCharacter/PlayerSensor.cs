using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{

    public float scanDist = 15.0f;

    public float itemDistance = 3.5f;

    [HideInInspector]
    public GameObject target;

    private AnimatorStateInfo fullBodyInfo;
    private List<Collider> targetInView;


    //单例
    private static PlayerSensor _sensor;
    private PlayerSensor()
    {

    }

    public static PlayerSensor sensor
    {
        get
        {
            if (_sensor == null)
            {
                _sensor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSensor>();
            }
            return _sensor;
        }
    }

    private void Start()
    {
        
        //itemInView = new List<Collider>();
        targetInView = new List<Collider>();
    }
        

    //获取道具
    public void ItemInView()
    {
        
        Collider[] targets = Physics.OverlapSphere(transform.position, itemDistance, LayerMask.GetMask("Item"));
        if (targets.Length > 0)
        {
            foreach (Collider collider in targets)
            {
                GetItem(targets);

            }

        }

    }

    //获取敌人
    public void TargetInView()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, scanDist, LayerMask.GetMask("Target"));
        targetInView = new List<Collider>();

        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (!targets[i].GetComponent<Character>().isDead)
                {
                    targetInView.Add(targets[i]);
                }
            }

            GetClosestTarget(targetInView);

        }
        else
        { target = null; }

    }



    //最近道具
    private void GetItem(Collider[] targets)
    {

        if (targets.Length > 0)
        {
            Vector3 offset;
            for (int i = 0; i < targets.Length; i++)
            {
                offset = targets[i].transform.position - transform.position;

                if (offset.magnitude < itemDistance)
                {
                    KnapSackManager.instance.StoreItem(targets[i].GetComponent<CollectableItem>().id);

                    Destroy(targets[i].gameObject);
                }
            }
        }
    }


    //最近敌人
    private void GetClosestTarget(List<Collider> targets)
    {
        if (targets.Count > 0)
        {

            int index = 0;
            float minDistance = (targets[index].transform.position - transform.position).sqrMagnitude;

            for (int i = 0; i < targets.Count; i++)
            {

                float temp = (targets[i].transform.position - transform.position).sqrMagnitude;
                if (minDistance > temp)
                {
                    minDistance = temp;
                    index = i;
                }

            }
            target = targets[index].gameObject;

        }
        else
        {
            target = null;
        }
      
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, scanDist);

    }
}
