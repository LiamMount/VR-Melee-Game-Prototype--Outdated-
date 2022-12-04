using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public Rigidbody bulletPrefab;
    public Transform spawnLocation;
    public float bulletSpeed = 5.0f;

    private float bulletTimer = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bulletTimer -= Time.deltaTime;

        if (bulletTimer <= 0)
        {
            bulletTimer = 1.5f;
            Fire();
        }
    }

    public void Fire()
    {
        Rigidbody bulletClone = Instantiate(bulletPrefab, spawnLocation.position, spawnLocation.rotation);
        bulletClone.velocity = spawnLocation.forward * bulletSpeed;
    }
}
