using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Durability : MonoBehaviour
{
    public Wand wandScript;
    public float damage;
    public float speed;
    public float distance;

    private Vector3 initPosition;
    private float curDistance;
    // Start is called before the first frame update
    void Start()
    {
        wandScript = FindObjectOfType<Wand>();

        damage = wandScript.damage;
        speed = wandScript.projectileSpeed;
        distance = wandScript.projectileDistance;

        initPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);

        curDistance = Vector3.Distance(initPosition, transform.position);
        if(curDistance > distance)
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
                Debug.Log("projctile attack Enemy");
            }
        }
    }
}
