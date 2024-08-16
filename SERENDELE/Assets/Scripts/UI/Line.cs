using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    Dictionary<int, string[]> talkData;  // 캐릭터 id, 대사

    private void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        /* 100대: NPC 이름, 10대: 대사 나오는 순서, 1대: 대사 */
        // 100: Rio
        talkData.Add(100 + 10, new string[] { 
            "안녕? 여기는 처음 왔구나?", 
            "세렌델에 온 걸 환영해." });
        talkData.Add(100 + 20 + 1, new string[] { 
            "나도 이 마을을 떠나서 여행을 하고 싶어.", 
            "그래도 역시 모험은 무서워..." });
        talkData.Add(100 + 20 + 2, new string[] {
            "가끔 숲에 들어가서 먹을 걸 채집해오곤 해.",
            "깊이 들어가지는 않아."});
        talkData.Add(100 + 20 + 3, new string[] {
            "나는 아주 어렸을 때부터 여기에 살았어.",
            "여기에서 태어나지는 않았지만 여긴 내 고향이나 마찬가지야."});

    }

    public string GetTalk(int id, int talkIndex)
    {
        if (!talkData.ContainsKey(id))  // 데이터가 존재하는지 검사
        {

            if (!talkData.ContainsKey(id - id % 10))
            {
                // 퀘스트 맨 처음 대사마저 없는 경우, 기본 대사를 가지고 옴
                if (talkIndex == talkData[id - id % 100].Length)
                {
                    return null;
                }
                else
                {
                    return talkData[id - id % 100][talkIndex];
                }
            }
            else
            {
                // 해당 퀘스트 진행 순서 대사가 없을 때, 퀘스트 맨 처음 대사를 가지고 옴
                if (talkIndex == talkData[id - id % 10].Length)  // ID가 없으면 퀘스트 대화순서 제거 후 재탐색
                {
                    return null;
                }
                else
                {
                    return talkData[id - id % 10][talkIndex];
                }
            }
        }

        if (talkIndex == talkData[id].Length)
        {
            return null;
        }
        else
        {
            return talkData[id][talkIndex];
        }
    }
}
