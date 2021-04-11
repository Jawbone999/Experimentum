using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NavNode : MonoBehaviour
{
    public NavNode next;

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, .25f);
        Gizmos.DrawLine(transform.position, next.transform.position);
    }
}
