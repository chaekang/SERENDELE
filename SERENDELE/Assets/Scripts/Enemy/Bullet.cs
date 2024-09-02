using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, 3f);
        target = GameObject.Find("all");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //target.GetComponent<HpAndExp>().DecreaseHp(10);
            Destroy(gameObject);
        }
    }
}