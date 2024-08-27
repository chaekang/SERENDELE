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
            // 플레이어의 HP를 감소시키기 전에 플레이어 레벨에 따라 데미지를 설정
            HpAndExp playerHp = other.GetComponent<HpAndExp>();
            if (playerHp != null)
            {
                AdjustDamageBasedOnPlayerLevel(playerHp.curLevel);
                playerHp.DecreaseHp(damage); // damage 값만큼 HP 감소
                Debug.Log("Player hit: " + damage);
            }

            Destroy(gameObject); // 총알을 파괴
        }
    }

    void AdjustDamageBasedOnPlayerLevel(int playerLevel)
    {
        // 플레이어의 레벨에 따라 데미지를 조정하는 로직
        switch (playerLevel)
        {
            case 1:
                damage = 3; // 레벨 1일 때의 데미지
                break;
            case 2:
                damage = 6; // 레벨 2일 때의 데미지
                break;
            case 3:
                damage = 10; // 레벨 3일 때의 데미지
                break;
            default:
                damage = 10; // 기본 데미지
                break;
        }
    }
}
