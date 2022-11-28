using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuTrigger : MonoBehaviour
{
    public GameObject gameMenu;
    // Start is called before the first frame update
    void Start()
    {
        gameMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Button.One))
        {
            gameMenu.SetActive(true);
        }
    }
}
