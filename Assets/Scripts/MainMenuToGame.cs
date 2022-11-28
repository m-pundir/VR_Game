using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuToGame : MonoBehaviour
{
    public AudioSource buttonSound;
    public void moveToGameScreen()
    {
        StartCoroutine(delaytransition());
        buttonSound.Play();
    }

    public void quitGame()
    {
        buttonSound.Play();
        Application.Quit();
    }

    IEnumerator delaytransition()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
