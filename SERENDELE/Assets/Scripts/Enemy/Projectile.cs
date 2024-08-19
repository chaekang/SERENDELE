using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform firePoint;  // 총알이 발사될 위치
    private float lifetime = 2f;  // 총알 라이프타임
    private float ProjectileSpeed = 500000f;  // 총알 속도 
    private int ProjectileDamage = 10;  // 총알 데미지

    private Rigidbody rb;

    private void Start()
    {
        // Rigidbody 
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody is missing from the Projectile.");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not assigned.");
            return;
        }

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }

        // 총알이 firePoint에서 발사
        transform.position = firePoint.position;
        transform.rotation = firePoint.rotation;

        // 총알이 플레이어를 향해 일직선으로 발사되도록 방향 설정
        Vector3 direction = (player.position - firePoint.position).normalized;  // 방향 수정
        rb.velocity = direction * ProjectileSpeed;  // 속도 설정

        // 일정 시간이 지나면 총알 제거
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 시, 총알 제거
        Destroy(gameObject);
    }
}
