using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    [Header("Storage")]
    public GameObject storagePanel;
    public ItemSlotUI[] StorageSlot;
    public ItemSlot[] StorageSlots;

    [Header("Inventory")]
    public GameObject inventoryPanel;
    public ItemSlotUI[] InvenSlot;
    public ItemSlot[] InvenSlots;

    FirebaseManager firebaseManager;

    private void Start()
    {
        firebaseManager = GameManager.Instance.FirebaseManager;

        storagePanel.SetActive(false);
        inventoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        storagePanel.SetActive(true);
        inventoryPanel.SetActive(true);
        LoadData();
    }

    private void OnDisable()
    {
        storagePanel.SetActive(false);
        inventoryPanel.SetActive(false);
        SaveData();
    }

    private void LoadData()
    {
        firebaseManager.LoadUserItems((inventoryItems) =>
        {
            for (int i = 0; i < inventoryItems.Count && i < InvenSlots.Length; i++)
            {
                //InvenSlots[i].SetItem(inventoryItems[i]);
            }
        });

        firebaseManager.LoadStorageItems((storageItems) =>
        {
            for (int i = 0; i < storageItems.Count && i < StorageSlots.Length; i++)
            {
                //StorageSlots[i].SetItem(storageItems[i]);
            }
        });
    }

    private void SaveData()
    {
        // Inventory와 Storage의 데이터를 Firebase에 저장
        foreach (var invenSlot in InvenSlots)
        {
            /*if (invenSlot.HasItem())
            {
                firebaseManager.SaveItemData(invenSlot.GetItem());
            }*/
        }

        foreach (var storageSlot in StorageSlots)
        {
            /*if (storageSlot.HasItem())
            {
                firebaseManager.SaveStorageData(storageSlot.GetItem());
            }*/
        }
    }
}
