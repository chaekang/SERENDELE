using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class _Player : MonoBehaviourPun
{
    private float speed; // 스피드 저장 변수
    public float basicSpeed;
    public float runningSpeed;

    public float jumpForce;
    private bool isGround = true;

    public GameObject equip;
    public string equippedWeapon = "nothing";

    private Rigidbody rb;
    private Animator animator;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public MeshCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            UpdateCollider();
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            speed = basicSpeed;
        }
    }

    // 콜라이더 할당 (임의)
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
            AttackAnim();
        }
    
        
    }

    void Move()
    {
        // 달리기
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

        // 걷기
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
        // 변수 weapon: sword(검류), wand(지팡이류), shield(방패류)
        // 파라미터 weapon: 0(nothing), 1(sword), 2(wand), 3(shield)
        if (equip.transform.childCount > 0)
        {
            equippedWeapon = equip.transform.GetChild(0).tag;
        }
        else
        {
            equippedWeapon = "nothing";
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("isAttacking");
            switch (equippedWeapon)
            {
                case "Sword":
                    Debug.Log("sword attack");
                    animator.SetInteger("weapon", 1);
                    break;
                case "Wand":
                    Debug.Log("wand attack");
                    animator.SetInteger("weapon", 2);
                    break;
                case "Shield":
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
        // 인벤토리 켜질 시 다시 마우스 포인터가 나타나도록 해주는 코드
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        // 인벤토리가 켜져 있는 동안 마우스가 회전하지 않도록 해주는 코드
        //canLook = !toggle;
    }
}
