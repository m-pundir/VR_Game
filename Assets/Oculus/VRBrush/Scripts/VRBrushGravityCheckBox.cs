using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VRBrushGravityCheckBox : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private VRControllerTriggerValue VRController;
    private BrushManager m_BrushManager;

    private bool isPressButton = false;

    private void Awake()
    {
        isPressButton = false;

        VRController = FindObjectOfType<VRControllerTriggerValue>();
        m_BrushManager = VRController.gameObject.GetComponent<BrushManager>();

        ray = new Ray(VRController.transform.position, VRController.transform.forward);
    }


    void Update()
    {
        if (m_BrushManager.CanDrawing == true)
            return;

        ray.origin = VRController.transform.position;
        ray.direction = VRController.transform.forward;

        if (this.GetComponent<BoxCollider>().Raycast(ray, out hit, 50f))
        {
            if (VRController.GetTriggerValue() > 0.1f)
            {
                isPressButton = true;
            }
            else if (isPressButton)
            {
                isPressButton = false;
                //gravity on off
                Toggle toggle = this.GetComponent<Toggle>();
                if(toggle.isOn)
                {
                    toggle.isOn = false;
                    m_BrushManager.m_UseGravity = false;
                }
                else
                {
                    toggle.isOn = true;
                    m_BrushManager.m_UseGravity = true;
                }
            }
        }
        else
        {
            isPressButton = false;
        }
    }
}
