using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody bulletRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        /*
        if (other.CompareTag("Player"))
        {
            Debug.Log("바위 플레이어와 충돌");
            Destroy(gameObject);
        }
        */
    }
}
