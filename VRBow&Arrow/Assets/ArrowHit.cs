using System;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowHit : MonoBehaviour
{
    
    public Rigidbody floorrb;
    public FixedJoint[] joints;
    private bool hasHit = false;

    private void OnCollisionEnter(Collision other)
    {
        if (hasHit)
        {
            return;
        }
        
        if (other.relativeVelocity.magnitude > 2f)
        {
            hasHit = true;
            
            if (floorrb != null)
                DropFloor();

            ReleaseTarget();
        }
    }

    private void ReleaseTarget()
    {
        foreach (FixedJoint joint in joints)
        {
            Destroy(joint);
        }
    }

    private void DropFloor()
    {
        floorrb.isKinematic = false;
        floorrb.useGravity = true;
    }
}
