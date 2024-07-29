using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sword : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isHand = false;

    // 무기 정보
    public float damage;
    public float speed;

    // 공격
    public bool isAttacking = false;
    public bool isReady = true; // 무기 쿨타임 지났는지
    public float coolTime = 0;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

        if (isHand)
        {
            UseWeapon();
        }
    }
    void UseWeapon()
    {
        if (isReady && Input.GetMouseButtonDown(0)) 
        {
            isAttacking = true;
            StartCoroutine(AttackCoolTime(speed));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isAttacking = false;
        }
    }

    IEnumerator AttackCoolTime(float speed)
    {
        isReady = false;
        coolTime += Time.deltaTime;
        yield return new WaitForSeconds(speed);
        isReady = true;
        coolTime = 0;
    }



    private void OnTriggerStay(Collider other)
    {
        if (isAttacking && other.gameObject.CompareTag("Enemy"))
        {
            Slider enemySlider = other.gameObject.GetComponentInChildren<Slider>();
            if (enemySlider != null)
            {
                enemySlider.value -= damage;
            }
        }
        isAttacking = false;
    }
}
