using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviourPun
{
    public float sesitivity;
    public float rotationX;
    public float rotationY;

    public float maxRotation;
    public float minRotation;

    public Transform wizard;
    // Start is called before the first frame update
    void Start()
    {
        wizard = transform.parent;
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

            if (rotationX > maxRotation) rotationX = maxRotation;
            else if (rotationX < minRotation) rotationX = minRotation;

            wizard.eulerAngles = new Vector3(0, rotationY, 0);
            transform.eulerAngles = new Vector3(-rotationX, rotationY, 0);
        }
        
    }
}
