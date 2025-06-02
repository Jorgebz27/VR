using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowLauncherScript : MonoBehaviour
{
    [Header("Launch Settings")] 
    [SerializeField] private float speed = 10;

    private Rigidbody rb;
    public bool inAir = false;
    private XRPullInteractable _pullInteractable;
    private bool isSubscribedToPull = false;

    private void Awake()
    {
        InitializeComponents();
        SetPhysics(false);
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"Rb not found on Arrow {gameObject.name}");
        }
    }

    public void Initialize(XRPullInteractable pullInteractable)
    {
        _pullInteractable = pullInteractable;
        if (_pullInteractable != null && !isSubscribedToPull)
        {
            _pullInteractable.PullActionReleased += Release;
            isSubscribedToPull = true;
        }
    }
    
    public void DetachFromBow(XRPullInteractable pullInteractable)
    {
        if (pullInteractable != null && isSubscribedToPull)
        {
            pullInteractable.PullActionReleased -= Release; //
            isSubscribedToPull = false;
        }
        _pullInteractable = null;

        if (transform.parent != null) {
        }
    }

    private void OnDestroy()
    {
        if (_pullInteractable != null && isSubscribedToPull)
        {
            _pullInteractable.PullActionReleased -= Release;
            isSubscribedToPull = false;
        }
    }

    public void Release(float value)
    {
        if (_pullInteractable != null && isSubscribedToPull)
        {
            _pullInteractable.PullActionReleased -= Release;
            isSubscribedToPull = false;
        }

        gameObject.transform.parent = null;
        inAir = true;
        SetPhysics(true);

        Vector3 force = transform.forward * value * speed;
        rb.AddForce(force, ForceMode.Impulse);

        StartCoroutine(RotateWithVelocity());
    }

    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (inAir)
        {
            if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(rb.linearVelocity, transform.up);
            }
            yield return null;
        }
    }
    
    public void StopFlight()
    {
        inAir = false;
        SetPhysics(false);
        StopCoroutine(RotateWithVelocity());
    }
    
    private void SetPhysics(bool usePhysics)
    {
        if (rb != null)
        {
            rb.useGravity = usePhysics;
            rb.isKinematic = !usePhysics;
        }
    }
    
    public void ResetState()
    {
        StopFlight();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public bool IsSubscribed()
    {
        return isSubscribedToPull;
    }
}
