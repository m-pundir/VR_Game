using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBrushUndoButton : MonoBehaviour
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
    }

    private void Update()
    {
        if (m_BrushManager.CanDrawing == true)
            return;

        ray = new Ray(VRController.transform.position, VRController.transform.forward);

        if (this.GetComponent<BoxCollider>().Raycast(ray, out hit, 50f))
        {
            if (VRController.GetTriggerValue() > 0.1f)
            {
                isPressButton = true;
            }
            else if (isPressButton)
            {
                isPressButton = false;
                m_BrushManager.DrawingUndoFunction();
            }
        }
        else
        {
            isPressButton = false;
        }
    }
}
