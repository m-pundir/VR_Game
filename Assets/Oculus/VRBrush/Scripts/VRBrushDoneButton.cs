using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRBrushDoneButton : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private VRControllerTriggerValue VRController;
    private BrushManager m_BrushManager;

    private Button btn;
    private bool isPressButton = false;

    private void Awake()
    {
        isPressButton = false;

        VRController = FindObjectOfType<VRControllerTriggerValue>();
        m_BrushManager = VRController.gameObject.GetComponent<BrushManager>();
        btn = this.GetComponent<Button>();
    }

    private void Update()
    {
        if (m_BrushManager.CanDrawing == true)
            return;

        ray = new Ray(VRController.transform.position, VRController.transform.forward);

        if (this.GetComponent<BoxCollider>().Raycast(ray, out hit, 50f))
        {
            btn.image.color = Color.yellow;
            if (VRController.GetTriggerValue() > 0.1f)
            {
                isPressButton = true;
                btn.image.color = Color.green;
            }
            else if (isPressButton)
            {
                isPressButton = false;
                m_BrushManager.CreateBrushStackParent_B();
                m_BrushManager.CreateBrushStackParent_C();
                btn.image.color = Color.white;
            }
        }
        else
        {
            isPressButton = false;
            btn.image.color = Color.white;
        }
    }
}
