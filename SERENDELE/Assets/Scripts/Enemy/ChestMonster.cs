using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMonster : MonoBehaviour
{
    Animator anim;
    float detectionRadius = 10f;
    float moveSpeed = 5f;
    Transform player;
    GameObject target;

    [SerializeField]
    GameObject explosion;

    private void Start()
    {
        anim = GetComponent<Animator>();
        explosion.SetActive(false);

        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            GameObject targetObject = GameObject.Find("all");
            if (playerObject != null && targetObject != null)
            {
                player = playerObject.transform;
                target = targetObject;
                Debug.Log("player object and target object are not null");
                break;
            }
            yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 재시도
        }
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                
                transform.position += direction * moveSpeed * Time.deltaTime;

                anim.SetBool("isChasing", true);
            }
            else
            {
                anim.SetBool("isChasing", false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            explosion.SetActive(true);
            target.GetComponent<HpAndExp>().DecreaseHp(100);
            Invoke("ActiveFalse", 1.2f);
        }
    }

    void ActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
