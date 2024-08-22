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

        foreach (var slot in InvenSlot)
        {
            slot.button.onClick.AddListener(() => OnInventorySlotClicked(slot));
        }

        foreach (var slot in StorageSlot)
        {
            slot.button.onClick.AddListener(() => OnStorageSlotClicked(slot));
        }
    }

    private void OnEnable()
    {
        storagePanel.SetActive(true);
        inventoryPanel.SetActive(true);
    }

    private void OnDisable()
    {
        storagePanel.SetActive(false);
        inventoryPanel.SetActive(false);
    }

    private void OnInventorySlotClicked(ItemSlotUI slotUI)
    {
        int index = slotUI.index;
        if (InvenSlots[index].item != null)
        {
            MoveItem(InvenSlots, StorageSlots, index);
            firebaseManager.SaveStorageData(StorageSlots[index].item);
            firebaseManager.DeleteItemData(InvenSlots[index].item);
        }
    }

    private void OnStorageSlotClicked(ItemSlotUI slotUI)
    {
        int index = slotUI.index;
        if (StorageSlots[index].item != null)
        {
            MoveItem(StorageSlots, InvenSlots, index);
            firebaseManager.SaveItemData(InvenSlots[index].item);
            firebaseManager.DeleteStorageData(StorageSlots[index].item);
        }
    }

    private void MoveItem(ItemSlot[] fromSlots, ItemSlot[] toSlots, int index)
    {
        // ºó ½½·Ô Ã£±â
        int targetIndex = FindEmptySlot(toSlots);

        if (targetIndex != -1)
        {
            toSlots[targetIndex] = fromSlots[index];
            fromSlots[index] = new ItemSlot(); // ¿ø·¡ ½½·Ô ºñ¿ì±â
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
        // UI °»½Å
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
        inventoryPanel.SetActive(false);
        storagePanel.SetActive(false);
    }
}
