using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRubble : MonoBehaviour
{
    public GameObject self;
    private float lifeSpan = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeSpan -= Time.deltaTime;
        if (lifeSpan <= 0)
        {
            Destroy(self);
        }
    }
}
