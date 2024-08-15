using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject serverUI; // UI 캔버스

    // 룸 생성 및 입장 관련 UI
    [SerializeField] TMP_Text nickName;
    [SerializeField] TMP_InputField input_RoomName;
    [SerializeField] Button btn_CreateRoom;
    [SerializeField] Button btn_JoinRoom;
    [SerializeField] GameObject roomListItem;

    // 캐릭터 선택 관련 UI
    [SerializeField] Button btn_Arie;
    [SerializeField] GameObject infoArie;

    [SerializeField] Button btn_Lembra;
    [SerializeField] GameObject infoLembra;

    [SerializeField] bool Lembra;
    [SerializeField] bool Arie;

    // 경고창
    [SerializeField] Image warnningMsg;
    [SerializeField] TMP_Text chooseCha; // 캐릭터 선정하지 않고 버튼 누를 시
    [SerializeField] TMP_Text inputTxt;  // 방 이름 적지 않고 Create 버튼 누를 시 
    [SerializeField] TMP_Text clickList; // 방 선택하지 않고 Enter 버튼 누를 시



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

        PhotonNetwork.CreateRoom(input_RoomName.text, options);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Room creation failed: " + message);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Room created successfully");
    }

    public void OnClickJoinRoom()
    {
        PhotonNetwork.JoinRoom(input_RoomName.text);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined room successfully");
        GameObject player = PhotonNetwork.Instantiate("Wizard", Vector3.zero, Quaternion.identity);

        if (serverUI != null)
        {
            serverUI.SetActive(false);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Failed to join room: " + message);
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
}
