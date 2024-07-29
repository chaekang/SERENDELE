using System;
using UnityEngine;

public enum ItemType
{
    Resource,   // 자원(성냥, 돌 등)
    Equipable,  // 착용할 수 있음(무기, 방어구 등)
    Consumable  // 물약, 음식
}
public enum EquipableType
{
    Head,    
    Body,
    Shoes,
    Weapon,
    None
}

public enum AttackDefenseType
{
    Offense,
    Defense
}

[Serializable]
public class ItemDataConsumable
{
    public float value;
}

[Serializable]
public class ItemDataAttackDefense
{
    public AttackDefenseType type;
    public float value;
}

[Serializable]
public class SerializableItemData
{
    public string displayName;    // 이름
    public string description;    // 설명
    public ItemType type;         // 아이템 타입
    public EquipableType equipableType;  // 장착 타입
    public int quantity;          // 수량
    public bool canStack;         // 여러 개 소지 가능
    public int maxStackAmount;    // 최대 소지 개수
    public ItemDataAttackDefense[] attackDefense;  // 공격력, 방어력
    public int consumableValue;   // 소모 가능한 아이템의 체력과 배고픔 데이터
    public int itemPrice;         // 가격
    public string dropPrefabPath; // Prefab의 경로를 저장
    public string iconPath;       // 아이콘의 경로를 저장
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;    // 이름
    public string description;    // 설명
    public ItemType type;         // 아이템 타입
    public EquipableType equipableType;         // 장착 타입
    public Sprite icon;           // 인벤토리 아이콘
    public GameObject dropPrefab; // 프리팹
    public int quantity;      // 수량(내가 설정하는게 아니라 Firebase에서 필요)

    [Header("Stacking")]
    public bool canStack;       // 여러 개 소지 가능
    public int maxStackAmount;  // 최대 소지 개수

    // 무기, 방어구의 공격력, 방어력
    [Header("OffenseDefense")]
    public ItemDataAttackDefense[] attackDefense;

    // 소모 가능한 아이템의 체력과 배고픔 데이터
    [Header("Consumable")]
    public int consumableValue;

    // 가격
    [Header("Price")]
    public int itemPrice;

    // SerializableItemData로 변환하는 메서드
    public SerializableItemData ToSerializable()
    {
        return new SerializableItemData
        {
            displayName = displayName,
            description = description,
            type = type,
            equipableType = equipableType,
            quantity = quantity,
            canStack = canStack,
            maxStackAmount = maxStackAmount,
            attackDefense = attackDefense,
            consumableValue = consumableValue,
            itemPrice = itemPrice,
            dropPrefabPath = dropPrefab != null ? "Item/" + dropPrefab.name : null,
            iconPath = icon != null ? "Icons/"+ icon.name : null
        };
    }

    // SerializableItemData에서 ItemData로 변환하는 메서드
    public void FromSerializable(SerializableItemData serializableItemData)
    {
        displayName = serializableItemData.displayName;
        description = serializableItemData.description;
        type = serializableItemData.type;
        equipableType = serializableItemData.equipableType;
        quantity = serializableItemData.quantity;
        canStack = serializableItemData.canStack;
        maxStackAmount = serializableItemData.maxStackAmount;
        attackDefense = serializableItemData.attackDefense;
        consumableValue = serializableItemData.consumableValue;
        itemPrice = serializableItemData.itemPrice;

        // Resources 폴더에서 Prefab 로드
        if (!string.IsNullOrEmpty(serializableItemData.dropPrefabPath))
        {
            dropPrefab = Resources.Load<GameObject>(serializableItemData.dropPrefabPath);
        }
        else
        {
            dropPrefab = null;
        }

        // 아이콘 로드
        if (!string.IsNullOrEmpty(serializableItemData.iconPath))
        {
            icon = Resources.Load<Sprite>(serializableItemData.iconPath);
        }
        else
        {
            icon = null;
        }
    }
}
