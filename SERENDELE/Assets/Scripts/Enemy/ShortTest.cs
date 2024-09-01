using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class ShortTest : MonoBehaviour
{

    [SerializeField]
    private Slider healthBar;

    public float enemyHealth;
    public float enemyCurHealth;  //적의 현재 체력
    public float getDamage; // 적이 플레이어로부터 받는 공격 데미지
    private Transform camera;

    private Transform player;
    private NavMeshAgent navAgent;

    public int level;
    int damage;   //대미지
    float Aspeed;   //공격 속도
    float distance;


    void Start()
    {
        setLevel();
        GetEnemyInform();
        HealthBarSet();
    }

    void Update()
    {
        Move();
        EnemyDamaged();
        checkDistance();
    }

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void GetEnemyInform()
    {
        camera = Camera.main.transform;
        GameObject enemyController = GameObject.Find("EnemyController");
        enemyHealth = enemyController.GetComponent<EnemyInformation>().enemyShortHealth;
        enemyCurHealth = enemyHealth;
        getDamage = 5f;   //피격대미지
    }

    public void Move()
    {
        navAgent.SetDestination(player.position);
        healthBar.transform.LookAt(healthBar.transform.position + camera.rotation * Vector3.forward, camera.rotation * Vector3.up);
    }

    public void setLevel()
    {
        switch (level)
        {
            case 1: damage = 3; Aspeed = 0.5f; break;
            case 2: damage = 6; Aspeed = 1f; break;
            case 3: damage = 10; Aspeed = 2f; break;
        }
    }
    private void checkDistance()
    {
        distance = Vector3.Distance(transform.position, player.position);
        if (distance < 2)
        {
            StopCoroutine("Attack");
            StartCoroutine("Attack");
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("all").GetComponent<HpAndExp>().DecreaseHp(damage);
        yield return new WaitForSeconds(Aspeed);
    }

    private void OnCollisionEnter(Collision collision)  //일반 접촉대미지
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            StartCoroutine("TouchDamage");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            StopCoroutine("TouchDamage");
        }
    }
    IEnumerator TouchDamage()
    {
        GameObject.Find("all").GetComponent<HpAndExp>().DecreaseHp(3);
        yield return new WaitForSeconds(0.4f);
    }


    private void HealthBarSet()
    {
        if (healthBar != null)
            healthBar.value = enemyCurHealth / enemyHealth;

    }
    private void EnemyDamaged()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            enemyCurHealth -= getDamage;
            Debug.Log("피해입음" + getDamage);
            Debug.Log("적 체력 : " + enemyCurHealth);
            HealthBarSet();
            if (enemyCurHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
