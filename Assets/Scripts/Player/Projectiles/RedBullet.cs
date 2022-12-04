using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBullet : MonoBehaviour
{
    public GameObject self;
    public GameObject hitbox;
    public Rigidbody rb;

    private float lifeTimer = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0)
        {
            //Animation/effect or something IDK
            Destroy(self);
        }

        hitbox.transform.localScale += new Vector3(Time.deltaTime / 2, 0, Time.deltaTime / 2);
    }

    private void OnTriggerStay(Collider col)
    {
        //Add enemy damaging code here
    }
}
