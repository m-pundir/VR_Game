using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class VRUIManager : MonoBehaviour
{
    public GameObject NextSceneButton;
    public GameObject QuitButton;

    public GameObject WelcomeText;
    public GameObject InstructionText;


    public TMPro.TextMeshProUGUI sceneName;

    public GameObject[] canvas;
    public VRScenes currentScene;

    private void Start()
    {
        NextSceneButton.GetComponent<VRUIButton>().SetCallBack(VRGameManager.instance.LoadScene);
        QuitButton.GetComponent<VRUIButton>().SetCallBack(VRGameManager.instance.QuitGame);

        sceneName.text = SceneManager.GetActiveScene().name;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == VRScenes.VRMainMenu.ToString())
        {
            currentScene = VRScenes.VRMainMenu;
            InstructionText.SetActive(false);
            WelcomeText.SetActive(true);
            NextSceneButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Start";

            VRGameManager.instance.SetGameState(GameState.MainMenu);

        }
        if (scene.name == VRScenes.Instructions.ToString())
        {
            currentScene = VRScenes.Instructions;
            InstructionText.SetActive(true);
            WelcomeText.SetActive(false);
            NextSceneButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Play";

            VRGameManager.instance.SetGameState(GameState.ShowInstructions);
        }
        if (scene.name == VRScenes.GamePlay.ToString())
        {
            currentScene = VRScenes.GamePlay;
            canvas[0].SetActive(false);
        }
        if (scene.name == VRScenes.Results.ToString())
        {
            currentScene = VRScenes.Results;
        }
        sceneName.text = currentScene.ToString();

        VRUIButton[] buttons = FindObjectsOfType<VRUIButton>();
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Initialise();
        }

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}