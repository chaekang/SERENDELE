using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TalkManager : MonoBehaviour
{
    public Line lineManager;
    public GameObject talkPanel;       // 게임 대화창
    public TextMeshProUGUI talkText;   // 게임창에 뜨는 텍스트
    public TextMeshProUGUI nameText;   // NPC 이름
    public GameObject scanNPC;         // 조사한 NPC
    public bool isAction;              // 상태저장용 변수
    public int talkIndex;

    public void Action(GameObject scanObj)
    {
        scanNPC = scanObj;
        ObjectData objData = scanObj.GetComponent<ObjectData>();
        Talk(objData.Id);
    }

    void Talk(int id)
    {
        string talkData = lineManager.GetTalk(id, talkIndex);

        if (talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            return;
        }

        talkText.text = talkData;
        nameText.text = ObjectData.Name;
        isAction = true;
        talkIndex++;
    }

    void Update()
    {
        // 마우스 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            // 클릭한 위치의 오브젝트 확인
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;
                // 클릭한 오브젝트에 ObjectData 스크립트가 있는지 확인
                ObjectData objData = clickedObject.GetComponent<ObjectData>();
                if (objData != null)
                {
                    Action(clickedObject);
                }
            }
        }
    }
}
