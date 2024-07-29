using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirestoreManager : MonoBehaviour
{
    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private FirebaseUser user;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            db = FirebaseFirestore.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;

            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        });
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
    }

    // 로그인했는지 확인
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public void CheckAndSaveDefaultData(string userId)
    {
        db.Collection("users").Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    SaveDefaultData(userId);
                }
            }
            else
            {
                Debug.LogError("Failed to check user data: " + task.Exception);
            }
        });
    }

    private void SaveDefaultData(string userId)
    {
        Dictionary<string, object> defaultData = new Dictionary<string, object>
        {
            { "maxHp", 200f },
            { "curHp", 200f },
            { "curExp", 0f },
            { "curLevel", 1 },
            { "offense", 0 },
            { "defense", 0 }
        };

        db.Collection("users").Document(userId).SetAsync(defaultData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Default data saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save default data: " + task.Exception);
            }
        });
    }

    // 데이터 저장하기
    public void SaveData(float maxhp, float curhp, float exp, int level, int offense, int defense)
    {
        if (user == null)
        {
            Debug.LogError("User is not logged in. Cannot save data.");
            return;
        }

        string userId = user.UserId; // 로그인한 사용자 ID 가져오기

        // 데이터 구조 정의
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "maxHp", maxhp },
            { "curHp", curhp },
            { "curExp", exp },
            { "curLevel", level },
            { "offense", offense },
            { "defense", defense }
        };

        // Firestore에 데이터 저장
        db.Collection("users").Document(userId).SetAsync(userData).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Data saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save data: " + task.Exception);
            }
        });
    }

    // 데이터 가져오기
    public void LoadData(System.Action<float, float, float, int, int, int> callback)
    {
        if (user == null)
        {
            Debug.LogError("User is not logged in. Cannot load data.");
            return;
        }

        string userId = user.UserId; // 로그인한 사용자 ID 가져오기

        CheckAndSaveDefaultData(userId);

        // Firestore에서 데이터 가져오기
        db.Collection("users").Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Dictionary<string, object> userData = snapshot.ToDictionary();
                    float maxhp = userData.ContainsKey("maxHp") ? float.Parse(userData["maxHp"].ToString()) : 200f;
                    float curhp = userData.ContainsKey("curHp") ? float.Parse(userData["curHp"].ToString()) : 200f;
                    float exp = userData.ContainsKey("curExp") ? float.Parse(userData["curExp"].ToString()) : 0f;
                    int level = userData.ContainsKey("curLevel") ? int.Parse(userData["curLevel"].ToString()) : 1;
                    int offense = userData.ContainsKey("offense") ? int.Parse(userData["offense"].ToString()) : 0;
                    int defense = userData.ContainsKey("defense") ? int.Parse(userData["defense"].ToString()) : 0;
                    callback(maxhp, curhp, exp, level, offense, defense);
                }
                else
                {
                    Debug.LogWarning("No user data found.");
                }
            }
            else
            {
                Debug.LogError("Failed to load data: " + task.Exception);
            }
        });
    }
}
