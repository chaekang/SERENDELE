using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class EnemyDistance : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;

    public float enemyHealth;
    public float enemyCurHealth;
    public float damage; 
    public float UpdateTargetDistance = 1f;
    private Transform localCamera;  // 로컬 카메라 참조

    // Attack 
    [SerializeField]
    public GameObject bulletPrefab;

    public float spawnRateMin = 0.5f;
    public float spawnRateMax = 3f;

    private Transform closestTarget;
    private Transform secondClosestTarget;
    private float closestDistance;
    private float secondClosestDistance;

    private float spawnRate;
    private float timeAfterSpawn;
    private Vector3 spawnPosition;

    private Animator anim;
    private PhotonView photonView;

    private bool isPlayerInRange = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();

        // 로컬 카메라 찾기
        if (photonView.IsMine)
        {
            localCamera = Camera.main.transform;
        }
        else
        {
            localCamera = FindLocalCamera();  // 다른 플레이어의 카메라 찾기
        }

        GetEnemyInform();
        HealthBarSet();
        BulletSetting();
        healthBar.gameObject.SetActive(false);
    }

    void Update()
    {
        // U 키를 누를 때만 RPC 호출
        if (Input.GetKeyDown(KeyCode.U))
        {
            photonView.RPC("EnemyDamaged", RpcTarget.All);
        }

        // 일정 주기마다 UpdateTargetArea 호출
        if (Time.frameCount % 10 == 0)  // 10 프레임마다 한 번 호출
        {
            UpdateTargetArea();
        }

        // 체력바 위치 업데이트 최적화
        UpdateHealthBarPosition();
    }

    private void GetEnemyInform()
    {
        GameObject enemyController = GameObject.Find("EnemyController");
        enemyHealth = enemyController.GetComponent<EnemyInformation>().enemyDistanceHealth;
        enemyCurHealth = enemyHealth;
        damage = 5f;
    }

    [PunRPC]
    private void EnemyDamaged()
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

    private void HealthBarSet()
    {
        if (healthBar != null)
            healthBar.value = enemyCurHealth / enemyHealth;
    }

    private void Move()
    {
        if (closestTarget != null)
        {
            transform.LookAt(new Vector3(closestTarget.position.x, 0, closestTarget.position.z));
        }
    }

    private void BulletSetting()
    {
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
    }

    private void AttackDistance()
    {
        if (closestTarget != null)
        {
            spawnPosition = closestTarget.position + new Vector3(0, 3f, 0);
            timeAfterSpawn += Time.deltaTime;

            if (timeAfterSpawn >= spawnRate)
            {
                anim.SetBool("isAttack", true);
                timeAfterSpawn = 0;
                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, closestTarget.rotation);  // 총알 소환
                bullet.transform.LookAt(closestTarget);
                spawnRate = Random.Range(spawnRateMin, spawnRateMax);
            }
        }
    }

    private void UpdateTargetArea()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, UpdateTargetDistance);
        closestTarget = null;
        secondClosestTarget = null;
        closestDistance = Mathf.Infinity;
        secondClosestDistance = Mathf.Infinity;
        bool playerDetected = false;

        foreach (Collider col in cols)
        {
            if (col.CompareTag("Player"))
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    secondClosestTarget = closestTarget;
                    secondClosestDistance = closestDistance;

                    closestTarget = col.transform;
                    closestDistance = distance;
                }
                else if (distance < secondClosestDistance)
                {
                    secondClosestTarget = col.transform;
                    secondClosestDistance = distance;
                }
                playerDetected = true;
            }
        }

        if (playerDetected)
        {
            isPlayerInRange = true;
            AttackDistance();
            Move();
        }
        else if (isPlayerInRange)
        {
            anim.SetBool("isAttack", false);
            isPlayerInRange = false;
        }
    }

    private void UpdateHealthBarPosition()
    {
        if (closestTarget != null && healthBar != null)
        {
            Vector3 healthBarPos = transform.position + new Vector3(0, 1.25f, 0);
            Vector3 screenPos = localCamera.GetComponent<Camera>().WorldToScreenPoint(healthBarPos);

            healthBar.transform.position = screenPos;

            // 현재 로컬 카메라에 해당하는 플레이어의 거리를 계산
            float distanceToLocalPlayer = Vector3.Distance(transform.position, localCamera.position);

            // 체력바 크기를 거리 비례로 조정
            float scale = Mathf.Clamp(1 / distanceToLocalPlayer * 5f, 0.3f, 1.5f);
            healthBar.transform.localScale = new Vector3(scale, scale, scale);

            // 플레이어가 감지 범위 내에 있는지 확인
            if (distanceToLocalPlayer > UpdateTargetDistance)
            {
                healthBar.gameObject.SetActive(false);  // 감지 범위를 벗어나면 체력바 비활성화
            }
            else
            {
                healthBar.gameObject.SetActive(true);  // 감지 범위 내에 있으면 체력바 활성화
            }
        }
    }


    // 로컬 카메라 찾는 함수 추가
    private Transform FindLocalCamera()
    {
        Camera[] cameras = Camera.allCameras;
        foreach (Camera cam in cameras)
        {
            if (cam.CompareTag("MainCamera") && cam.transform.root.GetComponent<PhotonView>().IsMine)
            {
                return cam.transform;
            }
        }
        return null;
    }
}
