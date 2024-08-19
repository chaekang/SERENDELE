using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TalkManager : MonoBehaviour
{
    public static TalkManager Instance;
    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            //DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion Singleton

    public Line lineManager;
    public GameObject talkPanel;       // 게임 대화창
    public TextMeshProUGUI talkText;   // 게임창에 뜨는 텍스트
    public TextMeshProUGUI nameText;   // NPC 이름
    public GameObject scanNPC;         // 조사한 NPC
    public bool isAction;              // 상태저장용 변수
    public int talkIndex;

    private string currentTalks;     // 현재 출력 중인 대사 리스트

    public void Action(GameObject scanObj)
    {
        scanNPC = scanObj;
        ObjectData objData = scanObj.GetComponent<ObjectData>();

        // NPC 대사 초기화
        currentTalks = lineManager.GetTalk(objData.Id, objData.index, talkIndex);

        // 대화 시작
        Talk();
    }

    void Talk()
    {
        if (currentTalks == null)
        {
            isAction = false;
            talkIndex = 0;
            talkPanel.SetActive(false);  // 대화창 닫기
            return;
        }

        talkPanel.SetActive(true);
        talkText.text = currentTalks;
        nameText.text = scanNPC.GetComponent<ObjectData>().Name;

        isAction = true;

        talkIndex++;
    }
}
