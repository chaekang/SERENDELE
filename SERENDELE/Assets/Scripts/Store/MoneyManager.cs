using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static int Money {  get; private set; }

    public static void Spend(int cost)
    {
        if (cost > Money)
        {
            Debug.LogError("Not Enough money");
            return;
        }
        Money -= cost;
    }

    public static void Earn(int income)
    {
        Money += income;
    }
}
