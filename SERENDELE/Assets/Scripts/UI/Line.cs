using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    Dictionary<int, string[]> talkData;  // 캐릭터 id, 대사
    int randomChoiceKey;

    private void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        /* 100대: NPC 이름, 10대: 대사 나오는 순서, 1대: 대사 */
        // 100: Rio
        talkData.Add(100, new string[] {
            "안녕? 여기는 처음 왔구나?",
            "세렌델 숲 속 마을에 온 걸 환영해." });
        talkData.Add(100 + 10 + 1, new string[] {
            "나도 이 마을을 떠나서 여행을 하고 싶어.",
            "그래도 역시 모험은 무서워..." });
        talkData.Add(100 + 10 + 2, new string[] {
            "가끔 숲에 들어가서 먹을 걸 채집해오곤 해.",
            "깊이 들어가지는 않아."});
        talkData.Add(100 + 10 + 3, new string[] {
            "나는 아주 어렸을 때부터 여기에 살았어.",
            "여기에서 태어나지는 않았지만 여긴 내 고향이나 마찬가지야."});

    }

    public string GetTalk(int id, int index, int talkIndex)
    {
        int baseKey = id + index;
        string selectedString = null;

        // 선택지 중 하나를 랜덤으로 선택
        if (talkIndex == 0)
        {
            // 처음 대사일 경우 randomChoiceKey를 새로 선택하고 저장
            randomChoiceKey = baseKey + Random.Range(1, 4);
        }

        // 기본 대사 가져오기
        if (talkData.ContainsKey(baseKey))
        {
            if (talkIndex == talkData[baseKey].Length)
            {
                selectedString = null;
            }
            else
            {
                selectedString = talkData[baseKey][talkIndex];
            }
        }

        // 저장된 randomChoiceKey를 사용하여 대사 가져오기
        if (talkData.ContainsKey(randomChoiceKey))
        {
            if (talkIndex < talkData[randomChoiceKey].Length)
            {
                selectedString = talkData[randomChoiceKey][talkIndex];
            }
        }

        return selectedString;
    }
}
