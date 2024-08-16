using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class action1 : MonoBehaviour
{
    public int level;

    private GameObject self;
    private GameObject target;
    
    private Transform player;
    private float distance;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        self = gameObject;
        target = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);
        if (distance < 2)
        {
            self.GetComponentInChildren<shortWeapon>().Attack();
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            target.GetComponent<HpAndExp>().DecreaseHp(3);
        }
    }

}
