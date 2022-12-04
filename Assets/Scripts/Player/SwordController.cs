using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SwordController : MonoBehaviour
{
    public float energy = 100;
    public EnergyBar energyBar;
    public GameObject SwordBeam;
    public Transform SwordPos;
    public Vector3 lastPos = new Vector3(0, 0, 0);
    public Vector3 currentPos;
    public Vector3 posDif;

    public bool blueSwordActive = true;
    public bool redSwordActive = false;
    public bool yellowSwordActive = false;
    public bool noSwordActive = false;

    public MeshRenderer meshRenderer;

    public Material blueSwordMat;
    public Material redSwordMat;
    public Material yellowSwordMat;

    public Transform shootPoint;

    //Break

    //UI Stuff
    //Put image switching stuff here

    //Blue Sword Stuff
    public Rigidbody blueShieldPrefab;
    public float blueShieldSpeed = 1f;
    private float blueShieldCooldown = 0.25f;
    private bool canBlock = false;

    //Red Sword Stuff
    public Rigidbody redBulletPrefab;
    public float redBulletSpeed = 8f;
    private float redBulletCooldown = 0.25f;
    private bool canFire = false;

    //Yellow Sword Stuff
    public Transform linePoint;

    public LineRenderer shootLine;
    public LineRenderer tetherLine;
    private float laserWidth = 0.02f;

    public SpringJoint joint;
    public GameObject tetheredObject;
    public Tetherable tetherableScript;
    private int tetherLayerMask;
    private bool canTether = true;

    //No Sword Stuff

    //Break

    //Controller Stuff
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public InputDevice targetDevice;

    // Start is called before the first frame update
    void Start()
    {
        TryInitialize();
    }

    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if (prefab)
            {
                Debug.Log("Yeh");
            }
            else
            {
                Debug.LogError("Did not find corresponding controller model");
            }
        }

        //Tether Setup
        tetherLayerMask = LayerMask.GetMask("Tetherable");

        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        shootLine.SetPositions(initLaserPositions);
        shootLine.startWidth = laserWidth;
        shootLine.endWidth = laserWidth;
        tetherLine.SetPositions(initLaserPositions);
        tetherLine.startWidth = laserWidth;
        tetherLine.endWidth = laserWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetDevice.isValid)
        {
            TryInitialize();
        }

        SwordUI();

        SwordTracker();

        EnergyManager();

        SwordOrganizer();
    }

    //Position tracking
    public void SwordTracker()
    {
        currentPos = SwordPos.position;
        posDif = currentPos - lastPos;
        lastPos = SwordPos.position;
    }

    //Energy
    public void EnergyManager()
    {
        if (energy < 100)
        {
            energy += Time.deltaTime * 6f;
            if (energy > 100)
            {
                energy = 100;
            }
            if (energy < 0)
            {
                energy = 0;
            }
        }

        energyBar.SetEnergy(energy);
    }

    //Sword Selection
    public void SwordOrganizer()
    {
        if (blueSwordActive)
        {
            BlueSword();
        }
        if (redSwordActive)
        {
            RedSword();
        }
        if (yellowSwordActive)
        {
            YellowSword();
        }
        if (noSwordActive)
        {
            NoSword();
        }
    }

    //Break

    //UI Management
    public void SwordUI()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue <= 0.1f)
        {
            targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axisValue);
            if (axisValue.x > 0.75f && axisValue.y < 0.5f && axisValue.y > -0.5f && !blueSwordActive)
            {
                YellowSwitch();
                if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                {
                    targetDevice.SendHapticImpulse(0u, 0.25f, 0.2f);
                }
            }
            else if (axisValue.y > 0.75f && axisValue.x < 0.5f && axisValue.x > -0.5f && !redSwordActive)
            {
                RedSwitch();
                if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                {
                    targetDevice.SendHapticImpulse(0u, 0.25f, 0.2f);
                }
            }
            else if (axisValue.x < -0.75f && axisValue.y < 0.5f && axisValue.y > -0.5f && !yellowSwordActive)
            {
                BlueSwitch();
                if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                {
                    targetDevice.SendHapticImpulse(0u, 0.25f, 0.2f);
                }
            }
            else if (axisValue.y < -0.75f && axisValue.x < 0.5f && axisValue.x > -0.5f && !noSwordActive)
            {
                NoSwitch();
                if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                {
                    targetDevice.SendHapticImpulse(0u, 0.25f, 0.2f);
                }
            }
        }
    }

    //Break

    //Blue Sword
    public void BlueSword()
    {
        if (blueShieldCooldown > 0)
        {
            blueShieldCooldown -= Time.deltaTime;
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f && blueShieldCooldown <= 0 && canBlock && energy >= 25)
        {
            BlueBlock();
            blueShieldCooldown = 0.25f;
            canBlock = false;
            energy -= 25;

            if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
            {
                targetDevice.SendHapticImpulse(0u, 0.25f, 0.2f);
            }
        }
        if (triggerValue <= 0.1f && !canBlock)
        {
            canBlock = true;
        }
    }

    public void BlueBlock()
    {
        Rigidbody shieldClone = Instantiate(blueShieldPrefab, shootPoint.position, shootPoint.rotation);
        shieldClone.velocity = shootPoint.forward * blueShieldSpeed;
    }

    //Red Sword
    public void RedSword()
    {
        if (redBulletCooldown > 0)
        {
            redBulletCooldown -= Time.deltaTime;
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f && redBulletCooldown <= 0 && canFire && energy >= 25)
        {
            RedFire();
            redBulletCooldown = 0.25f;
            canFire = false;
            energy -= 25;

            if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
            {
                targetDevice.SendHapticImpulse(0u, 0.25f, 0.2f);
            }
        }
        if (triggerValue <= 0.1f && !canFire)
        {
            canFire = true;
        }
    }

    public void RedFire()
    {
        Rigidbody bulletClone = Instantiate(redBulletPrefab, shootPoint.position, shootPoint.rotation);
        bulletClone.velocity = shootPoint.forward * redBulletSpeed;
    }

    //Yellow Sword
    public void YellowSword()
    {
        if (tetherLine.enabled && tetheredObject == null)
        {
            joint = null;
            canTether = true;
            tetherableScript = null;
            shootLine.enabled = false;
            tetherLine.enabled = false;
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f && canTether && energy > 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(shootPoint.position, shootPoint.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, tetherLayerMask))
            {
                tetherableScript = hit.transform.gameObject.GetComponent<Tetherable>();

                shootLine.enabled = true;
                shootLine.SetPosition(0, shootPoint.position);
                shootLine.SetPosition(1, hit.point);

                if (!tetherableScript.attached)
                {
                    tetheredObject = hit.transform.gameObject;

                    Vector3 oldPos = tetheredObject.transform.position;
                    Vector3 shootPos = shootPoint.position;
                    tetheredObject.transform.position = shootPos;

                    joint = tetheredObject.AddComponent(typeof(SpringJoint)) as SpringJoint;
                    joint.damper = 1f;
                    joint.spring = 12.0f;
                    joint.connectedBody = shootPoint.GetComponent<Rigidbody>();
                    joint.connectedAnchor = new Vector3(0, 0, 0);
                    joint.anchor = new Vector3(0, 0, 0);

                    tetheredObject.transform.position = oldPos;

                    canTether = false;
                    tetherableScript.attached = true;
                }
            }
            else if (canTether)
            {
                shootLine.enabled = true;
                shootLine.SetPosition(0, shootPoint.position);
                shootLine.SetPosition(1, shootPoint.TransformDirection(Vector3.forward) * 1000);
            }
            else
            {
                shootLine.enabled = false;
            }
        }

        if (triggerValue <= 0.1f)
        {
            shootLine.enabled = false;
        }

        if (triggerValue > 0.1f && !canTether && tetheredObject != null)
        {
            energy -= Time.deltaTime * 9.5f;

            linePoint.LookAt(tetheredObject.transform);

            shootLine.enabled = false;
            tetherLine.enabled = true;
            tetherLine.SetPosition(0, linePoint.position);
            tetherLine.SetPosition(1, tetheredObject.transform.position);

            if (targetDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
            {
                targetDevice.SendHapticImpulse(0u, 0.15f, 0.1f);
            }
        }

        if (triggerValue > 0.1f && canTether)
        {
            tetherLine.enabled = false;
        }

        if ((triggerValue <= 0.1f && !canTether && tetheredObject != null) || (!canTether && tetheredObject != null && energy <= 0))
        {
            YellowDisconnect();
        }
    }

    public void YellowDisconnect()
    {
        if (tetheredObject != null)
        {
            Destroy(tetheredObject.GetComponent<SpringJoint>());
            tetheredObject = null;
        }
        if (joint != null)
        {
            joint = null;
        }
        if (!canTether)
        {
            canTether = true;
        }
        if (tetherableScript != null)
        {
            tetherableScript.attached = false;
        }

        shootLine.enabled = false;
        tetherLine.enabled = false;
    }

    //No Sword
    public void NoSword()
    {
        if (energy < 100)
        {
            energy += Time.deltaTime * 4f;
            if (energy > 100)
            {
                energy = 100;
            }
            if (energy < 0)
            {
                energy = 0;
            }
        }
    }

    //Break

    //Sword Changes/Switches
    public void BlueSwitch()
    {
        SwordBeam.SetActive(true);

        YellowDisconnect();

        meshRenderer.material = blueSwordMat;

        blueSwordActive = true;
        redSwordActive = false;
        yellowSwordActive = false;
        noSwordActive = false;
    }

    public void RedSwitch()
    {
        SwordBeam.SetActive(true);

        YellowDisconnect();

        meshRenderer.material = redSwordMat;

        blueSwordActive = false;
        redSwordActive = true;
        yellowSwordActive = false;
        noSwordActive = false;
    }

    public void YellowSwitch()
    {
        SwordBeam.SetActive(true);

        meshRenderer.material = yellowSwordMat;

        blueSwordActive = false;
        redSwordActive = false;
        yellowSwordActive = true;
        noSwordActive = false;
    }

    public void NoSwitch()
    {
        YellowDisconnect();

        SwordBeam.SetActive(false);

        blueSwordActive = false;
        redSwordActive = false;
        yellowSwordActive = false;
        noSwordActive = true;
    }
}
