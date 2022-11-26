using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VRDeviceSet : MonoBehaviour
{
    public GameObject m_Oculus;
    public GameObject m_Vive;

    private void Awake()
    {
        //Debug.Log(UnityEngine.XR.XRSettings.loadedDeviceName);
        if(UnityEngine.XR.XRSettings.loadedDeviceName == "oculus display")
        {
            m_Oculus.SetActive(true);
        }
        else
        {
            m_Vive.SetActive(true);
        }
    }
}
