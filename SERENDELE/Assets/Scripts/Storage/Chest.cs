using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

public class Chest : MonoBehaviour, IInteractable
{
    public StorageManager storage;
    GameObject systemPanel;

    private void Start()
    {
        systemPanel = storage.systemPanel;
        systemPanel.SetActive(false);
    }

    public void OnInteract()
    {
        systemPanel.SetActive(true);
        storage.LoadDataFromFirebase();
    }

    public string GetInteractPrompt()
    {
        return string.Format("상자 열기");
    }
}
