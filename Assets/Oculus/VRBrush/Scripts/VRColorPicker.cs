using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRColorPicker : MonoBehaviour
{
    public GameObject hue;
    public GameObject sat;
    public GameObject result;
    public Material DefaultBrushMat;
    public Material BloomBrushMat;

    public Vector3 hsvColor;

    private Ray ray;
    private RaycastHit hit;
    private bool m_hueUsing = false;
    private bool m_satUsing = false;

    public GameObject VRController;
    private VRControllerTriggerValue m_ControllerTrigger;
    private BrushManager m_BrushManager;

    public TMPro.TextMeshProUGUI informationOne;
    public TMPro.TextMeshProUGUI informationTwo;

    private void Awake()
    {
        m_ControllerTrigger = VRController.GetComponent<VRControllerTriggerValue>();
        m_BrushManager = VRController.GetComponent<BrushManager>();

        hsvColor = Vector3.one;
        ApplyColor();
    }

    private void Update()
    {
        if (m_BrushManager.CanDrawing == true)
            return;

        ray = new Ray(VRController.transform.position, VRController.transform.forward);

        int layerMask = 1 << LayerMask.NameToLayer("UI");

        if (Physics.Raycast(ray, out hit, 50f, layerMask))
        {
            if (m_ControllerTrigger.GetTriggerValue() > 0.1f)
            {
                m_BrushManager.ColorPickerUIUsing = true;
                if (hit.transform.GetComponent<HueTexture>() && !m_satUsing)
                {
                    m_hueUsing = true;
                    HueTexture hue = hit.transform.GetComponent<HueTexture>();
                    hue.HueUpdate(hit.point);
                }
                else if (hit.transform.GetComponent<SatTexture>() && !m_hueUsing)
                {
                    m_satUsing = true;
                    SatTexture sat = hit.transform.GetComponent<SatTexture>();
                    sat.SatUpdate(hit.point);
                }
                else if (hit.transform.GetComponent<BrushRadiusSliderUI>())
                {
                    hit.transform.GetComponent<BrushRadiusSliderUI>().RadiusUpdate();
                }
            }
            else
            {
                m_hueUsing = false;
                m_satUsing = false;
                m_BrushManager.ColorPickerUIUsing = false;
            }
        }
        else
        {
            if(m_ControllerTrigger.GetTriggerValue() < 0.1f)
            {
                m_hueUsing = false;
                m_satUsing = false;
                m_BrushManager.ColorPickerUIUsing = false;
            }
        }
    }

    public void SetHue(Vector3 hue)
    {
        double hValue = System.Math.Round(hue.y, 2);
        Vector3 vHueValue = new Vector3(hue.x, (float)hValue, hue.z);
        hsvColor.x = vHueValue.y;
        sat.GetComponent<SatTexture>().SetsatvalColors(vHueValue);
        ApplyColor();
    }

    public void SetSat(Vector3 sat)
    {
        hsvColor.y = sat.x;
        hsvColor.z = sat.y;
        ApplyColor();
    }

    private void ApplyColor()
    {
        Color resultColor = Color.HSVToRGB(hsvColor.x, hsvColor.y, hsvColor.z);
        if (m_BrushManager.m_useBloom)
        {
            result.GetComponent<Image>().material.color = resultColor;
            result.GetComponent<Image>().material.SetColor("_EmissionColor", resultColor * 2f);
        }
        else
        {
            result.GetComponent<Image>().material.color = resultColor;
            result.GetComponent<Image>().material.SetColor("_EmissionColor", Color.black * 0f);
        }
        //result.GetComponent<Image>().color = resultColor;
        
        DefaultBrushMat.color = resultColor;

        BloomBrushMat.color = resultColor;
        BloomBrushMat.SetColor("_EmissionColor", resultColor * 2f);
    }
}
