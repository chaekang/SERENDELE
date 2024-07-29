using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text nickName;
    [SerializeField] TMP_InputField input_RoomName;
    [SerializeField] TMP_InputField input_MaxPlayer;
    [SerializeField] Button btn_CreateRoom;
    [SerializeField] Button btn_JoinRoom;
    [SerializeField] GameObject roomListItem;
    public Transform rtContent;

    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        nickName.text = PhotonNetwork.LocalPlayer.NickName;
        input_RoomName.onValueChanged.AddListener(OnNameValueChanged);
        input_MaxPlayer.onValueChanged.AddListener(OnPlayerValueChange);
        btn_CreateRoom.onClick.AddListener(OnClickCreateRoom);
        btn_JoinRoom.onClick.AddListener(OnClickJoinRoom);
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
        btn_CreateRoom.interactable = input_RoomName.text.Length > 0 && input_MaxPlayer.text.Length > 0;
    }

    void OnPlayerValueChange(string s)
    {
        btn_CreateRoom.interactable = s.Length > 0 && input_RoomName.text.Length > 0;
    }

    public void OnClickCreateRoom()
    {
        int maxPlayers = int.Parse(input_MaxPlayer.text);
        if (maxPlayers < 1 || maxPlayers > 5)
        {
            Debug.LogError("Max Players must be between 1 and 5");
            return;
        }

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
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Failed to join room: " + message);
    }
}
