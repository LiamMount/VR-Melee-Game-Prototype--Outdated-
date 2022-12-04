using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TestBullet : MonoBehaviour
{
    public GameObject self;
    public Rigidbody rb;

    private float lifeTimer = 6.0f;
    private float reflectTimer = 0f;

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
            Destroy(self);
        }

        if (reflectTimer > 0)
        {
            reflectTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Beam")
        {
            SwordController swordScript = col.transform.parent.transform.parent.GetComponent<SwordController>();

            if (reflectTimer <= 0)
            {
                //Vibrate
                InputDevice targetDevice = swordScript.targetDevice;
                if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                {
                    targetDevice.SendHapticImpulse(0u, 0.15f, 0.1f);
                }

                //Deflect self
                
                Vector3 BulletVelocity = rb.velocity;
                rb.velocity = BulletVelocity *= -1;

                reflectTimer = 0.2f;
            }
        }

        if (col.gameObject.tag == "Shield")
        {
            if (reflectTimer <= 0)
            {
                //Deflect self

                Vector3 BulletVelocity = rb.velocity;
                rb.velocity = BulletVelocity *= -1;

                reflectTimer = 0.2f;
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Beam")
        {
            SwordController swordScript = col.transform.parent.transform.parent.GetComponent<SwordController>();

            if (reflectTimer <= 0)
            {
                //Vibrate
                InputDevice targetDevice = swordScript.targetDevice;
                if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                {
                    targetDevice.SendHapticImpulse(0u, 0.15f, 0.1f);
                }

                //Deflect self

                Vector3 BulletVelocity = rb.velocity;
                rb.velocity = BulletVelocity *= -1;

                reflectTimer = 0.2f;
            }
        }

        if (col.gameObject.tag == "Shield")
        {
            if (reflectTimer <= 0)
            {
                //Deflect self

                Vector3 BulletVelocity = rb.velocity;
                rb.velocity = BulletVelocity *= -1;

                reflectTimer = 0.2f;
            }
        }
    }
}
