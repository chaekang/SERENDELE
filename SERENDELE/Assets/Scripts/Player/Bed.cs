using UnityEngine;
using UnityEngine.UI;

public class Bed : MonoBehaviour
{
    [SerializeField]
    private GameObject bedPanel; // Panel with Yes and No buttons

    private DayNightCycle dayNightCycle;
    private FirebaseManager firebaseManager;

    private void Start()
    {
        // Find the DayNightCycle component in the scene
        dayNightCycle = FindObjectOfType<DayNightCycle>();
        firebaseManager = FindObjectOfType<FirebaseManager>();

        if (dayNightCycle == null)
        {
            Debug.LogError("DayNightCycle script not found in the scene.");
        }

        // Initially hide the panel
        bedPanel.SetActive(false);
    }

    private void OnMouseDown()
    {
        // This method is called when the bed object is clicked
        bedPanel.SetActive(true); // Show the panel
    }

    public void OnYesClicked()
    {
        SaveCurrentInventory();

        if (dayNightCycle != null)
        {
            // Set time to 6 AM, which is 6/24 in the day cycle percentage
            dayNightCycle.time = 6f / 24f;
        }
        bedPanel.SetActive(false); // Hide the panel
    }

    public void OnNoClicked()
    {
        bedPanel.SetActive(false); // Just hide the panel
    }

    private void SaveCurrentInventory()
    {
        Inventory inventory = Inventory.instance;

        if (inventory != null && firebaseManager != null)
        {
            foreach (var slot in inventory.slots)
            {
                if (slot.item != null)
                {
                    firebaseManager.SaveItemData(slot.item);
                }
            }

            foreach (var playerSlot in inventory.playerSlot)
            {
                if (playerSlot.item != null)
                {
                    firebaseManager.SaveItemData(playerSlot.item);
                }
            }
        }
    }
}
