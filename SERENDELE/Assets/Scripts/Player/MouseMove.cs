using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviourPun
{
    public float sesitivity;
    public float rotationX;
    public float rotationY; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            float mouseMoveX = Input.GetAxis("Mouse X");
            float mouseMoveY = Input.GetAxis("Mouse Y");

            rotationY += mouseMoveX * sesitivity * Time.deltaTime;
            rotationX += mouseMoveY * sesitivity * Time.deltaTime;

            if (rotationX > 5f) rotationX = 5f;
            else if (rotationX < -5f) rotationX = -5f;


            transform.eulerAngles = new Vector3(-rotationX, rotationY, 0);
        }
        
    }
}
