using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class InGameMenu : UIManager
{
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject centerEyeAnchor;
    public Vector3 menuOffSetFromCamera;

    void Start()
    {
        SetVisibility(gameMenu, false);
    }


    void Update()
    {
        gameMenu.transform.position = centerEyeAnchor.transform.position + menuOffSetFromCamera;
        //gameMenu.transform.rotation = centerEyeAnchor.transform.rotation;
    }
}
