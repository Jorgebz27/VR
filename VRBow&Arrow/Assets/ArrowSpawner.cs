using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ArrowSpawner : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform notch;
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private int initialPoolSize = 3;
    [SerializeField] private Transform arrowPoolParent;
    [SerializeField] private XRSocketInteractor bowSocket;
    [SerializeField] private Transform bowSocketTransform;

    private XRGrabInteractable bow;
    private XRPullInteractable pullInteractable;
    private bool arrowNotched = false;
    private GameObject currentArrow = null;
    private List<GameObject> arrowPool = new List<GameObject>();
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        bow = GetComponent<XRGrabInteractable>();
        pullInteractable = GetComponent<XRPullInteractable>();

        if (arrowPoolParent == null)
        {
            arrowPoolParent = transform;
        }
    }

    private void Start()
    {
        PreWarmPool();
        if (pullInteractable != null)
        {
            pullInteractable.PullActionReleased += NotchEmpty;
        }
    }

    private void OnDestroy()
    {
        if (pullInteractable != null)
        {
            pullInteractable.PullActionReleased -= NotchEmpty;
        }

        foreach (var arrow in arrowPool)
        {
            if (arrow != null) Destroy(arrow);
        }
        arrowPool.Clear();
    }

    private void PreWarmPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowPoolParent);
            arrow.SetActive(false);
            arrowPool.Add(arrow);
        }
    }

    private void Update()
    {
        //if (bow.isSelected && !arrowNotched && currentArrow == null)
        //{
        //    arrowNotched = true;
        //    if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        //    spawnCoroutine = StartCoroutine(SpawnArrowAfterDelayCoroutine());
        //}
        //
        //if (!bow.isSelected && currentArrow != null)
        //{
        //    ArrowLauncherScript launcher = currentArrow.GetComponent<ArrowLauncherScript>();
        //    if (launcher != null && !launcher.inAir)
        //    {
        //        launcher.DetachFromBow(pullInteractable);
        //        ReturnArrowToPool(currentArrow);
        //    }
        //}
        
        if (bow.isSelected) //El arco se agarra
        {
            if (!arrowNotched && currentArrow == null)
            {
                arrowNotched = true;
                if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
                spawnCoroutine = StartCoroutine(SpawnArrowAfterDelayCoroutine());
            }
        }
        else                //(!bow.isSelected) El arco se suelta
        {     
            if (currentArrow != null)
            {
                ArrowLauncherScript launcher = currentArrow.GetComponent<ArrowLauncherScript>();
                if (launcher != null && !launcher.inAir)
                {
                    launcher.DetachFromBow(pullInteractable);
                    ReturnArrowToPool(currentArrow);
                }
            }
            
            //if (bowSocket != null && !bowSocket.hasSelection)
            //{
            //    Rigidbody bowRb = bow.GetComponent<Rigidbody>();
            //    if (bowRb != null)
            //    {   
            //        bowRb.isKinematic = true;
            //        bowRb.useGravity = false;
            //    }
            //    bow.transform.position = bowSocketTransform.position;
            //    bow.transform.rotation = bowSocketTransform.rotation;
            //}
        }
    }
    
    private void NotchEmpty(float value)
    {
        arrowNotched = false;
        currentArrow = null;
    }

    private IEnumerator SpawnArrowAfterDelayCoroutine()
    {
        yield return new WaitForSeconds(spawnDelay);
        
        if (bow.isSelected && currentArrow == null)
        {
            SpawnAndNotchArrow();
        }
        else
        {
            arrowNotched = false;
        }
        spawnCoroutine = null;
    }

    private void SpawnAndNotchArrow()
    {
        currentArrow = GetArrowFromPool();
        if (currentArrow != null)
        {
            currentArrow.transform.SetParent(notch);
            currentArrow.transform.localPosition = Vector3.zero;
            currentArrow.transform.localRotation = Quaternion.identity;
            currentArrow.SetActive(true);

            ArrowLauncherScript launcher = currentArrow.GetComponent<ArrowLauncherScript>();
            if (launcher != null)
            {
                launcher.Initialize(pullInteractable);
            }

            ArrowImpact impactScript = currentArrow.GetComponent<ArrowImpact>();
            if (impactScript != null)
            {
                impactScript.SetSpawner(this);
            }
        }
        else
        {
            arrowNotched = false;
        }
    }

    public GameObject GetArrowFromPool()
    {
        foreach (GameObject arrow in arrowPool)
        {
            if (!arrow.activeInHierarchy)
            {
                arrow.GetComponent<ArrowLauncherScript>()?.ResetState();
                arrow.GetComponent<ArrowImpact>()?.ResetState();
                return arrow;
            }
        }
        
        GameObject newArrow = Instantiate(arrowPrefab, arrowPoolParent);
        arrowPool.Add(newArrow);
        newArrow.GetComponent<ArrowLauncherScript>()?.ResetState();
        newArrow.GetComponent<ArrowImpact>()?.ResetState();
        return newArrow;
    }

    public void ReturnArrowToPool(GameObject arrow)
    {
        if (arrow == null) return;
        ArrowImpact impactScript = arrow.GetComponent<ArrowImpact>();
        if (impactScript != null)
        {
            impactScript.ResetState();
        }
        
        ArrowLauncherScript launcherScript = arrow.GetComponent<ArrowLauncherScript>();
        if (launcherScript != null)
        {
            launcherScript.ResetState();
            if (launcherScript.IsSubscribed()) {
                launcherScript.DetachFromBow(pullInteractable);
            }
        }
        
        arrow.transform.SetParent(arrowPoolParent);
        arrow.SetActive(false);

        if (arrow == currentArrow)
        {
            currentArrow = null;
            arrowNotched = false;
        }
    }
}
