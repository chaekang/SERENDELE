using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;  // 에너미의 NavMeshAgent
    public Transform player;  // 플레이어의 위치
    public LayerMask whatIsPlayer;  // 플레이어가 속한 레이어를 식별하기 위해 사용

    // 공격 관련 변수
    public GameObject projectilePrefab;  // 발사할 구체 프리팹
    public Transform firePoint;  // 구체를 발사할 위치
    public float timeBetweenAttacks = 2f;  // 공격 간의 대기 시간
    private bool alreadyAttacked;  // 공격이 이미 수행되었는지 여부
    public float projectileSpeed = 20f;  // 구체의 속도
    public int projectileDamage = 10;  // 구체의 데미지

    // 거리와 탐지 범위 관련 변수
    public float sightRange = 20f;  // 에너미의 시야 범위
    public float attackRange = 15f;  // 에너미의 공격 범위
    public float maintainDistance = 10f;  // 플레이어와 유지할 거리

    private void Awake()
    {
        // 에너미의 NavMeshAgent 컴포넌트를 가져옵니다.
        agent = GetComponent<NavMeshAgent>();
        // 씬에서 플레이어를 찾아 연결합니다.
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // 플레이어와의 거리를 계산합니다.
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어가 시야 범위 내에 있을 때의 행동
        if (distanceToPlayer <= sightRange)
        {
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();  // 플레이어를 추적합니다.
            }
            else if (distanceToPlayer <= attackRange && distanceToPlayer > maintainDistance)
            {
                AttackPlayer();  // 플레이어를 공격합니다.
            }
            else if (distanceToPlayer <= maintainDistance)
            {
                MaintainDistance();  // 일정 거리를 유지하며 후퇴합니다.
            }
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);  // 플레이어의 위치로 이동합니다.
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);  // 공격할 때는 이동하지 않도록 멈춥니다.

        transform.LookAt(player);  // 플레이어를 향해 바라봅니다.

        if (!alreadyAttacked)
        {
            // 구체 발사
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = (player.position - firePoint.position).normalized * projectileSpeed;

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);  // 일정 시간 후 공격을 재설정합니다.
        }
    }

    private void MaintainDistance()
    {
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        Vector3 newPosition = transform.position + directionAwayFromPlayer.normalized * maintainDistance;
        agent.SetDestination(newPosition);  // 플레이어로부터 일정 거리를 유지하며 후퇴합니다.
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;  // 공격 가능 상태로 재설정합니다.
    }

    // 디버깅용 시야 및 공격 범위 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maintainDistance);
    }
}
