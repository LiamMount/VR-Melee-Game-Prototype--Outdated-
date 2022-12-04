using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHorde : MonoBehaviour
{
    public Rigidbody basic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < 5)
        {
            Rigidbody basicEnemy = Instantiate(basic, transform.position, transform.rotation);
        }
    }
}
