using System.Collections;
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
    public ItemSlotUI[] uidSlot;
    public ItemSlot[] slots;

    public ItemSlotUI[] uiPlayerSlot; // 0: head, 1: body, 2: shoes, 3: weapon
    public ItemSlot[] playerSlot;

    public GameObject inventoryWindow;
    public Transform dropPosition;  

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
    private LobbyManager lobbyManager;

    private void Awake()
    {
        instance = this;
        firebaseManager = FindObjectOfType<FirebaseManager>();
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        inventoryWindow.SetActive(false);

        slots = new ItemSlot[uidSlot.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new ItemSlot();
            uidSlot[i].index = i;
            uidSlot[i].Clear();
        }

        playerSlot = new ItemSlot[uiPlayerSlot.Length];
        for (int i = 0; i < playerSlot.Length; i++)
        {
            playerSlot[i] = new ItemSlot();
            uiPlayerSlot[i].index = i;
            uiPlayerSlot[i].Clear();
        }

        ClearSelectItemWindow();

        firebaseManager.LoadUserItems(OnItemsLoaded);
        StartCoroutine(FindPositionsCoroutine());
    }

    private IEnumerator FindPositionsCoroutine()
    {
        while (dropPosition == null || equipWeaponPosition == null)
        {
            if (dropPosition == null)
            {
                Debug.Log("Attempting to assign drop position.");
                dropPosition = FindInactiveObjectByName("DropPosition")?.transform;
            }

            if (equipWeaponPosition == null)
            {
                Debug.Log("Attempting to assign equipWeaponPosition.");
                GameObject equipWeaponPositionObj = FindEquipWeaponPosition();
                if (equipWeaponPositionObj != null)
                {
                    equipWeaponPosition = equipWeaponPositionObj.transform;
                }
                else
                {
                    Debug.LogWarning("equipWeaponPosition not found. Retrying...");
                }
            }

            if (dropPosition != null && equipWeaponPosition != null)
            {
                yield break;
            }

            // 잠시 대기한 후 다시 시도
            yield return new WaitForSeconds(0.5f);
        }
    }

    private GameObject FindEquipWeaponPosition()
    {
        // 상위 오브젝트를 먼저 찾습니다.
        if (lobbyManager.Arie)
        {
            GameObject arieObject = GameObject.Find("Arie(Clone)");
            if (arieObject == null)
            {
                return null;
            }
            // 자식 오브젝트를 탐색합니다.
            Transform equipWeaponPositionTransform = arieObject.transform.Find("Armature/Hips/LowerSpine/Chest/Clavicle.R/UpperArm.R/LowerArm.R/Hand.R/Middle01.R/Middle02.R/equipWeaponPosition");

            if (equipWeaponPositionTransform != null)
            {
                return equipWeaponPositionTransform.gameObject;
            }
            else
            {
                Debug.LogError("EquipWeaponPosition 오브젝트를 경로에서 찾을 수 없습니다.");
            }
        }
        else if (lobbyManager.Lembra)
        {
            GameObject LembraObject = GameObject.Find("Lembra(Clone)");
            if (LembraObject == null)
            {
                Debug.LogError("Lembra(Clone) 오브젝트를 찾을 수 없습니다.");
                return null;
            }
            // 자식 오브젝트를 탐색합니다.
            Transform equipWeaponPositionTransform = LembraObject.transform.Find("Armature/Hips/LowerSpine/Chest/Clavicle.R/UpperArm.R/LowerArm.R/Hand.R/Middle01.R/Middle02.R/equipWeaponPosition");

            if (equipWeaponPositionTransform != null)
            {
                return equipWeaponPositionTransform.gameObject;
            }
            else
            {
                Debug.LogError("EquipWeaponPosition 오브젝트를 경로에서 찾을 수 없습니다.");
            }
        }
        return null;
    }


    private GameObject FindInactiveObjectByName(string name)
    {
        // 모든 게임 오브젝트를 찾아 비활성화된 오브젝트까지 검색
        GameObject[] objs = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in objs)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }

        return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !GameManager.Instance.MarketManager.isMarketPanelActive)
        {
            Debug.Log("F key");
            Toggle();
        }
    }

    public void Toggle()
    {
        bool isOpen = !inventoryWindow.activeInHierarchy;
        inventoryWindow.SetActive(isOpen);

        if (isOpen )
        {
            if (firebaseManager != null)
            {
                firebaseManager.LoadUserItems(OnItemsLoaded);

            }
            else
            {
                Debug.LogError("firebaseManager is null");
            }
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)
    {
        if (item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity += item.quantity;
                firebaseManager.SaveItemData(slotToStackTo.item); 
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
    }

    public bool HasItems(ItemData item, int quantity)
    {
        return false;
    }
}
