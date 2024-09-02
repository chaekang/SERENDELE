using UnityEngine;
using UnityEngine.UI;

public class EnemySlider : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private int maxHP = 100;  // 최대 체력 

    private int _hp;

    public int HP
    {
        get => _hp;
        private set => _hp = Mathf.Clamp(value, 0, maxHP);  // 체력은 0 이상, 최대 체력 이하로 유지
    }

    private void Awake()
    {
        HP = maxHP;
        SetMaxHealth(maxHP);
    }

    public void SetMaxHealth(int health)
    {
        _hpBar.maxValue = health;  // 슬라이더 최대 값 => 체력의 최대 값
        _hpBar.value = health;  // 슬라이더 현재 값 => 최대 값
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;  
        _hpBar.value = HP;  // 슬라이더의 현재 값을 체력 값으로
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 무기와 충돌을 감지
        if (collision.gameObject.CompareTag("Wand")) // Wand와 부딪히면 체력 깎이게? 
        {
            TakeDamage(10);  // 데미지 일단 10으로
        }
    }
}
