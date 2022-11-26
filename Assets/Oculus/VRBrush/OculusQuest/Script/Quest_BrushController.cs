using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_BrushController : MonoBehaviour
{
    private VRControllerTriggerValue m_Trigger;
    private BrushManager m_BrushManager;
    private LineRenderer rayRenderer;

    public Transform tfBrushPoint; //브러쉬 드로잉 포인트 위치

    private GameObject collidingObject;
    private GameObject objectInHand;

    //public GameObject brushPoint;
    private void Awake()
    {
        rayRenderer = this.GetComponent<LineRenderer>();
        if (rayRenderer)
        {
            rayRenderer.startWidth = 0.001f;
            rayRenderer.endWidth = 0.001f;
        }

        GameObject goBrushPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        goBrushPoint.name = "BrushPoint";
        goBrushPoint.transform.SetParent(this.transform);
        goBrushPoint.GetComponent<SphereCollider>().enabled = false;
        tfBrushPoint = goBrushPoint.transform;

        goBrushPoint.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        goBrushPoint.transform.localPosition = new Vector3(0f, 0f, 0.0625f);

        //brushPoint.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        //brushPoint.transform.localPosition = new Vector3(0f, 0f, 0.0625f);

        m_Trigger = this.GetComponent<VRControllerTriggerValue>();
        m_BrushManager = this.GetComponent<BrushManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Oculus
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, this.transform.forward);

        int layerMask = 1 << LayerMask.NameToLayer("UI");

        m_Trigger.SetTriggerValue(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger));

        if (Physics.Raycast(ray, out hit, 50f, layerMask))
        {
            rayRenderer.enabled = true;
            rayRenderer.SetPosition(0, this.transform.position);
            rayRenderer.SetPosition(1, hit.point);
        }
        else
        {
            rayRenderer.enabled = false;

            if (m_Trigger.GetTriggerValue() > 0.1f)
            {
                if (collidingObject)
                {
                    GrabObject();
                }
                else
                {
                    m_BrushManager.UndoStackClear();

                    float brushRadius = m_BrushManager.GetRadius() * m_Trigger.GetTriggerValue();
                    m_BrushManager.DrawingBrush(tfBrushPoint.position, brushRadius);
                }
            }
            else
            {
                if (objectInHand)
                {
                    ReleaseObject();
                }
                else
                {
                    m_BrushManager.EndBrush();
                    m_BrushManager.CanDrawing = false;
                }
            }
        }

        Vector2 secondaryThumbstickValue = Vector2.zero;

        secondaryThumbstickValue = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        if (secondaryThumbstickValue.x > 0)
        {
            m_BrushManager.RadiusSizeUp();

            float r = m_BrushManager.GetRadius();
            tfBrushPoint.transform.localScale = new Vector3(r * 2, r * 2, r * 2);
        }
        else if (secondaryThumbstickValue.x < 0)
        {
            m_BrushManager.RadiusSizeDown();

            float r = m_BrushManager.GetRadius();
            tfBrushPoint.transform.localScale = new Vector3(r * 2, r * 2, r * 2);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        collidingObject = col.gameObject;
    }

    private void GrabObject()
    {
        if (objectInHand)
            return;

        objectInHand = collidingObject;
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;

        return fx;
    }

    private void ReleaseObject()
    {
        if (this.GetComponent<FixedJoint>())
        {
            this.GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            objectInHand.GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RHand);
            objectInHand.GetComponent<Rigidbody>().angularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RHand);
        }

        objectInHand = null;
    }
}
