using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SatTexture : MonoBehaviour
{
    private Texture2D satvalTex;
    private Color[] satvalColors;
    
    private Vector3 vSatValue;
    public GameObject satKnob;
    public Transform satKnobMax;

    void Start()
    {
        satvalColors = new Color[] {
            new Color( 0, 0, 0 ),
            new Color( 0, 0, 0 ),
            new Color( 1, 1, 1 ),
            Color.red,
        };

        satvalTex = new Texture2D(2, 2);
        this.GetComponent<Image>().sprite = Sprite.Create(satvalTex, new Rect(0.5f, 0.5f, 1, 1), new Vector2(0.5f, 0.5f));
        ResetSatValTexture();

    }

    // Update is called once per frame
    void Update()
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
                vSatValue = Vector3.one - (this.GetComponent<Collider>().bounds.max - satKnob.transform.position) / this.GetComponent<Collider>().bounds.size.y;

                this.GetComponentInParent<VRColorPicker>().SetSat(vSatValue);
            }
        }
        */
    }

    public void SatUpdate(Vector3 _point)
    {
        Vector3 point = this.GetComponent<Collider>().ClosestPointOnBounds(_point);
        SetKnobPosition(point);
        vSatValue = Vector3.one - (satKnobMax.localPosition - satKnob.transform.localPosition) /
            this.GetComponent<BoxCollider>().size.y;

        this.GetComponentInParent<VRColorPicker>().SetSat(vSatValue);
    }

    private void ResetSatValTexture()
    {
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 2; i++)
            {
                satvalTex.SetPixel(i, j, satvalColors[i + j * 2]);
            }
        }
        satvalTex.Apply();
    }

    void SetKnobPosition(Vector3 point)
    {
        Vector3 temp = satKnob.transform.localPosition;
        satKnob.transform.position = point;

        satKnob.transform.localPosition = new Vector3(satKnob.transform.localPosition.x,
            satKnob.transform.localPosition.y, temp.z);
    }

    public void SetsatvalColors(Vector3 vHueValue)
    {
        satvalColors[3] = Color.HSVToRGB(vHueValue.y, 1f, 1f);
        ResetSatValTexture();
    }
}
