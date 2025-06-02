using System;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowHit : MonoBehaviour
{
    
    public GameObject floorPrefab;
    public Transform floorSpawn;

    private void OnTriggerEnter(Collider arrow)
    {
        Instantiate(floorPrefab, floorSpawn.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
