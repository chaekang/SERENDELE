using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Wand : MonoBehaviour
{
    public bool isHand = false; 

    // ���� ����
    public float damage;
    public float speed;

    // ����
    public bool isAttacking = false;
    public bool isReady = true; // ���� ��Ÿ�� ��������
    public float coolTime = 0;

    // ����ü
    public GameObject projectile;    // ������ ����ü
    public float projectileDistance; // ����ü �Ÿ�
    public float projectileSpeed;    // ����ü �ӵ�

    private Transform weaponPosition;
    private Collider col;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.isTrigger = false;
        rb.useGravity = true;
        rb.isKinematic = false;

        StartCoroutine(CheckHandStatus());
    }
    private void Update()
    {
        if (isHand)
        {
            UseWeapon();
        }
    }
    // Update is called once per frame
    IEnumerator CheckHandStatus()
    {
        while (true)
        {
            if (isHand)
            {
                col.isTrigger = true;
                rb.useGravity = false;
                rb.isKinematic = true;
                weaponPosition = GetComponentInParent<Transform>();
                weaponPosition = weaponPosition.parent;
                transform.localScale = Vector3.one;
                transform.position = weaponPosition.position;
                transform.rotation = weaponPosition.rotation;

            }
            else
            {
                col.isTrigger = false;
                rb.useGravity = true;
                rb.isKinematic = false;


            }
            yield return new WaitForSeconds(0.1f);
        }
        
    }
    void UseWeapon()
    {
        if (isReady && Input.GetMouseButtonDown(0))
        {
            isAttacking = true;
            SpawnProjectile();
            StartCoroutine(AttackCoolTime(speed));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isAttacking = false;
        }
    }

    IEnumerator AttackCoolTime(float speed)
    {
        isReady = false;
        coolTime += Time.deltaTime;
        yield return new WaitForSeconds(speed);
        isReady = true;
        coolTime = 0;
    }
    void SpawnProjectile()
    {
        Instantiate(projectile, weaponPosition.transform.position, weaponPosition.transform.rotation);
    }

}
