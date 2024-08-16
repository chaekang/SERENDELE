using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shortWeapon : MonoBehaviour
{
    public enum Type { Melee, Range }; //근거리, 원거리 공격 변수
    public Type type;    //무기 타입
    public int level;
    private int damage;   //대미지
    private float Aspeed;   //공격 속도
    public BoxCollider attackArea;   //공격 범위
    //public TrailRenderer trailEffect;    //공격 효과

    private GameObject target;

    void Start()
    {
        target = GameObject.FindWithTag("Player");
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

    public void Attack()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        attackArea.enabled = true;
        //체력바와 연동하여 대미지만큼 깎이도록 하기. (플레이어의 체력바가)
        //태그에 부딪혔을 때 슬라이더를 찾아서 마이너스.
        //플레이어 감지는 Player 태그를 통해서 찾도록 수정
        yield return new WaitForSeconds(Aspeed);
        attackArea.enabled = false;
        yield return new WaitForSeconds(Aspeed);

        //결과 전달 키워드 yield. 1 프레임 대기. (1개 이상 사용 가능!)
        // -> yield 키워드를 여러개 사용하면 시간차 로직 작성 가능.
        // yield break 로 코루틴 탈출.

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            target.GetComponent<HpAndExp>().DecreaseHp(damage);
        }
    }

}
