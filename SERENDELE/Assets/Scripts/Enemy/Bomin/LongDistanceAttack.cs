using UnityEngine;
using UnityEngine.AI;

public class LongDistanceAttack : MonoBehaviour
{
    public float stoppingDistance = 6f; // 플레이어와 거리
    public float fireRate = 2f; // 총알 발사 주기
    public GameObject bulletPrefab;
    public Transform firePoint;
    private Transform player;
    private NavMeshAgent agent;
    private float nextFireTime = 0f;
    public float rotationSpeed = 5f; // 회전 속도
    public int bulletDamage = 10; // 총알의 대미지

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // 플레이어를 일정 거리를 유지하면서 추적하게
        if (distance > stoppingDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(transform.position); // 일정 거리에 도달하면 정지
        }

        // 플레이어를 향해 부드럽게 회전
        RotateTowardsPlayer();

        // 일정 주기마다 총알 발사하게
        if (Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized; // 방향 계산하는 부분
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // 회전 값
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed); // 부드럽게 회전
    }

    void Shoot()
    {
        // 총알 생성과 발사
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 총알의 SpinBullet 스크립트에 대미지 설정
        SpinBullet bulletScript = bullet.GetComponent<SpinBullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = bulletDamage;
        }
    }
}
