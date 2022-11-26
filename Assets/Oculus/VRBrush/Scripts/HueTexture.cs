using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HueTexture : MonoBehaviour
{
    private Texture2D hueTex;
    private Color[] hueColors;
    private Vector3 vHueValue;

    public GameObject hueKnob;
    public Transform hueKnobMax;

    private void Awake()
    {
        Setup();
    }

    private void Update()
    {
        /*
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
            if (this.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 point = this.GetComponent<Collider>().ClosestPointOnBounds(hit.point);
                SetKnobPosition(point);
                vHueValue = Vector3.one - (this.GetComponent<Collider>().bounds.max - hueKnob.transform.position) / this.GetComponent<Collider>().bounds.size.y;

                this.GetComponentInParent<VRColorPicker>().SetHue(vHueValue);
            }
        }
        */
        //hueKnob.transform.localPosition = new Vector3(hueKnob.transform.localPosition.x, vTest.y, hueKnob.transform.localPosition.z);
    }

    public void HueUpdate(Vector3 _point)
    {
        //Vector3 point = this.GetComponent<Collider>().ClosestPointOnBounds(_point);
        //Vector3 point = this.transform.position + this.GetComponent<Collider>().bounds.max + _point;
        SetKnobPosition(_point);
        //vHueValue = Vector3.one - (this.GetComponent<Collider>().bounds.max - hueKnob.transform.position) / this.GetComponent<Collider>().bounds.size.y;
        vHueValue = Vector3.one - (hueKnobMax.localPosition - hueKnob.transform.localPosition) / 
            this.GetComponent<BoxCollider>().size.y;

        this.GetComponentInParent<VRColorPicker>().SetHue(vHueValue);
    }

    void SetKnobPosition(Vector3 point)
    {
        Vector3 temp = hueKnob.transform.localPosition;
        hueKnob.transform.position = point;
        hueKnob.transform.localPosition = new Vector3(temp.x,
            Mathf.Clamp(hueKnob.transform.localPosition.y, 0f, 200f), temp.z) ;
    }

    private void Setup()
    {
        hueColors = new Color[]
        {
            Color.red,
            Color.yellow,
            Color.green,
            Color.cyan,
            Color.blue,
            Color.magenta,
        };


        hueTex = new Texture2D(1, 7);
        for (int i = 0; i < 7; i++)
        {
            hueTex.SetPixel(0, i, hueColors[i % 6]);
        }
        hueTex.Apply();
        this.GetComponent<Image>().sprite = Sprite.Create(hueTex, new Rect(0, 0.5f, 1, 6), new Vector2(0.5f, 0.5f));
    }
}
