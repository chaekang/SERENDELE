using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun.Demo.Asteroids;
using System;
using Random = UnityEngine.Random;
using JetBrains.Annotations;
using static System.Net.WebRequestMethods;

public class Boss : MonoBehaviour
{
    // Shaking Camera
    private Camera camera;
    private Vector3 cameraPos;
    [SerializeField][Range(0.01f, 0.1f)] private float shakeRange = .05f;
    [SerializeField][Range(0.1f, 1.0f)] private float shakeDuration = .5f;
    [SerializeField] private int shakeInitSpace = 5;
    Coroutine startCameraShakeCoroutine, endCameraShakeCoroutine;

    // Boss Information
    [SerializeField]
    private Slider healthBar;

    public float bossHealth;
    public float bossCurHealth;

    public float UpdateTargetDistance = 100f;
    private Vector3 bossPosition;

    public Animator animator;
    private bool isAnimating;

    // Attack Basic Informaiton 
    private Transform target;
    private Vector3 attackPosition;
    private GameObject bossAttackArea;
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
        GetCameraInform();
        HealthBarSet();
        BulletSetting();

        StartCoroutine(AttackPattern());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargetArea();
        StopChasingPlayer();
    }

    private void GetBossInform()
    {
        GameObject enemyController = GameObject.Find("EnemyController");
        bossHealth = enemyController.GetComponent<EnemyInformation>().enemyDistanceHealth;
        bossCurHealth = bossHealth;
        spawnRate = 3f;
        isAnimating = false;

        target = GameObject.FindWithTag("Player").transform;

        if (target == null)
        {
            GetBossInform();
        }

        bossAttackArea = GameObject.FindWithTag("BossAttackArea");
    }
    
    private void GetAttackRange()
    {
        enemyController = GameObject.Find("EnemyController");
        attackRangeSector = enemyController.transform.Find("AttackRangeSector").GetComponent<Projector>();
        attackRangeLine = enemyController.transform.Find("AttackRangeLine").GetComponentInChildren<Projector>();
        attackRangeCircle = enemyController.transform.Find("AttackRangeCircle").GetComponentInChildren<Projector>();

    }

    private void GetCameraInform()
    {
        camera = Camera.main;
        cameraPos = camera.transform.localPosition;
    }

    private void StopChasingPlayer()
    {
        if (target == null)
        {
            return;
        }
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌함");
        if (other.CompareTag("Player")) 
        {
            Debug.Log("플레이어와 충돌");
            BossDamaged(10f);  
        }
    }

    private void BossDamaged(float damage)
    {
        if (bossAttackArea)
        {
            isAnimating = true;

            animator.SetTrigger("Stun");

            bossCurHealth -= damage;
            HealthBarSet();

            if (bossCurHealth <= 0)
            {
                Destroy(gameObject);
            }

            isAnimating = false;

            Debug.Log("피해입음" + damage);
            Debug.Log("적 체력 : " + bossCurHealth);
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

    private void ShakeCamera(float shakeRange = 0, float duration = 0)
    {
        Debug.Log("카메라 흔들기 호출");

        StopPrevCameraShakeCoroutines();

        shakeRange = shakeRange == 0 ? shakeRange : shakeRange;
        duration = duration == 0 ? shakeDuration : duration;

        startCameraShakeCoroutine = StartCoroutine("StartShake", shakeRange);
        endCameraShakeCoroutine = StartCoroutine("StopShake", duration);
    }

    private IEnumerator StartShake(float shakeRange)
    {
        float cameraPosX, cameraPosY;
        int shakeInitSpacing = shakeInitSpace;

        while (true)
        {
            Vector3 cameraOriginalPos = camera.transform.position;

            --shakeInitSpacing;
            cameraPosX = Random.Range(-shakeRange, shakeRange);
            cameraPosY = Random.Range(-shakeRange * 0.5f, shakeRange);

            // cameraPosX = Random.value * shakeRange * 2 - shakeRange;
            // cameraPosY = Random.value * shakeRange * 2 - shakeRange;

            Vector3 shakePos = cameraOriginalPos;
            shakePos.x += cameraPosX;
            shakePos.y += cameraPosY;

            camera.transform.position = shakePos;

            if (shakeInitSpacing < 0)
            {
                shakeInitSpacing = shakeInitSpace;
                camera.transform.localPosition = cameraOriginalPos;
            }

            yield return null;
        }
    }

    private IEnumerator StopShake(float duration)
    {
        yield return new WaitForSeconds(duration);

        camera.transform.localPosition = cameraPos;

        StopPrevCameraShakeCoroutines();
    }

    private void StopPrevCameraShakeCoroutines()
    {
        if (startCameraShakeCoroutine != null)
        {
            StopCoroutine(startCameraShakeCoroutine);
        }

        if (endCameraShakeCoroutine != null)
        {
            StopCoroutine(endCameraShakeCoroutine);
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
        string[] attackName = { "Smash", "SpawnEnemy", "Swing" };
        // string[] attackName = { "AttackDistance" };
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
        spawnPosition = target.position + new Vector3(0, 3f, 0);
        timeAfterSpawn = 0f;
        animationWaitTime = 57f / 60f;

        while (timeAfterSpawn < spawnRate)
        {
            timeAfterSpawn += Time.deltaTime;
            yield return null;
        }
        animator.SetTrigger("Distance");
        yield return new WaitForSeconds(animationWaitTime);

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, target.rotation);
        bullet.transform.LookAt(target);
    }

    private IEnumerator SpawnEnemy()
    {
        bossPosition = this.gameObject.transform.position;
        bossPosition.y += 5.0f;
        isAnimating = true;
        animationWaitTime = 96f / 60f;

        animator.SetTrigger("Spawn");
        yield return new WaitForSeconds(animationWaitTime);
        // Spawn enemyDistance
        for (int i = 0; i < spawnNumEnemyDistance; i++)
        {
            GameObject enemy = Instantiate(enemyDistance, bossPosition, target.rotation);
            enemy.transform.LookAt(target);
        }

        for (int i = 0; i < spawnNumEnemyExplode; i++)
        {
            //GameObject enemy = Instantiate(enemyExplode, bossPosition, target.rotation);
            //enemy.transform.LookAt(target);
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

        attackDistance = 10.0f;
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
            Debug.Log("사정거리 안에 들어오다");
            if (Vector3.Dot(direction.normalized, transform.forward) > dotValue)
            {
                isAnimating = true;
                Debug.Log("애니메이션실행");
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
        
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        targetRigidbody.AddForce(new Vector3(0f, attackForce, 0f), ForceMode.Impulse);

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
