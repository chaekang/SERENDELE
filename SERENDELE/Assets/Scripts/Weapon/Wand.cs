using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Wand : MonoBehaviour
{
    public bool isHand = false; // 인벤토리랑 연동돼야함 (우선 true)

    // 무기 정보
    public float damage;
    public float speed;

    // 공격
    public bool isAttacking = false;
    public bool isReady = true; // 무기 쿨타임 지났는지
    public float coolTime = 0;

    // 투사체
    public GameObject projectile;    // 지팡이 투사체
    public float projectileDistance; // 투사체 거리
    public float projectileSpeed;    // 투사체 속도

    public Transform weaponPosition;
    private Collider col;
    private Rigidbody rb;

    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.isTrigger = false;
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHand)
        {
            UseWeapon();
            col.isTrigger = true;
            rb.useGravity = false;
            rb.isKinematic = true;
            weaponPosition = GetComponentInParent<Transform>();
            weaponPosition = weaponPosition.parent;
            transform.position = weaponPosition.position;
            transform.rotation = weaponPosition.rotation;
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
