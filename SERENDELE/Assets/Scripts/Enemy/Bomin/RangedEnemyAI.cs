using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform player;

    public GameObject projectilePrefab;
    public Transform firePoint;

    private float timeBetweenAttacks = 2f;
    private bool alreadyAttacked;

    private int projectileSpeed = 4;
    private int projectileDamage = 10;

    private float sightRange = 20f;
    private float attackRange = 10f;
    private float maintainDistance = 5f;

    private float lookSpeed = 5f; // 플레이어를 바라보는 속도

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= sightRange)
        {
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else if (distanceToPlayer <= attackRange && distanceToPlayer > maintainDistance)
            {
                AttackPlayer();
            }
            else if (distanceToPlayer <= maintainDistance)
            {
                MaintainDistance();
            }
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        // 플레이어를 부드럽게 바라보도록 회전
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * lookSpeed);

        if (!alreadyAttacked)
        {
            // 발사 로직 수정
            GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                Vector3 shootDirection = (player.position - firePoint.position).normalized;
                projectile.Launch(shootDirection);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void MaintainDistance()
    {
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        Vector3 newPosition = transform.position + directionAwayFromPlayer.normalized * maintainDistance;
        agent.SetDestination(newPosition);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maintainDistance);
    }*/
}
