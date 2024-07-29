using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using System.Collections;
using System;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference databaseReference;
    private bool isInitialized = false;
    private Queue<ItemData> itemDataQueue = new Queue<ItemData>();
    private FirebaseAuth auth;
    private FirebaseUser user;

    void Start()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            auth = FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask => {
                if (authTask.IsCompleted)
                {
                    user = auth.CurrentUser;
                    isInitialized = true;

                    // 큐에 있는 데이터 저장
                    while (itemDataQueue.Count > 0)
                    {
                        SaveItemData(itemDataQueue.Dequeue());
                    }
                }
                else
                {
                    Debug.LogError("Firebase Authentication failed: " + authTask.Exception);
                }
            });
        });
    }

    // 아이템 데이터 저장 (덮어쓰기)
    public void SaveItemData(ItemData itemData)
    {
        if(!isInitialized)
    {
            Debug.LogWarning("Firebase is not initialized yet. Queueing the data to save later.");
            itemDataQueue.Enqueue(itemData);
            return;
        }

        string itemKey = itemData.displayName;
        SerializableItemData serializableData = itemData.ToSerializable();
        string jsonData = JsonUtility.ToJson(serializableData);

        databaseReference.Child("Inventory").Child(user.UserId).Child(itemKey).SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Item data saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save item data: " + task.Exception);
            }
        });
    }


    // 유저 아이템 데이터 불러오기
    public void LoadUserItems(System.Action<List<ItemData>> callback)
    {
        if (!isInitialized)
        {
            return;
        }

        databaseReference.Child("Inventory").Child(user.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<ItemData> itemList = new List<ItemData>();

                foreach (DataSnapshot itemSnapshot in snapshot.Children)
                {
                    try
                    {
                        string json = itemSnapshot.GetRawJsonValue();
                        SerializableItemData serializableItemData = JsonUtility.FromJson<SerializableItemData>(json);
                        ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                        itemData.FromSerializable(serializableItemData);
                        itemList.Add(itemData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error parsing item data: " + e);
                    }
                }

                Debug.Log("Loaded items count: " + itemList.Count);
                callback(itemList);
            }
            else
            {
                Debug.LogError("Failed to load user items: " + task.Exception);
            }
        });
    }

    // 아이템 데이터 삭제
    public void DeleteItemData(ItemData itemData)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("Firebase is not initialized yet. Cannot delete item data.");
            return;
        }

        string itemKey = itemData.displayName;

        databaseReference.Child("Inventory").Child(user.UserId).Child(itemKey).RemoveValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Item data deleted successfully.");
            }
            else
            {
                Debug.LogError("Failed to delete item data: " + task.Exception);
            }
        });
    }
}
