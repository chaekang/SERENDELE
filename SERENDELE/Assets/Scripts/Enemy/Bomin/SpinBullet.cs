using UnityEngine;

public class SpinBullet : MonoBehaviour
{
    public float speed = 0.5f; // 총알 속도
    public float rotationSpeed = 500f; // 총알 회전 속도
    public int damage = 10; // 총알 데미지
    public float lifeTime = 100f; // 총알 라이프타임 (초 단위)

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>(); 
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        // 총알의 라이프타임 설정
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 총알 회전
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HpAndExp playerHp = other.GetComponent<HpAndExp>();
            if (playerHp != null)
            {
                playerHp.DecreaseHp(damage); // 플레이어의 HP를 감소시킴
                Debug.Log("Player hit by bullet! Damage: " + damage);
            }

            // 총알 파괴
            Destroy(gameObject);
        }
    }
}
