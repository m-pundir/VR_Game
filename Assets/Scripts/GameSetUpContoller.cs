using Photon.Pun;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetUpContoller : MonoBehaviour
{

    //[SerializeField] GameObject player;
    void Start()
    {
        CreatePlayer();
    }


    private void CreatePlayer()
    {
        Debug.Log("Creating player");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Camera"), Vector3.zero, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Oculus_VRBrush"), Vector3.zero, Quaternion.identity);
        }
       
    }
}
