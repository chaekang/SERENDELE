using Cinemachine;
using Photon.Pun;
using UnityEngine;
using Cinemachine;

public class _Player : MonoBehaviourPun
{
    public float basicSpeed;
    public float runningSpeed;
    public float jumpForce;

    private bool isGround = true;

    public string weapon = "nothing";
    private float speed;

    private Rigidbody rb;
    private Animator animator;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public MeshCollider collider;
    public CinemachineVirtualCamera followCam;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            UpdateCollider();
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            speed = basicSpeed;

            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
            followCam.Follow = transform;
            followCam.LookAt = transform;

        }
    }

    // �ݶ��̴� �Ҵ� (����)
    public void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh(); 
        skinnedMeshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;   
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Move();
            Jump();
        }
    
        
    }

    void Move()
    {
        // �޸���
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("isRunning", true);
            speed = runningSpeed;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = basicSpeed;
            animator.SetBool("isRunning", false);
        }

        // �ȱ�
        if (Input.GetKey(KeyCode.W))
        {
            //Debug.Log("isMoving");
            animator.SetBool("isMoving", true);
            if (Input.GetKey(KeyCode.A))
                transform.Translate(-speed * Time.deltaTime, 0, speed * Time.deltaTime);
            else if(Input.GetKey(KeyCode.D))
                transform.Translate(speed * Time.deltaTime, 0, speed * Time.deltaTime);
            else
                transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //Debug.Log("isMoving");
            animator.SetBool("isMoving", true);
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //Debug.Log("isMoving");
            animator.SetBool("isMoving", true);
            transform.Translate(0, 0, -speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //Debug.Log("isMoving");
            animator.SetBool("isMoving", true);
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        else
        {
            //Debug.Log("Idle");
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
        }
    }

    void Jump()
    {
        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            isGround = false;
            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(jumpForce * -Physics.gravity.y);
            rb.AddForce(jumpVelocity, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") isGround = true;

        if (collision.gameObject.tag == "Enemy")
        {
            GameManager.Instance.HpAndExp.DecreaseHp(20);
        }
    }

    void AttackAnim()
    {
        // ���� weapon: sword(�˷�), wand(�����̷�), shield(���з�)
        // �Ķ���� weapon: 0(nothing), 1(sword), 2(wand), 3(shield)
        if (Input.GetMouseButtonDown(0))
        {
            //animator.SetTrigger("isAttacking");
            Debug.Log("attack");
            switch (weapon)
            {
                case "sword":
                    Debug.Log("sword attack");
                    animator.SetInteger("weapon", 1);
                    break;
                case "wand":
                    Debug.Log("wand attack");
                    animator.SetInteger("weapon", 2);
                    break;
                case "shield":
                    Debug.Log("shield attack");
                    animator.SetInteger("weapon", 3);
                    break;
                case "nothing":
                    Debug.Log("no weapon");
                    animator.SetInteger("weapon", 0);
                    break;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("bow");
        }

    }

    public void ToggleCursor(bool toggle)
    {
        // �κ��丮 ���� �� �ٽ� ���콺 �����Ͱ� ��Ÿ������ ���ִ� �ڵ�
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        // �κ��丮�� ���� �ִ� ���� ���콺�� ȸ������ �ʵ��� ���ִ� �ڵ�
        //canLook = !toggle;
    }
}