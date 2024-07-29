using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public bool isHand = false; // 인벤토리랑 연동돼야함 (우선 true)

    // 무기 정보
    public float durability;

    // 방어
    public bool isDefending = false;

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
        if (Input.GetMouseButtonDown(0)) 
        {
            isDefending = true;
            gameObject.GetComponent<Collider>().enabled = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDefending = false;
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }

}
