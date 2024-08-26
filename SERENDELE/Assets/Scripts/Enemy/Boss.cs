using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun.Demo.Asteroids;
using System;
using JetBrains.Annotations;

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
    private bool isAnimating;

    // Attack Basic Informaiton 
    private Transform target;
    private Vector3 attackPosition;
    private float attackDistance;
    private float attackDamage;
    private float attackForce = 3.0f;
    private float skillWaitTime = 5.0f;
    private float animationWaitTime;

    // Distance Attack 
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

    private float spawnRate;
    private float timeAfterSpawn;
    private Vector3 spawnPosition;

    // Projector 
    private GameObject enemyController;
    private Projector attackRangeSector;
    private Projector attackRangeLine;
    private Projector attackRangeCircle;

    // Projector Inform 
    private float sectorAttackDuration = 3f;
    private float runningTime = 3f;


    // Start is called before the first frame update
    void Start()
    {
        GetBossInform();
        GetAttackRange();
        HealthBarSet();
        BulletSetting();

        StartCoroutine(AttackPattern());
    }

    // Update is called once per frame
    void Update()
    {
        BossDamaged();
        UpdateTargetArea();
        CheckAnimating();
    }

    private void GetBossInform()
    {
        GameObject enemyController = GameObject.Find("EnemyController");
        bossHealth = enemyController.GetComponent<EnemyInformation>().enemyDistanceHealth;
        bossCurHealth = bossHealth;
        damage = 5f;
        spawnRate = 3f;
        isAnimating = false;

        target = GameObject.FindWithTag("Player").transform;
    }
    
    private void GetAttackRange()
    {
        enemyController = GameObject.Find("EnemyController");
        attackRangeSector = enemyController.transform.Find("AttackRangeSector").GetComponent<Projector>();
        attackRangeLine = enemyController.transform.Find("AttackRangeLine").GetComponentInChildren<Projector>();
        attackRangeCircle = enemyController.transform.Find("AttackRangeCircle").GetComponentInChildren<Projector>();
    }

    private void CheckAnimating()
    {
        if (isAnimating)
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }
        else
        {
            Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            transform.LookAt(targetPosition);
        }
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



    /* Boss Attack Function */ 
    
    
    private void PlayerAttack()
    {
        GameObject a = GameObject.Find("a");
        if (a != null)
        {
            HpAndExp hpAndExp = a.GetComponent<HpAndExp>();
            if (hpAndExp != null)
            {
                hpAndExp.DecreaseHp(attackDamage);
                Rigidbody targetRigidbody = a.GetComponent<Rigidbody>();
                targetRigidbody.AddForce(new Vector3(0f, attackForce, 0f), ForceMode.Impulse);
            }
        }
    }

    private IEnumerator AttackPattern()
    {
        string[] attackName = { "Smash", "SpawnEnemy", "AttackDistance", "Swing" };
        while (bossCurHealth > 0)
        {
            int attackChoiceIndex = UnityEngine.Random.Range(0, attackName.Length);
            string selectedAttack = attackName[attackChoiceIndex];
            Debug.Log("패턴 고름"+ selectedAttack);
            switch (selectedAttack)
            {
                case "Smash":
                    yield return StartCoroutine(Smash());
                    break;
                case "SpawnEnemy":
                    yield return StartCoroutine(SpawnEnemy());
                    break;
                case "AttackDistance":
                    yield return StartCoroutine(AttackDistance());
                    break;
                case "Swing":
                    yield return StartCoroutine(Swing());
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(skillWaitTime);
        }
    }
    private IEnumerator AttackDistance()
    {
        /*
        spawnPosition = target.position + new Vector3(0, 3f, 0);
        timeAfterSpawn += Time.deltaTime;

        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, target.rotation);
            bullet.transform.LookAt(target);
        }
        */
        spawnPosition = target.position + new Vector3(0, 3f, 0);
        timeAfterSpawn = 0f;

        while (timeAfterSpawn < spawnRate)
        {
            timeAfterSpawn += Time.deltaTime;
            yield return null;
        }

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, target.rotation);
        bullet.transform.LookAt(target);
        Debug.Log("원거리 공격");
    }

    private IEnumerator SpawnEnemy()
    {
        bossPosition = this.gameObject.transform.position;
        isAnimating = true;
        animator.SetTrigger("Spawn");
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
        isAnimating = false;

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator Swing()
    {
        attackRangeSector.gameObject.SetActive(true);
        attackRangeSector.orthographicSize = 1.0f;
        float dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange * .5f));
        Vector3 direction = target.position - transform.position;

        attackDistance = 4.0f;
        float swingDamage = 3.0f;
        animationWaitTime = 44f / 60f;

        while (runningTime > .0f)
        {
            runningTime -= Time.deltaTime;
            attackRangeSector.orthographicSize += 3.2f * Time.deltaTime;
            yield return null;
        }

        if (direction.magnitude < attackDistance)
        {
            if (Vector3.Dot(direction.normalized, transform.forward) > dotValue)
            {
                isAnimating = true;
                animator.SetTrigger("Swing");
                yield return new WaitForSeconds(animationWaitTime);
                // PlayerAttack(smashDamage);

                Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
                targetRigidbody.AddForce(new Vector3(0f, attackForce, 0f), ForceMode.Impulse);
            }
        }
        isAnimating = false;
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator Smash()
    {
        attackRangeLine.gameObject.SetActive(true);
        attackRangeLine.orthographicSize = 0.01f;
        attackRangeLine.aspectRatio = 20.0f;

        float smashDamage = 5.0f;
        animationWaitTime = 110f / 60f;

        while (runningTime > .0f)
        {
            runningTime -= Time.deltaTime;
            attackRangeSector.orthographicSize += 3.2f * Time.deltaTime;

            yield return null;
        }

        isAnimating = true;
        animator.SetTrigger("Smash");
        yield return new WaitForSeconds(animationWaitTime);
        ShockPlayer(attackRangeSector, smashDamage);
        
        // Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        // targetRigidbody.AddForce(new Vector3(0f, attackForce, 0f), ForceMode.Impulse);

        isAnimating = false;
        yield return new WaitForSeconds(1f);
    }

    private void ShockPlayer(Projector projector, float damage)
    {
        float halfHeight = projector.orthographicSize;
        float halfWidth = projector.orthographicSize * projector.aspectRatio;

        Vector3 projectorPosition = projector.transform.position;
        Quaternion projectorRotation = projector.transform.rotation;

        Vector3 center = projectorPosition + projectorRotation * Vector3.forward * halfHeight;
        Vector3 size = new Vector3(halfWidth * 2, 0.01f, halfHeight * 2);  // 높이는 거의 없도록 설정

        Bounds bounds = new Bounds(center, size);

        Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.extents, projectorRotation);

        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 forceDirection = Vector3.up;
                    rb.AddForce(forceDirection * attackForce, ForceMode.Impulse);
                    // PlayerAttack(smashDamage);
                }
            }
        }
    }
}
