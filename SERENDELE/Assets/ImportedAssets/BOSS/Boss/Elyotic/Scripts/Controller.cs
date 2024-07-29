using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    public float rotateSpeed = 3;
    public float speed = 6 ; 
    public CharacterController controller;
    public Animator animator;

    public Camera playerCamera;
    private Vector3 _cameraOffSet;

    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;
    

    //Metodo Start e executado uma unica vez, quando o script e executado.
    void Start()
    {
        _cameraOffSet = playerCamera.transform.position - transform.position;
    }

    //Medodo Update e executado a cada Frame
    void Update()
    {
        Rotacionar();
        Mover();

        Vector3 newPos = transform.position + _cameraOffSet;
        playerCamera.transform.position = Vector3.Slerp(playerCamera.transform.position, newPos, smoothFactor);
    }

    private void Mover()

    {
        Vector3  vetorvelocidade = transform.forward * Input.GetAxis ("Vertical") * speed;
        controller.SimpleMove(vetorvelocidade);
        float velocidadeDePersonagem = Mathf.Abs(controller.velocity.x) + Mathf.Abs(controller.velocity.z);
        animator.SetFloat("velocidade", velocidadeDePersonagem);

    }
private void Rotacionar()

    {

        transform.Rotate(0, Input.GetAxis ("Horizontal") * rotateSpeed, 0); 

        }
}