using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public StorageManager storage;
    GameObject systemPanel;

    private void Start()
    {
        systemPanel = storage.systemPanel;
        systemPanel.SetActive(false);
    }

    private void OnMouseDown()
    {
        systemPanel.SetActive(true);
        storage.LoadDataFromFirebase();
    }
}
