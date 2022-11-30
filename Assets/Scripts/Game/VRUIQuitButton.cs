using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(VRBrushUI))]
[RequireComponent(typeof(BoxCollider))]
public class VRUIQuitButton : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;

    private Button btn;
    private bool isPressButton = false;

    public Action callBack;
    float triggerValue = 0f;

    [SerializeField]private LaserPointer laserPointer;

    private void Awake()
    {
    }

    private void Start()
    {
        Initialise();
    }

    public void Initialise()
    {
        laserPointer = FindObjectOfType<LaserPointer>();

        isPressButton = false;
        btn = this.GetComponent<Button>();

        GetComponent<BoxCollider>().size = new Vector3(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height,
            20f);

    }


    public bool active = false;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == VRScenes.GamePlay.ToString()) { active = false; return; }
        active = true;
        triggerValue = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        ray = new Ray(laserPointer.GetStartPoint(), laserPointer.GetForwardPoint());

        if (this.GetComponent<BoxCollider>().Raycast(ray, out hit, 50000f))
        {
            btn.image.color = Color.yellow;
            if (triggerValue > 0.1f)
            {
                isPressButton = true;
                btn.image.color = Color.red;
            }
            else if (isPressButton)
            {
                Application.Quit();
                isPressButton = false;
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
