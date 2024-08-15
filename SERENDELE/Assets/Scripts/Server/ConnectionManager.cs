using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_InputField inputNickname;
    [SerializeField]
    Button enterButton;

    void Start()
    {
        inputNickname.onValueChanged.AddListener(OnValueChanged);
        enterButton.onClick.AddListener(OnClickConnect);
    }

    void OnValueChanged(string s)
    {
        enterButton.interactable = s.Length > 0;
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Master Server Connection Success");

        PhotonNetwork.LocalPlayer.NickName = inputNickname.text;

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        PhotonNetwork.LoadLevel("2. CreateServer");
        print("Enter Lobby");
    }

    public void OnClickConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
}
