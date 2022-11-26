//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Valve.VR;

//public class Vive_BrushController : MonoBehaviour
//{
//    private VRControllerTriggerValue m_Trigger;
//    private BrushManager m_BrushManager;
//    private LineRenderer rayRenderer;

//    public Transform tfBrushPoint; //브러쉬 드로잉 포인트 위치

//    //vive
//    public SteamVR_Behaviour_Pose m_ControllerPose;
//    public SteamVR_Action_Boolean m_TrackPadClick;
//    public SteamVR_Action_Single m_Squeeze;
//    public SteamVR_Action_Boolean m_SnapTurnRight;
//    public SteamVR_Action_Boolean m_SnapTurnLeft;

//    private GameObject collidingObject;
//    private GameObject objectInHand;

//    private void Awake()
//    {
        
//        rayRenderer = this.GetComponent<LineRenderer>();
//        if (rayRenderer)
//        {
//            rayRenderer.startWidth = 0.001f;
//            rayRenderer.endWidth = 0.001f;
//        }

//        GameObject goBrushPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        goBrushPoint.name = "BrushPoint";
//        goBrushPoint.transform.SetParent(this.transform);
//        goBrushPoint.GetComponent<SphereCollider>().enabled = false;
//        tfBrushPoint = goBrushPoint.transform;

//        goBrushPoint.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
//        goBrushPoint.transform.localPosition = new Vector3(0f, 0f, 0.0625f);

//        m_Trigger = this.GetComponent<VRControllerTriggerValue>();
//        m_BrushManager = this.GetComponent<BrushManager>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //Vive
//        RaycastHit hit;
//        Ray ray = new Ray(this.transform.position, this.transform.forward);

//        int layerMask = 1 << LayerMask.NameToLayer("UI");

//        m_Trigger.SetTriggerValue(m_Squeeze.GetAxis(SteamVR_Input_Sources.RightHand));

//        if (Physics.Raycast(ray, out hit, 50f, layerMask) && !m_BrushManager.CanDrawing)
//        {
//            rayRenderer.enabled = true;
//            rayRenderer.SetPosition(0, this.transform.position);
//            rayRenderer.SetPosition(1, hit.point);
//        }
//        else
//        {
//            rayRenderer.enabled = false;
//            if (m_BrushManager.ColorPickerUIUsing == false)
//            {

//                if (m_Trigger.GetTriggerValue() > 0.1f)
//                {
//                    if (collidingObject)
//                    {
//                        GrabObject();
//                    }
//                    else
//                    {
//                        m_BrushManager.UndoStackClear();

//                        float brushRadius = m_BrushManager.GetRadius() * m_Trigger.GetTriggerValue();
//                        m_BrushManager.DrawingBrush(tfBrushPoint.position, brushRadius);
//                    }
//                }
//                else
//                {
//                    if (objectInHand)
//                    {
//                        ReleaseObject();
//                    }
//                    else
//                    {
//                        m_BrushManager.EndBrush();
//                        m_BrushManager.CanDrawing = false;
//                    }
//                }
//            }
//        }

//        Vector2 secondaryThumbstickValue = Vector2.zero;


//        if (m_SnapTurnRight.GetState(SteamVR_Input_Sources.RightHand))
//            secondaryThumbstickValue = new Vector2(1f, 0);
//        else if (m_SnapTurnLeft.GetState(SteamVR_Input_Sources.RightHand))
//            secondaryThumbstickValue = new Vector2(-1f, 0);


//        if (secondaryThumbstickValue.x > 0)
//        {
//            m_BrushManager.RadiusSizeUp();

//            float r = m_BrushManager.GetRadius();
//            tfBrushPoint.transform.localScale = new Vector3(r * 2, r * 2, r * 2);
//        }
//        else if (secondaryThumbstickValue.x < 0)
//        {
//            m_BrushManager.RadiusSizeDown();

//            float r = m_BrushManager.GetRadius();
//            tfBrushPoint.transform.localScale = new Vector3(r * 2, r * 2, r * 2);
//        }
//    }

//    public void OnTriggerEnter(Collider other)
//    {
//        SetCollidingObject(other);
//    }

//    public void OnTriggerStay(Collider other)
//    {
//        SetCollidingObject(other);
//    }

//    public void OnTriggerExit(Collider other)
//    {
//        if(!collidingObject)
//        {
//            return;
//        }

//        collidingObject = null;
//    }

//    private void SetCollidingObject(Collider col)
//    {
//        if(collidingObject || !col.GetComponent<Rigidbody>())
//        {
//            return;
//        }

//        Debug.Log(col.name);
//        collidingObject = col.gameObject;
//    }

//    private void GrabObject()
//    {
//        if (objectInHand)
//            return;

//        objectInHand = collidingObject;
//        collidingObject = null;

//        var joint = AddFixedJoint();
//        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
//    }

//    private FixedJoint AddFixedJoint()
//    {
//        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
//        fx.breakForce = 20000;
//        fx.breakTorque = 20000;

//        return fx;
//    }

//    private void ReleaseObject()
//    {
//        if (this.GetComponent<FixedJoint>())
//        {
//            this.GetComponent<FixedJoint>().connectedBody = null;
//            Destroy(GetComponent<FixedJoint>());
//            objectInHand.GetComponent<Rigidbody>().velocity = m_ControllerPose.GetVelocity();
//            objectInHand.GetComponent<Rigidbody>().angularVelocity = m_ControllerPose.GetAngularVelocity();
//        }

//        objectInHand = null;
//    }
//}