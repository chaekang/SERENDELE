using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject managerGroup;
    [SerializeField] GameObject serverUI; // UI ĵ����
    [SerializeField] TMP_Text dayAndNight_text;
    [SerializeField] GameObject playerInfo;

    // �� ���� �� ���� ���� UI
    [Header("Lobby UI")]
    [SerializeField] TMP_Text nickName;
    [SerializeField] TMP_InputField input_RoomName;
    [SerializeField] Button btn_CreateRoom;
    [SerializeField] Button btn_JoinRoom;
    [SerializeField] GameObject roomListItem;

    // ĳ���� ���� ���� UI
    [SerializeField] Button btn_Arie;
    [SerializeField] GameObject infoArie;

    [SerializeField] Button btn_Lembra;
    [SerializeField] GameObject infoLembra;

    [Header("Selected Character")]
    [SerializeField] bool Lembra;
    [SerializeField] bool Arie;


    [Header("Warning Message")]
    [SerializeField] Image warningMsg;
    [SerializeField] TMP_Text warningText;



    public Transform rtContent;

    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        //nickName.text = PhotonNetwork.LocalPlayer.NickName;
        input_RoomName.onValueChanged.AddListener(OnNameValueChanged);
        btn_CreateRoom.onClick.AddListener(OnClickCreateRoom);
        btn_JoinRoom.onClick.AddListener(OnClickJoinRoom);

        infoArie.SetActive(false);
        infoLembra.SetActive(false);
        btn_Arie.onClick.AddListener(ClickArie);
        btn_Lembra.onClick.AddListener(ClickLembra);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        DeleteRoomListItem();
        UpdateRoomListItem(roomList);
        CreateRoomListItem();
    }

    void SelectRoomItem(string roomName)
    {
        input_RoomName.text = roomName;
    }

    void DeleteRoomListItem()
    {
        foreach (Transform tr in rtContent)
        {
            Destroy(tr.gameObject);
        }
    }

    void UpdateRoomListItem(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (dicRoomInfo.ContainsKey(info.Name))
            {
                if (info.RemovedFromList)
                {
                    dicRoomInfo.Remove(info.Name);
                    continue;
                }
            }
            dicRoomInfo[info.Name] = info;
        }
    }

    void CreateRoomListItem()
    {
        foreach (RoomInfo info in dicRoomInfo.Values)
        {
            GameObject go = Instantiate(roomListItem, rtContent);
            RoomListItem item = go.GetComponent<RoomListItem>();
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers);
            item.onDelegate = SelectRoomItem;
        }
    }

    void OnNameValueChanged(string s)
    {
        btn_JoinRoom.interactable = s.Length > 0;
    }

    void OnPlayerValueChange(string s)
    {
        btn_CreateRoom.interactable = s.Length > 0 && input_RoomName.text.Length > 0;
    }

    public void OnClickCreateRoom()
    {
        int maxPlayers = 2;

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayers,
            IsVisible = true,
            IsOpen = true
        };

        if (!Lembra && !Arie)
        {
            StartCoroutine(WarningMsg("characterError"));
        }
        else if(input_RoomName.text == "")
        {
            StartCoroutine(WarningMsg("createRoomError"));
        }
        else
        {
            PhotonNetwork.CreateRoom(input_RoomName.text, options);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if(!Lembra && !Arie)
        {
            StartCoroutine(WarningMsg("characterError"));
        }
        else
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log("Room creation failed: " + message);
            StartCoroutine(WarningMsg("createRoomError"));
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Room created successfully");
    }

    public void OnClickJoinRoom()
    {
       
        if (!Lembra && !Arie)
        {
            StartCoroutine(WarningMsg("characterError"));
        }
        else if (input_RoomName.text == "")
        {
            StartCoroutine(WarningMsg("enterRoomError"));
        }
        else
        {
            PhotonNetwork.JoinRoom(input_RoomName.text);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined room successfully");
        if (Arie)
        {
            GameObject Arie = PhotonNetwork.Instantiate("Arie", Vector3.zero, Quaternion.identity); 
        }
        else if (Lembra)
        {
            GameObject Lembra = PhotonNetwork.Instantiate("Lembra", Vector3.zero, Quaternion.identity);
        }

        serverUI.SetActive(false);
        managerGroup.SetActive(true);
        dayAndNight_text.gameObject.SetActive(true);
        playerInfo.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (!Lembra && !Arie)
        {
            StartCoroutine(WarningMsg("characterError"));
        }
        else
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.Log("Failed to join room: " + message);
            StartCoroutine(WarningMsg("enterRoomError"));
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has entered the room.");

    }

    void ClickArie()
    {
        Arie = true;
        Lembra = false;
        infoArie.SetActive(true);
        infoLembra.SetActive(false);
    }

    void ClickLembra()
    {
        Arie = false;
        Lembra = true;
        infoArie.SetActive(false);
        infoLembra.SetActive(true);
    }

    IEnumerator WarningMsg(string msg)
    {
        //characterError   ĳ���� �������� �ʰ� ��ư ���� ��
        //createRoomError  �� �̸� ���� �ʰ� Create ��ư ���� �� 
        //enterRoomError   �� �������� �ʰ� Enter ��ư ���� ��
        switch (msg)
        {
            case "characterError":
                warningText.text = "Please select a character!";
                break;
            case "createRoomError":
                warningText.text = "Please enter a room name!";
                break;
            case "enterRoomError":
                warningText.text = "Please select a room!";
                break;
            default:
                warningText.text = "Unknown warning!";
                break;
        }

        warningMsg.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        warningMsg.gameObject.SetActive(false);  
    }
}
