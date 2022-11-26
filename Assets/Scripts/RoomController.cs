using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviourPunCallbacks
{

    [SerializeField] int multiplayerSceneIndex; // number for the build index to game screen

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        base.OnEnable();
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        base.OnDisable();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        StartGame();
        base.OnJoinedRoom();
    }

    private void StartGame()
    {
        if(PhotonNetwork.IsMasterClient) // is the client the host
        {
            Debug.Log("Starting game");
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }

}
