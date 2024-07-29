using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class HpAndExp : MonoBehaviour
{
    // 변수
    public float maxHp;  // 최대 HP
    public float curHp;   // 현재 HP
    public float curExp;  // 현재 경험치
    public int curLevel;  // 현재 레벨
    public int offense;   // 공격력
    public int defense;   // 방어력

    // UI
    private TextMeshProUGUI levelTxt;
    private TextMeshProUGUI HpTxt;
    private TextMeshProUGUI ExpTxt;
    private Slider HPBar;
    private Slider ExpBar;
    private Image HPBarFillImage;

    private Dictionary<int, int> experienceTable;
    private DatabaseReference databaseReference;

    private FirestoreManager firestoreManager;

    private void Start()
    {
        firestoreManager = FindObjectOfType<FirestoreManager>();
        StartCoroutine(SaveDataPeriodically());

        // 경험치 테이블
        experienceTable = new Dictionary<int, int>();
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                LoadExperienceTableFromFirebase();
                firestoreManager.LoadData(UpdatePlayerData);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });

        // UI 찾기
        levelTxt = GameObject.Find("level_status").GetComponent<TextMeshProUGUI>();
        if (levelTxt != null) Debug.Log("levelTxt found and assigned.");
        else Debug.LogError("levelTxt not found!");
        HpTxt = GameObject.Find("hp_status").GetComponent<TextMeshProUGUI>();
        if (HpTxt != null) Debug.Log("HpTxt found and assigned.");
        else Debug.LogError("HpTxt not found!");
        ExpTxt = GameObject.Find("exp_status").GetComponent<TextMeshProUGUI>();
        if (ExpTxt != null) Debug.Log("ExpTxt found and assigned.");
        else Debug.LogError("ExpTxt not found!");
        HPBar = GameObject.Find("HP_bar").GetComponent<Slider>();
        if (HPBar != null) Debug.Log("HPBar found and assigned.");
        else Debug.LogError("HPBar not found!");
        ExpBar = GameObject.Find("EXP_bar").GetComponent<Slider>();
        if (ExpBar != null) Debug.Log("ExpBar found and assigned.");
        else Debug.LogError("ExpBar not found!");

        // HPBar 기본값 초기화
        HPBar.minValue = 0;
        HPBar.maxValue = 1;

        // HPBar 색상
        HPBarFillImage = HPBar.fillRect.GetComponent<Image>();
    }

    private void Update()
    {
        // HP와 동기화
        HPBar.value = curHp / maxHp;
        HpTxt.text = $"{(int)curHp}";

        // HPBar 색상 변경
        if (HPBar.value <= 0.2f)
        {
            HPBarFillImage.color = Color.red;
        }
        else
        {
            HPBarFillImage.color = new Color(0.13f, 0.69f, 0.2f); // 22B033 색상
        }

        // EXP와 동기화
        int maxExpForCurrentLevel = GetExperienceForLevel(curLevel);
        ExpBar.value = maxExpForCurrentLevel > 0 ? curExp / maxExpForCurrentLevel : 0;
        ExpTxt.text = $"{(int)curExp}";

        // 레벨 텍스트 동기화
        levelTxt.text = $"{curLevel}";
    }

    // Firebase에 있는 경험치 테이블 가져오기
    private void LoadExperienceTableFromFirebase()
    {
        databaseReference.Child("experience_table").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting data from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var child in snapshot.Children)
                {
                    int level = int.Parse(child.Key);
                    int experience = int.Parse(child.Value.ToString());
                    experienceTable[level] = experience;
                }
                Debug.Log("Experience table loaded successfully.");
            }
        });
    }

    // experienceTable에 있는 레벨 당 필요 경험치 가져오기
    public int GetExperienceForLevel(int level)
    {
        if (experienceTable.ContainsKey(level))
        {
            return experienceTable[level];
        }
        return 0;
    }

    // 주기적으로 데이터 저장
    private IEnumerator SaveDataPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(600); // 10분 대기
            firestoreManager.SaveData(maxHp, curHp, curExp, curLevel, offense, defense);
        }
    }

    // 플레이어 데이터 업데이트하기
    private void UpdatePlayerData(float maxhp, float curhp, float exp, int level, int off, int def)
    {
        maxHp = maxhp;
        curHp = curhp;
        curExp = exp;
        curLevel = level;
        offense = off;
        defense = def;
        Debug.Log($"Player data loaded: MaxHP={maxhp}, CurHp={curhp}, EXP={exp}, Level={level}, Offense={offense}, Defense={defense}");
    }

    // HP 회복
    public void IncreaseHp(float amount)
    {
        curHp += amount;
        if (curHp > maxHp)
        {
            curHp = maxHp;
        }
        Debug.Log($"HP increased by {amount}. Current HP: {curHp}");
    }

    // HP 감소(공격 등)
    public void DecreaseHp(float amount)
    {
        curHp -= amount * (float)(1 - 0.01 * defense);
        if (curHp < 0)
        {
            curHp = 0;
            Debug.Log("Game Over");
        }
    }

    // 경험치 증가
    public void IncreaseExp(float amount)
    {
        curExp += amount;
        CheckForLevelUp();
    }

    // 레벨업 해야하는지 확인
    private void CheckForLevelUp()
    {
        int requiredExperience = GetExperienceForLevel(curLevel);
        if (curExp >= requiredExperience)
        {
            curExp -= requiredExperience;
            LevelUp();
            firestoreManager.SaveData(maxHp, curHp, curExp, curLevel, offense, defense);  // 레벨업 하면 데이터 저장
        }
    }

    // 레벨업
    private void LevelUp()
    {
        curLevel++;
        IncreaseStatsForLevelUp();
        curHp = maxHp;
    }

    // 레벨업 시 스탯 상승
    public void IncreaseStatsForLevelUp()
    {
        maxHp += 50;
        offense += 4;
        defense += 4;
    }
}
