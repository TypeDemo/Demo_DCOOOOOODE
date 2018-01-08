using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AISensor : Sensor
{
    

    public float scanAngle = 90;
    public float scanDist = 10.0f;

    public bool debugVisual;

    public float sightDistance = 10;

    public float sightLineNum = 30;

    [HideInInspector]
    public bool beHit=false;
    
    private LayerMask layerMask;

    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public bool find;

    private void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Build");
    }

    void Update()
    {

        FiledOfView();
    }
        

    void FiledOfView()
    {
        
        Vector3 offSet = Vector3.zero;
        float angle;
        //var start = transform.position + Vector3.up * 1.0f;
        //var forwarLeft = start + Quaternion.Euler(0, -90.0f, 0) * transform.forward * scanDist;
        //var forwardRight = start + Quaternion.Euler(0, 90.0f, 0) * transform.forward * scanDist;
        //Debug.DrawLine(start, forwarLeft, Color.cyan);
        //Debug.DrawLine(start, forwardRight, Color.cyan);
        
        Collider[] targets = Physics.OverlapSphere(transform.position, scanDist, layerMask.value);

        if (targets.Length > 0)
        {
            foreach (var n in targets)
            {
                var temp = n.GetComponent<Character>();
                offSet = n.transform.position - transform.position;
                angle = Vector3.Angle(transform.forward, offSet);
                RaycastHit hitInfo;

                if (Physics.Raycast(transform.position, offSet, out hitInfo, scanDist, layerMask.value))
                {
                    
                    if (temp==null||temp.isDead)
                        continue;
                    if(hitInfo.collider.CompareTag("Bomb")|| hitInfo.collider.CompareTag("Builder"))
                    {
                        if(angle <= scanAngle)
                        {
                            target = n.transform;
                            find = true;
                            break;
                        }
                          
                    }
                    else if (hitInfo.collider.CompareTag("Player"))
                    {
                        if (angle <= scanAngle)
                        {
                            target = n.transform;
                            find = true;
                            break;
                        }
                        
                    }

                }
            }
        }
        else if(targets.Length==0)
        {
            find = false;
            target = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, scanDist);

    }


}
