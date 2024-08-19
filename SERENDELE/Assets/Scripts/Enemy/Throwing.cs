using UnityEngine;
using System.Collections;

public class Throwing : MonoBehaviour
{
    public Transform firePoint;  // 투사체가 발사될 위치
    public float lifetime = 10f;  // 구체의 생명주기
    private float projectileSpeed = 20f;
    private int projectileDamage = 10;

    private float gravity = 9.81f;  // 중력 가속도
    private float firingAngle = 45f; // 발사 각도
    private Transform myTransform;

    private void Start()
    {
        myTransform = transform;
        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not assigned.");
            return;
        }

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }

        // 구체가 lifetime 시간이 지나면 자동으로 제거
        Destroy(gameObject, lifetime);

        // 포물선 투사체 발사
        StartCoroutine(ParabolicProjection(player));
    }

    IEnumerator ParabolicProjection(Transform player)
    {
        // 투사체를 firePoint 위치로 이동
        myTransform.position = firePoint.position;

        // 목표물까지 거리
        float target_Distance = Vector3.Distance(myTransform.position, player.position);

        // 목표물에 도달하는 데 필요한 초기 속도 계산
        float projectile_Velocity = Mathf.Sqrt(target_Distance * gravity / Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad));

        // 속도의 X, Y 구성 요소 계산
        float Vx = projectile_Velocity * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = projectile_Velocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // 비행 시간 계산
        float flight_Duration = target_Distance / Vx;

        // 발사체를 회전시켜 목표물을 향하도록 설정
        myTransform.rotation = Quaternion.LookRotation(player.position - myTransform.position);

        // 경과 시간 초기화
        float elapse_time = 0;

        // 투사체가 목표 지점에 도달할 때까지 포물선을 그리며 이동
        while (elapse_time < flight_Duration)
        {

            // 투사체를 포물선을 그리며 이동
            myTransform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime, Space.World);

            // 경과 시간 증가
            elapse_time += Time.deltaTime;

            yield return null;
        }

        // 목표에 도달하면 투사체 제거
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 시, 구체 제거
        Destroy(gameObject);
    }
}
