using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public void LoadNextScene()
    {
        if (VRGameManager.instance != null)
        {
            VRGameManager.instance.LoadScene();
        }
    }
}
