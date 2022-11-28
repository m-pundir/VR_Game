using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class triggerPainSound : MonoBehaviour
{
    public AudioSource paintSound;
    public bool playTheSound = false;
    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            if(!paintSound.isPlaying)
            {
                paintSound.Play();
            }
        }
        else
        {
            paintSound.Stop();
        }


    }
}
