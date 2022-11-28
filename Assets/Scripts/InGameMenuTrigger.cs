using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuTrigger : MonoBehaviour
{
    public GameObject gameMenu;
    public bool gameIsPaused = false;

    // left hand colour picker
    [SerializeField] GameObject leftHand;

    // right hand brush contoller
    [SerializeField] GameObject rightHand;


    // Start is called before the first frame update
    void Start()
    {
        gameMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.One))
        {
            if(gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
            
        }
    }

    void Pause()
    {
        gameMenu.SetActive(true);
        gameIsPaused = true;

        // hide the colour picker Ui
        leftHand.SetActive(false);
        rightHand.GetComponent<Quest_BrushController>().enabled = false;

    }

    void Resume()
    {
        gameMenu.SetActive(false);
        gameIsPaused = false;

        leftHand.SetActive(true);
        rightHand.GetComponent<Quest_BrushController>().enabled = true;
    }
}
