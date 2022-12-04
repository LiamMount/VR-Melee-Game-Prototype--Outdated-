using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TestKnockback : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider col)
    {
        TriggerChecker(col);
    }

    private void OnTriggerStay(Collider col)
    {
        TriggerChecker(col);
    }

    public void TriggerChecker(Collider col)
    {
        if (col.gameObject.tag == "Beam")
        {
            SwordController swordScript = col.transform.parent.transform.parent.GetComponent<SwordController>();

            //Vibrate
            InputDevice targetDevice = swordScript.targetDevice;
            if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
            {
                targetDevice.SendHapticImpulse(0u, 0.15f, 0.1f);
            }

            moveDirection = swordScript.posDif;
            rb.AddForce(moveDirection.normalized * ((swordScript.posDif.magnitude + 0.1f) * 10), ForceMode.Impulse);
        }

        if (col.gameObject.tag == "Shield")
        {
            moveDirection = col.transform.forward;
            rb.AddForce(moveDirection.normalized * 4, ForceMode.Impulse);
        }

        if (col.gameObject.tag == "Player Bullet" || col.gameObject.tag == "Enemy Bullet")
        {
            moveDirection = col.gameObject.transform.forward;
            rb.AddForce(moveDirection.normalized * 0.25f, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        CollisionChecker(col);
    }

    private void OnCollisionStay(Collision col)
    {
        CollisionChecker(col);
    }

    public void CollisionChecker(Collision col)
    {
        if (col.gameObject.tag == "Beam")
        {
            SwordController swordScript = col.transform.parent.transform.parent.GetComponent<SwordController>();

            //Vibrate
            InputDevice targetDevice = swordScript.targetDevice;
            if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
            {
                targetDevice.SendHapticImpulse(0u, 0.15f, 0.1f);
            }

            moveDirection = swordScript.posDif;
            rb.AddForce(moveDirection.normalized * ((swordScript.posDif.magnitude + 0.1f) * 10), ForceMode.Impulse);
        }

        if (col.gameObject.tag == "Shield")
        {
            moveDirection = col.transform.forward;
            rb.AddForce(moveDirection.normalized * 4, ForceMode.Impulse);
        }

        if (col.gameObject.tag == "Player Bullet" || col.gameObject.tag == "Enemy Bullet")
        {
            moveDirection = col.gameObject.transform.forward;
            rb.AddForce(moveDirection.normalized * 0.25f, ForceMode.Impulse);
        }
    }
}
