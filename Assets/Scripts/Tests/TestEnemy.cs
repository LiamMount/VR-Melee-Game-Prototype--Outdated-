using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TestEnemy : MonoBehaviour
{
    //General
    public GameObject self;
    public Rigidbody rb;
    public Tetherable tetherableScript;

    //Being an Enemy
    public Rigidbody bulletPrefab;
    public float bulletSpeed = 7;
    public Transform shootPoint;
    private Transform target;
    private Vector3 stagePos;
    private Transform stageCenter;
    private float stageTimer;

    private float stunTimer = 0f;
    private float shootTimer;

    //VFX
    public GameObject shootFX;

    //Getting Destroyed
    public Rigidbody rubblePrefab;
    private bool breaking = false;
    private Vector3 moveDirection;

    //Break

    // Start is called before the first frame update
    void Start()
    {
        FindTarget();
        Boot();
        LocationDecide();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            FindTarget();
        }

        Reboot();
        if (stunTimer <= 0)
        {
            Aim();
            ShootCheck();
            StageSwitch();
            StageMove();
        }
    }

    //Technical
    public void Boot()
    {
        stageCenter = GameObject.FindGameObjectWithTag("Center").transform;
        stageTimer = Random.Range(9f, 11f);
        shootTimer = Random.Range(5f, 7f);
    }

    public void FindTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Reboot()
    {
        if (!tetherableScript.attached && stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                if (rb.useGravity)
                {
                    rb.useGravity = false;
                }
            }
            else if (stunTimer > 0)
            {
                rb.useGravity = true;
            }
        }
        if (tetherableScript.attached)
        {
            stunTimer = 1f;
            shootTimer = Random.Range(5f, 7f);
            shootFX.SetActive(false);
        }
    }   

    //Gun
    public void Aim()
    {
        Vector3 newDirection = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(newDirection);
        Quaternion nextRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        transform.rotation = nextRotation;
    }

    public void ShootCheck()
    {
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer > 0 && shootTimer < 2)
            {
                shootFX.SetActive(true);
            }
            if (shootTimer <= 0)
            {
                Fire();
                shootFX.SetActive(false);
                shootTimer = Random.Range(5f, 7f);
            }
        }
    }

    public void Fire()
    {
        Rigidbody bulletClone = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        bulletClone.velocity = shootPoint.forward * bulletSpeed;
    }

    //Movement
    public void StageSwitch()
    {
        if (stageTimer > 0)
        {
            stageTimer -= Time.deltaTime;
            if (stageTimer <= 0)
            {
                stageTimer = Random.Range(9f, 11f);
                LocationDecide();
            }
        }
    }

    public void LocationDecide()
    {
        stagePos = new Vector3(stageCenter.position.x + Random.Range(-7f, 7f), stageCenter.position.y + Random.Range(-3f, 3f), stageCenter.position.z + Random.Range(-3f, 3f));
    }

    public void StageMove()
    {
        moveDirection = stagePos - transform.position;
        float magnitude = Vector3.Distance(transform.position, stagePos) / 2;
        rb.AddForce(moveDirection.normalized * magnitude, ForceMode.Force);
    }

    //Collision
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
                targetDevice.SendHapticImpulse(0u, 0.35f, 0.1f);
            }

            if (!breaking)
            {
                breaking = true;
                moveDirection = swordScript.posDif;
                float magnitude = (swordScript.posDif.magnitude + 0.25f) * 30;

                SummonRubble(moveDirection, magnitude);

                Destroy(self);
            }
        }

        if (col.gameObject.tag == "Shield")
        {
            moveDirection = col.gameObject.GetComponent<Rigidbody>().velocity.normalized;
            rb.AddForce(moveDirection.normalized * 4, ForceMode.Impulse);

            stunTimer = 1f;
        }

        if (col.gameObject.tag == "Player Bullet" || col.gameObject.tag == "Enemy Bullet")
        {
            if (!breaking)
            {
                breaking = true;
                moveDirection = col.gameObject.GetComponent<Rigidbody>().velocity.normalized;
                float magnitude = col.gameObject.GetComponent<Rigidbody>().velocity.magnitude * 2;

                SummonRubble(moveDirection, magnitude);

                Destroy(self);
            }
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
                targetDevice.SendHapticImpulse(0u, 0.35f, 0.1f);
            }

            if (!breaking)
            {
                breaking = true;
                moveDirection = swordScript.posDif;
                float magnitude = (swordScript.posDif.magnitude + 0.25f) * 30;

                SummonRubble(moveDirection, magnitude);

                Destroy(self);
            }
        }

        if (col.gameObject.tag == "Shield")
        {
            moveDirection = col.gameObject.GetComponent<Rigidbody>().velocity.normalized;
            rb.AddForce(moveDirection.normalized * 4, ForceMode.Impulse);

            stunTimer = 1f;
        }

        if (col.gameObject.tag == "Player Bullet" || col.gameObject.tag == "Enemy Bullet")
        {
            if (!breaking)
            {
                breaking = true;
                moveDirection = col.gameObject.GetComponent<Rigidbody>().velocity.normalized;
                float magnitude = col.gameObject.GetComponent<Rigidbody>().velocity.magnitude * 2;

                SummonRubble(moveDirection, magnitude);

                Destroy(self);
            }
        }
    }

    //Death
    public void SummonRubble(Vector3 moveDirection, float magnitude)
    {
        Rigidbody rubble = Instantiate(rubblePrefab, transform.position, transform.rotation);
        rubble.AddForce(moveDirection.normalized * magnitude, ForceMode.Impulse);
    }
}
