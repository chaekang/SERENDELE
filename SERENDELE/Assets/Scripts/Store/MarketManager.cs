using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public GameObject purchaseManager;
    public GameObject sellManager;
    public StorageManager storage;

    public GameObject money;

    public GameObject BuyPanel;
    public GameObject SellPanel;

    public bool marketBuy;

    public bool isMarketPanelActive = false;


    private void Update()
    {
        // MarketPanel의 활성화 상태를 확인하여 플래그 설정
        isMarketPanelActive = BuyPanel.activeSelf || SellPanel.activeSelf;

        if (isMarketPanelActive)
        {
            money.gameObject.SetActive(true);
        }
        else
        {
            money.gameObject.SetActive(false);
        }

        if (Inventory.instance.IsOpen())
        {
            ChoiceManager.Instance.choicePanel.gameObject.SetActive(false);
        }
    }
}
