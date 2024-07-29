using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSlot
{
    public ItemData item;
    public int quantity;

    public int Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }
}

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uidSlot;     // UI ���� ������ ����
    public ItemSlot[] slots;         // ���� ������ ������ ����Ǵ� �迭

    public ItemSlotUI[] uiPlayerSlot; // 0: head, 1: body, 2: shoes, 3: weapon
    public ItemSlot[] playerSlot;

    public GameObject inventoryWindow;      // �κ��丮 â
    public Transform dropPosition;      // ������ ��� ��ġ

    // ���� ����
    public Transform equipWeaponPosition;
    private GameObject equipWeapon;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    [Header("Equip")]
    public GameObject equipPrefab;
    private int curEquipIndex;

    public static Inventory instance;

    private FirebaseManager firebaseManager;

    private void Awake()
    {
        instance = this;
        firebaseManager = FindObjectOfType<FirebaseManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        inventoryWindow.SetActive(false);

        slots = new ItemSlot[uidSlot.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            // UI Slot �ʱ�ȭ�ϱ�
            slots[i] = new ItemSlot();
            uidSlot[i].index = i;
            uidSlot[i].Clear();
        }

        playerSlot = new ItemSlot[uiPlayerSlot.Length];
        for (int i = 0; i < playerSlot.Length; i++)
        {
            // UI Slot �ʱ�ȭ�ϱ�
            playerSlot[i] = new ItemSlot();
            uiPlayerSlot[i].index = i;
            uiPlayerSlot[i].Clear();
        }

        ClearSelectItemWindow();

        firebaseManager.LoadUserItems(OnItemsLoaded);
    }

    // �κ��丮 Ű �Է� Ȯ��
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !GameManager.Instance.MarketManager.isMarketPanelActive)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        bool isOpen = !inventoryWindow.activeInHierarchy;
        inventoryWindow.SetActive(isOpen);

        if (isOpen )
        {
            firebaseManager.LoadUserItems(OnItemsLoaded);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)
    {
        // �ߺ� ��� ������
        if (item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity += item.quantity;
                firebaseManager.SaveItemData(slotToStackTo.item); // ������ ����
                UpdateUI();
                return;
            }
        }

        // �ߺ� ����� ������
        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = item.quantity;

            firebaseManager.SaveItemData(emptySlot.item); // ������ ����
            UpdateUI();
            return;
        }

    }

    private void ThrowItem(ItemData item)
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360f));
    }

    void UpdateUI()
    {
        for (int i = 0; i < playerSlot.Length; i++)
        {
            if (playerSlot[i].item != null)
                uiPlayerSlot[i].Set(playerSlot[i]);
            else
                uiPlayerSlot[i].Clear();
        }

        List<ItemSlot> filledSlots = new List<ItemSlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                filledSlots.Add(slots[i]);
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < filledSlots.Count)
            {
                slots[i] = filledSlots[i];
                uidSlot[i].Set(slots[i]);
            }
            else
            {
                slots[i] = new ItemSlot();
                uidSlot[i].Clear();
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
                uidSlot[i].Set(slots[i]);
            else
                uidSlot[i].Clear();
        }
    }


    ItemSlot GetItemStack(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStackAmount)
                return slots[i];
        }

        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                return slots[i];
        }

        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null && playerSlot[index].item == null) return;

        // ������ ������ ���� ��������

        if (slots[index].item != null)
        {
            selectedItem = slots[index];
            selectedItemIndex = index;

            selectedItemName.text = selectedItem.item.displayName;
            selectedItemDescription.text = selectedItem.item.description;
            selectedItemStatName.text = "HP";
            selectedItemStatValue.text = selectedItem.item.consumableValue.ToString();

            if (selectedItem.item.attackDefense != null)
            {
                for (int i = 0; i < selectedItem.item.attackDefense.Length; i++)
                {
                    selectedItemStatName.text += selectedItem.item.attackDefense[i].type.ToString() + "\n";
                    selectedItemStatValue.text += selectedItem.item.attackDefense[i].value.ToString() + "\n";
                }
            }

            useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
            equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uidSlot[index].equipped);
            unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uidSlot[index].equipped);
            dropButton.SetActive(true);
        }
        else if (playerSlot[index].item != null)
        {

            Debug.Log("Select: " + playerSlot[index].item.name);

            selectedItem = playerSlot[index];
            selectedItemIndex = index;

            selectedItemName.text = selectedItem.item.displayName;
            selectedItemDescription.text = selectedItem.item.description;
            selectedItemStatName.text = string.Empty;
            selectedItemStatValue.text = string.Empty;

            for (int i = 0; i < selectedItem.item.attackDefense.Length; i++)
            {
                selectedItemStatName.text += selectedItem.item.attackDefense[i].type.ToString() + "\n";
                selectedItemStatValue.text += selectedItem.item.attackDefense[i].value.ToString() + "\n";
            }

            unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiPlayerSlot[index].equipped);
            dropButton.SetActive(true);
        }
    }

    public void ClearSelectItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if (selectedItem.item.type == ItemType.Consumable)
        {
            int value = selectedItem.item.consumableValue;
            GameManager.Instance.HpAndExp.IncreaseHp(value);
        }

        RemoveSelectedItem();
    }

    public void OnEquipButton()
    {
        if (selectedItem.item.type == ItemType.Equipable)
        {
            if (selectedItem.item.equipableType == EquipableType.Weapon)
            {
                playerSlot[3].item = selectedItem.item;
                equipWeapon = Instantiate(selectedItem.item.dropPrefab, equipWeaponPosition.position, Quaternion.identity);
                equipWeapon.transform.SetParent(equipWeaponPosition);

                // ********************
                WeaponManager weaponManager = equipWeapon.gameObject.GetComponent<WeaponManager>();
                weaponManager.isHand = true;
                // ********************
            }
            else if (selectedItem.item.equipableType == EquipableType.Head)
            {
                playerSlot[0].item = selectedItem.item;
            }
            else if (selectedItem.item.equipableType == EquipableType.Body)
            {
                playerSlot[1].item = selectedItem.item;
            }
            else if (selectedItem.item.equipableType == EquipableType.Shoes)
            {
                playerSlot[2].item = selectedItem.item;
            }

            slots[selectedItemIndex].item = null;
            slots[selectedItemIndex].quantity = 0;
            uiPlayerSlot[3].equipped = true;

            ClearSelectItemWindow();
            UpdateUI();
        }
    }

    void UnEquip(int index)
    {
        if (playerSlot[3].item != null)
        {
            ItemSlot emptySlot = GetEmptySlot();

            if (emptySlot != null)
            {
                emptySlot.item = playerSlot[3].item;
                emptySlot.quantity = playerSlot[3].quantity;

                playerSlot[3].item = null;
                playerSlot[3].quantity = 0;
                uidSlot[index].equipped = false;

                UpdateUI();
            }
            if (equipWeapon != null)
            {
                Destroy(equipWeapon);
                equipWeapon = null;
            }

            selectedItemName.text = string.Empty;
            selectedItemDescription.text = string.Empty;
            selectedItemStatName.text = string.Empty;
            selectedItemStatValue.text = string.Empty;

            unEquipButton.SetActive(false);
            dropButton.SetActive(false);
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;
        firebaseManager.DeleteItemData(selectedItem.item); // ������ ����

        if (selectedItem.quantity <= 0)
        {
            if (uidSlot[selectedItemIndex].equipped) UnEquip(selectedItemIndex);

            selectedItem.item = null;
            ClearSelectItemWindow();
        }
        UpdateUI();
    }

    public void OnItemsLoaded(List<ItemData> items)
    {
        ClearLocalInventory();

        foreach (var item in items)
        {
            AddItem(item);
        }

        UpdateUI();
    }

    private void ClearLocalInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].item = null;
            slots[i].quantity = 0;
            uidSlot[i].Clear();
        }
    }

    public void RemoveItem(ItemData item)
    {
        // ������ ���� ���� ����
    }

    public bool HasItems(ItemData item, int quantity)
    {
        // ������ ���� ���� Ȯ�� ���� ����
        return false;
    }
}
