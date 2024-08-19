using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun.Demo.Asteroids;
using System;

public class Boss : MonoBehaviour
{
    // Boss Information
    [SerializeField]
    private Slider healthBar;

    public float bossHealth;
    public float bossCurHealth;
    public float damage; // Test damage to see if enemy explode when its health became 0 
    public float UpdateTargetDistance = 5f;
    private Vector3 bossPosition;

    public Animator animator;
    string[] animTriggers;

    // Attack 
    [SerializeField]
    public GameObject bulletPrefab;
    public GameObject enemyDistance;
    public GameObject enemyExplode;

    public float spawnRateMin = 0.5f;
    public float spawnRateMax = 3f;
    public float spawnNumEnemyDistance = 3f;
    public float spawnNumEnemyExplode = 3f;

    // Sector Attack 
    private float angleRange = 30.0f;
    // private float 

    private Transform target;
    private float spawnRate;
    private float timeAfterSpawn;
    private Vector3 spawnPosition;
    private Vector3 attackPosition;
    private Vector3 attackDistance;

    // Projector Inform 
    private Projector attackRangeSector = GameObject.Find("AttackRangeSector").GetComponentInChildren<Projector>();
    private float sectorAttackDuration = 3f;
    private float runningTime = 3f;


    // Start is called before the first frame update
    void Start()
    {
        GetBossInform();
        HealthBarSet();
        BulletSetting();

        AttackPattern();
    }

    // Update is called once per frame
    void Update()
    {
        BossDamaged();
        UpdateTargetArea();
        transform.LookAt(new Vector3(target.transform.position.x, 0, target.transform.position.z));
    }

    private void GetBossInform()
    {
        GameObject enemyController = GameObject.Find("EnemyController");
        bossHealth = enemyController.GetComponent<EnemyInformation>().enemyDistanceHealth;
        bossCurHealth = bossHealth;
        damage = 5f;
        spawnRate = 3f;

        target = GameObject.FindWithTag("Player").transform;
    }

    private void AttackPattern()
    {
        SpawnEnemy();
    }

    private void BossDamaged()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            bossCurHealth -= damage;
            Debug.Log("피해입음" + damage);
            Debug.Log("적 체력 : " + bossCurHealth);
            HealthBarSet();
            if (bossCurHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void HealthBarSet()
    {
        if (healthBar != null)
            healthBar.value = bossCurHealth / bossHealth;
    }

    private void BulletSetting()
    {
        timeAfterSpawn = 0f;
    }

    private void AttackDistance()
    {
        spawnPosition = target.position + new Vector3(0, 3f, 0);
        timeAfterSpawn += Time.deltaTime;

        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, target.rotation);
            bullet.transform.LookAt(target);
        }
    }

    private void UpdateTargetArea()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, UpdateTargetDistance);
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Player"))
                {
                    target = cols[i].transform;
                    AttackDistance();
                }
            }
        }
        else
        {
            target = null;
        }
    }

    private void SpawnEnemy()
    {
        bossPosition = this.gameObject.transform.position;
        animator.SetTrigger("Spawn");
        // animator.GetCurrentAnimatorStateInfo(0).IsName("Smash") &&
        Debug.Log("스폰 공격");
        // Spawn enemyDistance
        for (int i = 0; i < spawnNumEnemyDistance; i++)
        {
            GameObject enemy = Instantiate(enemyDistance, bossPosition, target.rotation);
            enemy.transform.LookAt(target);
        }

        // Spawn enemyExplode
        for (int i = 0; i < spawnNumEnemyExplode; i++)
        {
            GameObject enemy = Instantiate(enemyExplode, bossPosition, target.rotation);
            enemy.transform.LookAt(target);
        }
    }

    private void Smash()
    {
        attackPosition = target.position;
        // animator.SetTrigger("Smash");
    }

    /*
    private IEnumerator SectorAttack()
    {
        attackRangeSector.gameObject.SetActive(true);
        attackRangeSector.orthographicSize = 1.0f;

        while (runningTime > .0f)
        {
            runningTime -= Time.deltaTime;
            attackRangeSector.orthographicSize += 3.2f * Time.deltaTime;
            yield return null;
        }

        float dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange * .5f));
        Vector3 direction = target.position - transform.position;
        if (direction.magnitude < )
    }
    */
}
