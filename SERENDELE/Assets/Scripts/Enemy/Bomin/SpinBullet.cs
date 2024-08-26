using UnityEngine;

public class SpinBullet : MonoBehaviour
{
    public float speed = 10f; // 총알 속도
    public float rotationSpeed = 1000f; // 총알 회전 속도
    public int damage = 10; // 총알 데미지
    public float lifeTime = 3f; // 총알 라이프타임 (초 단위)
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 direction = (player.position - transform.position).normalized;
        GetComponent<Rigidbody>().velocity = direction * speed;

        // 총알의 라이프타임 설정
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 총알 회전
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Detected with: " + other.name);
        if (other.CompareTag("Player"))
        {
            // 플레이어의 HP를 감소시키기
            HpAndExp playerHp = other.GetComponent<HpAndExp>();
            if (playerHp != null)
            {
                playerHp.DecreaseHp(damage); // damage 값만큼 HP 감소
                Debug.Log("Player hit: " + damage);
            }

            Destroy(gameObject); // 총알을 파괴
        }
    }
}
