using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI quatityText;
    private ItemSlot curSlot;
    private Outline outline;

    public int index;
    public bool equipped;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {

    }

    // 아이템 Slot 정보 전달
    public void Set(ItemSlot slot)
    {
        curSlot = slot;
        icon.gameObject.SetActive(true);
        icon.sprite = slot.item.icon;
        quatityText.text = slot.Quantity > 1 ? slot.Quantity.ToString() : string.Empty;

        if (outline != null)
        {

        }
    }

    public void Disable()
    {
        button.interactable = false;
    }

    public void Enable()
    {
        button.interactable = true;
    }

    public void Clear()
    {
        curSlot = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
    }

    public void OnButtonClick()
    {
        Inventory.instance.SelectItem(index);
    }

    public void OnMarketButtonClick()
    {
        if (GameManager.Instance.MarketManager.marketBuy)
        {
            PurchaseManager.instance.SelectItem(index);
        }
        else
        {
            SellManager.instance.SelectItem(index);
        }
    }
}