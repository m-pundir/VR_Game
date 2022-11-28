using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuTrigger : MonoBehaviour
{
    public GameObject gameMenu;
    public bool gameIsPaused = false;
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
    }

    void Resume()
    {
        gameMenu.SetActive(false);
        gameIsPaused = false;
    }
}
