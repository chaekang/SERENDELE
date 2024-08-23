using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public GameObject systemPanel;

    [Header("Storage")]
    public ItemSlotUI[] StorageSlot;
    public ItemSlot[] StorageSlots;

    [Header("Inventory")]
    public ItemSlotUI[] InvenSlot;
    public ItemSlot[] InvenSlots;

    private ItemSlot selectedItem;
    FirebaseManager firebaseManager;

    private void Start()
    {
        firebaseManager = GameManager.Instance.FirebaseManager;
        systemPanel.SetActive(false);

        StorageSlots = new ItemSlot[StorageSlot.Length];
        for (int i = 0; i < StorageSlots.Length; i++)
        {
            StorageSlots[i] = new ItemSlot();
            StorageSlot[i].index = i;
            StorageSlot[i].Clear();
        }

        InvenSlots = new ItemSlot[InvenSlot.Length];
        for (int i = 0; i < InvenSlots.Length; i++)
        {
            InvenSlots[i] = new ItemSlot();
            InvenSlot[i].index = i;
            InvenSlot[i].Clear();
        }

        foreach (var slot in InvenSlot)
        {
            slot.button.onClick.AddListener(() => OnInventorySlotClicked(slot));
        }

        foreach (var slot in StorageSlot)
        {
            slot.button.onClick.AddListener(() => OnStorageSlotClicked(slot));
        }
    }

    public void LoadDataFromFirebase()
    {
        firebaseManager.LoadUserItems(inventoryData =>
        {
            // 불러온 인벤토리 데이터를 InvenSlots에 설정
            for (int i = 0; i < inventoryData.Count && i < InvenSlots.Length; i++)
            {
                InvenSlots[i].item = inventoryData[i];
            }
            RefreshUI();
        });

        firebaseManager.LoadStorageItems(storageData =>
        {
            // 불러온 스토리지 데이터를 StorageSlots에 설정
            for (int i = 0; i < storageData.Count && i < StorageSlots.Length; i++)
            {
                StorageSlots[i].item = storageData[i];
            }
            RefreshUI();
        });
    }

    private void OnInventorySlotClicked(ItemSlotUI slotUI)
    {
        int index = slotUI.index;
        if (InvenSlots[index].item != null)
        {
            selectedItem = InvenSlots[index]; // 선택된 아이템 저장
            MoveItem(InvenSlots, StorageSlots, index);
            firebaseManager.SaveStorageData(selectedItem.item);
            firebaseManager.DeleteItemData(selectedItem.item);
            selectedItem = null; // 작업이 끝난 후 초기화
        }
    }

    private void OnStorageSlotClicked(ItemSlotUI slotUI)
    {
        int index = slotUI.index;
        if (StorageSlots[index].item != null)
        {
            selectedItem = StorageSlots[index]; // 선택된 아이템 저장
            MoveItem(StorageSlots, InvenSlots, index);
            firebaseManager.SaveItemData(selectedItem.item);
            firebaseManager.DeleteStorageData(selectedItem.item);
            selectedItem = null; // 작업이 끝난 후 초기화
        }
    }

    private void MoveItem(ItemSlot[] fromSlots, ItemSlot[] toSlots, int index)
    {
        // 빈 슬롯 찾기
        int targetIndex = FindEmptySlot(toSlots);

        if (targetIndex != -1)
        {
            toSlots[targetIndex] = fromSlots[index];
            fromSlots[index] = new ItemSlot(); // 원래 슬롯 비우기
            RefreshUI();
        }
    }

    private int FindEmptySlot(ItemSlot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                return i;
        }
        return -1;
    }

    private void RefreshUI()
    {
        // UI 갱신
        for (int i = 0; i < InvenSlot.Length; i++)
        {
            if (InvenSlots[i].item != null)
                InvenSlot[i].Set(InvenSlots[i]);
            else
                InvenSlot[i].Clear();
        }

        for (int i = 0; i < StorageSlot.Length; i++)
        {
            if (StorageSlots[i].item != null)
                StorageSlot[i].Set(StorageSlots[i]);
            else
                StorageSlot[i].Clear();
        }
    }

    public void StorageExitBtn()
    {
        systemPanel.SetActive(false);
    }
}
