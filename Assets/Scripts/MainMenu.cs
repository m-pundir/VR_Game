using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : UIManager
{
    [SerializeField] GameObject menuPannel;
    [SerializeField] GameObject[] demoObjects;
    [SerializeField] GameObject centerEyeAnchor;

    // left hand colour picker
    [SerializeField] GameObject leftHand;

    // right hand brush contoller
    [SerializeField] GameObject rightHand;
    public Vector3 menuOffSetFromCamera;
    void Start()
    {
        // function inherited from the UIManager Class
        SetVisibility(menuPannel, true);

        // hide the colour picker Ui
        leftHand.SetActive(false);

        rightHand.GetComponent<Quest_BrushController>().enabled = false;

        // i dont know if we want to keep the Demo objects in the game but if we do i havent deleted them
        for (int i = 0; i < demoObjects.Length; i++)
        {
            SetVisibility(demoObjects[i], false);
        }
    }

    void Update()
    {
        // Make the pannel update its position so that the user doesnt lose the menu
        menuPannel.transform.position = centerEyeAnchor.transform.position + menuOffSetFromCamera;
    }
}
