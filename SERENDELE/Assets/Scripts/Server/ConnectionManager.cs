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

    public AuthManager authManager;
    

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
        authManager = FindObjectOfType<AuthManager>();
        Debug.Log(authManager.IsLogin);
        if (authManager.IsLogin)
        {
            base.OnJoinedLobby();

            PhotonNetwork.LoadLevel("2. CreateServer");
            print("Enter Lobby");
        }
        else
        {
            Debug.Log("Login Failed. Cant Enter Lobby");

            // 다시 포톤 연결
            PhotonNetwork.Disconnect();  
            StartCoroutine(Reconnect());
        }
    }

    IEnumerator Reconnect()
    {
        Debug.Log("...Reconnecting to Photon");
        while (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();
            yield return new WaitForSeconds(1f); 
        }
    }

    public void OnClickConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
}
