using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject startButton;
    [SerializeField] GameObject cancelButton;
    [SerializeField] int roomSize;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        startButton.SetActive(true);
        base.OnConnectedToMaster();
    }

    public void ButtonStart() // this functions will be called when the start button is clicked
    {
        startButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Start");
    }

    public void ButtonCancel() // this functions will be called when the cancel button is clicked
    {
        cancelButton.SetActive(false);
        startButton.SetActive(true);
        PhotonNetwork.LeaveRoom(); // if we click cancel then we want to leave the room that it is either creating or about to join us to
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
        base.OnJoinRandomFailed(returnCode, message);
    }

    public void CreateRoom()
    {
        Debug.Log("Creating room now");
        int randomNumber = Random.Range(0, 10); // creates a random name for the room
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte) roomSize};

        PhotonNetwork.CreateRoom("Room" + randomNumber, roomOptions);
        Debug.Log(randomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a room"); // the room number might already exisit so it will need to be created again so that the room number isnt the same as an exisitin one
        CreateRoom();
        base.OnCreateRoomFailed(returnCode, message);
    }

}
