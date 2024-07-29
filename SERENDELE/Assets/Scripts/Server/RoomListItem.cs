using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public TMP_Text roomInfo;

    // 클릭되었을 때 호출되는 함수
    public Action<string> onDelegate;

    // InputField 참조를 저장하는 변수
    private TMP_InputField inputServerNameField;

    void Start()
    {
        // InputField를 동적으로 찾습니다
        GameObject go = GameObject.Find("InputServerName");
        if (go != null)
        {
            inputServerNameField = go.GetComponent<TMP_InputField>();
            Debug.Log("찾음" + inputServerNameField.name);
        }
        else
        {
            Debug.LogError("InputServerName GameObject를 찾을 수 없습니다.");
        }
    }

    public void SetInfo(string roomName, int currPlayer, int maxPlayer)
    {
        name = roomName;
        roomInfo.text = "  " + roomName + "  (" + currPlayer + '/' + maxPlayer + ")";
    }

    public void OnClick()
    {
        // 만약 onDelegate 에 무언가 들어있다면 실행
        if (onDelegate != null)
        {
            onDelegate(name);
        }
        Debug.Log("InputField에 텍스트 설정: " + name);
        inputServerNameField.text = name;
    }
}
