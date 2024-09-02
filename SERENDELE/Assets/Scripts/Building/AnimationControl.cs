using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    private Animator Animator;
    private float distance;
    private Transform player;
    private float speed;

    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        speed = rigidbody.velocity.magnitude;
        Animator.speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);
        MoveControl();
        ActionControl();
    }

    void MoveControl()
    {
        if (speed >= 0.01)
        {
            if(speed < 2)
            {
                Animator.SetFloat("Walk", speed);
            }
            Animator.SetFloat("Run", speed);
        } else if(speed < 0.01)
        {
            Animator.SetFloat("Stand", 0);
        }
    }

    void ActionControl()
    {
        if (distance<0.8)
        {
            Animator.SetTrigger("Attack");
        }
    }

}