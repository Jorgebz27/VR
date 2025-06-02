using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class ArrowImpact : MonoBehaviour
{
    [Header("Impact Settings")]
    [SerializeField] private bool explodeOnImpact = false;
    [SerializeField] private float stickDuration = 0.3f;
    [SerializeField] private float minEmbedDepth = 0.05f;
    [SerializeField] private float maxEmbedDepth = 0.15f;
    [SerializeField] private LayerMask ignoreLayers;
    [SerializeField] private Transform tip;
    
    private ArrowLauncherScript arrowLauncher;
    private Rigidbody rb;
    private bool hasHit = false;
    private ArrowSpawner spawnerRef;
    
    private void Awake()
    {
        arrowLauncher = GetComponent<ArrowLauncherScript>();
        rb = GetComponent<Rigidbody>();
    }

    public void SetSpawner(ArrowSpawner spawner)
    {
        spawnerRef = spawner;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit || ((1 << collision.gameObject.layer) & ignoreLayers) != 0)
        {
            return;
        }
        
        hasHit = true;
        arrowLauncher.StopFlight();

        if (explodeOnImpact)
        {
            HandleExplotion();
        }
        else
        {
            HandleStick(collision);
        }
    }
    
    private void HandleExplotion()
    {
        Debug.Log("Explode");
        if (spawnerRef != null)
        {
            spawnerRef.ReturnArrowToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HandleStick(Collision collision)
    {
        Vector3 arrowDirection = transform.forward;
        Vector3 arrowUp = transform.up;
        ContactPoint contact = collision.GetContact(0);
        
        float randomDepth = Random.Range(minEmbedDepth, maxEmbedDepth);
        Quaternion finalRotation = Quaternion.LookRotation(arrowDirection, arrowUp);
        Vector3 centerOffset = tip.localPosition;
        Vector3 finalPosition = contact.point - (finalRotation * centerOffset) + contact.normal * -randomDepth;
        
        transform.SetPositionAndRotation(finalPosition, finalRotation);
        
        CreateStabJoint(collision, randomDepth);
        
        transform.SetParent(collision.transform, true);
        StartCoroutine(DespawnAfterDelay());
    }

    public ConfigurableJoint CreateStabJoint(Collision collision, float randomDepth)
    {
        var joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = collision.rigidbody;
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        var limit = joint.linearLimit;
        limit.limit = randomDepth;
        joint.linearLimit = limit;
        return joint;
    }

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(stickDuration);
        if (spawnerRef != null)
        {
            spawnerRef.ReturnArrowToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ResetState()
    {
        hasHit = false;
        StopAllCoroutines();
        
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        if (joint != null)
        {
            joint.connectedBody = null;
            Destroy(joint);
        }

        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
    }
}
