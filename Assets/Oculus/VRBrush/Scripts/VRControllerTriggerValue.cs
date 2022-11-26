using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControllerTriggerValue : SingleTon<VRControllerTriggerValue>
{
    public float m_TriggerValue;

    public void SetTriggerValue(float value)
    {
        m_TriggerValue = value;
    }

    public float GetTriggerValue()
    {
        return m_TriggerValue;
    }
}
