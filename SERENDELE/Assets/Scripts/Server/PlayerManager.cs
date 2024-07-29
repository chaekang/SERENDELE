using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject serverUI; // UI Äµ¹ö½º

    public override void OnJoinedRoom()
    {


        GameObject player = PhotonNetwork.Instantiate("Wizard", Vector3.zero, Quaternion.identity);
        
        if (serverUI != null)
        {
            serverUI.SetActive(false);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has entered the room.");
    }
}
