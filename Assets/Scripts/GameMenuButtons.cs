using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameMenuButtons : MonoBehaviour
{
    public AudioSource buttonSound;
    public void returnToMainMenu()
    {

        StartCoroutine(delaytransition());
        buttonSound.Play();
    }

    public void NextTurn()
    {
        buttonSound.Play();     
    }

    IEnumerator delaytransition()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
