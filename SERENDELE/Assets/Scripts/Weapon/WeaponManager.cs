using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isHand = false;
    public bool isOpeningInven = true;
    public int OpenInven;

    private Sword sword;
    private Wand wand;
    private Shield shield;

    void Start()
    {
        OpenInven++;
    }

    // Update is called once per frame
    void Update()
    {
        IsOpeningInventory();
        if (isHand && !isOpeningInven)
        {
            ActivateWeapon(gameObject.tag);
        }
        else if (isHand && isOpeningInven)
        {
            InactivateWeapon(gameObject.tag);
        }
    }

    void ActivateWeapon(string tag)
    {
        switch (tag)
        {
            case "Sword":
                sword = gameObject.GetComponent<Sword>();
                sword.isHand = true;
                break;

            case "Wand":
                wand = gameObject.GetComponent<Wand>();
                wand.isHand = true;
                break;

            case "Shield":
                shield = gameObject.GetComponent<Shield>();
                shield.isHand = true;
                break;
        }
    }

    void InactivateWeapon(string tag)
    {
        switch (tag)
        {
            case "Sword":
                sword = gameObject.GetComponent<Sword>();
                sword.isHand = false;
                break;

            case "Wand":
                wand = gameObject.GetComponent<Wand>();
                wand.isHand = false;
                break;

            case "Shield":
                shield = gameObject.GetComponent<Shield>();
                shield.isHand = false;
                break;
        }
    }

    void IsOpeningInventory()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            OpenInven++;
            if (OpenInven % 2 == 1)
            {
                isOpeningInven = true;
            }
            else
            {
                isOpeningInven = false;
            }
        }
    }
}
