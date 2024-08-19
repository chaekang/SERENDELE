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

    void Start()
    {
        GetEnemyInform();
        HealthBarSet();
        BulletSetting();
    }

    void Update()
    {
        EnemyDamaged();
        UpdateTargetArea();
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
            enemyCurHealth -= damage;
            Debug.Log("피해입음" + damage);
            Debug.Log("적 체력 : " + enemyCurHealth);
            HealthBarSet();
            if (enemyCurHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void HealthBarSet()
    {
        if (healthBar != null)
            healthBar.value = enemyCurHealth / enemyHealth;
    }

    private void Move()
    {
        transform.LookAt(new Vector3(target.transform.position.x, 0, target.transform.position.z));
        healthBar.transform.LookAt(healthBar.transform.position + camera.rotation * Vector3.forward, camera.rotation * Vector3.up);
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
            timeAfterSpawn = 0;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, target.rotation);
            bullet.transform.LookAt(target);
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
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
                    Move();
                }
            }
        }
        else
        {
            target = null;
        }
    }
}
