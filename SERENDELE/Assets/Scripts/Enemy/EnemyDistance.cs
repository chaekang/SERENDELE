using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EnemyDistance : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;

    public float enemyHealth;
    public float enemyCurHealth;
    public float damage; // Test damage to see if enemy explode when its health became 0 
    public float UpdateTargetDistance = 1f;
    private Transform camera;

    // Attack 
    [SerializeField]
    public GameObject bulletPrefab;

    public float spawnRateMin = 0.5f;
    public float spawnRateMax = 3f;

    private Transform target;
    private float spawnRate;
    private float timeAfterSpawn;
    private Vector3 spawnPosition;

    private Animator anim;

    private bool isPlayerInRange = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        GetEnemyInform();
        HealthBarSet();
        BulletSetting();
        healthBar.gameObject.SetActive(false);
    }

    void Update()
    {
        EnemyDamaged();
        UpdateTargetArea();
        UpdateHealthBarPosition();
    }

    private void GetEnemyInform()
    {
        camera = Camera.main.transform;
        GameObject enemyController = GameObject.Find("EnemyController");
        enemyHealth = enemyController.GetComponent<EnemyInformation>().enemyDistanceHealth;
        enemyCurHealth = enemyHealth;
        damage = 5f;
    }

    private void EnemyDamaged()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            anim.SetTrigger("GetHit");
            enemyCurHealth -= damage;
            Debug.Log("피해입음" + damage);
            Debug.Log("적 체력 : " + enemyCurHealth);
            HealthBarSet();
            if (enemyCurHealth <= 0)
            {
                anim.SetBool("isDead", true);
                //Destroy(gameObject);
            }
        }
    }

    // HP 업데이트
    private void HealthBarSet()
    {
        if (healthBar != null)
            healthBar.value = enemyCurHealth / enemyHealth;
    }

    // 방향 조절
    private void Move()
    {
        transform.LookAt(new Vector3(target.transform.position.x, 0, target.transform.position.z));
        //healthBar.transform.LookAt(healthBar.transform.position + camera.rotation * Vector3.forward, camera.rotation * Vector3.up);
    }


    private void BulletSetting()
    {
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
    }

    private void AttackDistance()
    {
        // target = GameObject.FindWithTag("Player").transform;
        spawnPosition = target.position + new Vector3(0, 3f, 0);
        timeAfterSpawn += Time.deltaTime;

        if (timeAfterSpawn >= spawnRate)
        {
            anim.SetBool("isAttack", true);
            timeAfterSpawn = 0;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, target.rotation);  // 총알 소환
            bullet.transform.LookAt(target);
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }

    private void UpdateTargetArea()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, UpdateTargetDistance);
        bool playerDetected = false;

        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Player"))
                {
                    target = cols[i].transform;
                    healthBar.gameObject.SetActive(true);
                    AttackDistance();
                    Move();
                    playerDetected = true;
                }
            }
        }

        if (playerDetected)
        {
            isPlayerInRange = true;
        }
        else if (isPlayerInRange)
        {
            // 플레이어가 범위에서 벗어났을 때 애니메이션과 체력바를 비활성화
            anim.SetBool("isAttack", false);
            healthBar.gameObject.SetActive(false);
            isPlayerInRange = false;
        }
    }
    private void UpdateHealthBarPosition()
    {
        if (target != null && healthBar != null)
        {
            Vector3 healthBarPos = transform.position + new Vector3(0, 1.25f, 0);
            healthBar.transform.position = Camera.main.WorldToScreenPoint(healthBarPos);

            // Distance between Enemy and Player
            float distance = Vector3.Distance(transform.position, target.position);

            // Size of Slider
            float scale = Mathf.Clamp(1 / distance * 5f, 0.3f, 1.5f);
            healthBar.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
