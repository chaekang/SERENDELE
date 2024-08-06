using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    public Wand wandScript;
    public Camera cam; 
    public float damage;
    public float speed;
    public float distance;

    private Vector3 initPosition;
    private float curDistance;

    private Vector3 direction;

    void Start()
    {
        if (cam == null)
        {
            GameObject itemCameraObject = GameObject.FindGameObjectWithTag("ItemCamera");
            cam = itemCameraObject.GetComponent<Camera>();
        }

        wandScript = FindObjectOfType<Wand>();

        damage = wandScript.damage;
        speed = wandScript.projectileSpeed;
        distance = wandScript.projectileDistance;

        initPosition = transform.position;

        // 화면 정중앙의 월드 좌표를 계산
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane);
        Vector3 worldCenter = cam.ScreenToWorldPoint(screenCenter);

        // 투사체의 방향을 계산하고 설정
        direction = (worldCenter - transform.position).normalized;
        transform.forward = direction;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        // 투사체를 현재 방향으로 이동
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        curDistance = Vector3.Distance(initPosition, transform.position);
        if (curDistance > distance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Slider enemySlider = other.gameObject.GetComponentInChildren<Slider>();
            if (enemySlider != null)
            {
                enemySlider.value -= damage;
                Destroy(gameObject);
                Debug.Log("Projectile attack Enemy");
            }
        }
    }
}
