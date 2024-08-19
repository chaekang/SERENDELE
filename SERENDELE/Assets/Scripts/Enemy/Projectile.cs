using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;  // 구체가 자동으로 사라지기까지의 시간

    private void Start()
    {
        // 구체가 lifetime 시간이 지나면 자동으로 제거됩니다.
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어와 상호작용 없이 구체가 충돌할 경우에만 제거됩니다.
        Destroy(gameObject);
    }
}
