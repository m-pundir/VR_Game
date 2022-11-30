using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRGameManager : MonoBehaviour
{
    public static VRGameManager instance = null;
    [SerializeField] private GameState currentGameState;
    [SerializeField]private VRScenes currentScene;
    [SerializeField] private VRScenes nextScene;

    public static event Action<GameState> onGameStateChanged;
    public static event Action<VRScenes> onSceneChanged;

    private int maxSaveCountPerTeam = 2;

    public List<GameObject> Team1Drawings = new List<GameObject>();
    public List<GameObject> Team2Drawings = new List<GameObject>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        onGameStateChanged += VRGameManager_onGameStateChanged;
    }

    private void VRGameManager_onGameStateChanged(GameState obj)
    {
        
    }

    public GameState GetGameState()
    {
        return currentGameState;
    }

    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        onGameStateChanged?.Invoke(currentGameState);
    }


    public void LoadScene()
    {
        SceneManager.LoadScene(nextScene.ToString());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == VRScenes.VRMainMenu.ToString())
        {
            currentScene = VRScenes.VRMainMenu;
            nextScene = VRScenes.Instructions;

            VRGameManager.instance.SetGameState(GameState.MainMenu);

        }
        if (scene.name == VRScenes.Instructions.ToString())
        {
            currentScene = VRScenes.Instructions;
            nextScene = VRScenes.GamePlay;

            VRGameManager.instance.SetGameState(GameState.ShowInstructions);

        }
        if (scene.name == VRScenes.GamePlay.ToString())
        {
            currentScene = VRScenes.GamePlay;
            nextScene = VRScenes.Results;
        }
        if (scene.name == VRScenes.Results.ToString())
        {
            currentScene = VRScenes.Results;
            nextScene = VRScenes.VRMainMenu;
        }

        onSceneChanged?.Invoke(currentScene);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;        
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public VRScenes GetCurrentScene()
    {
        return currentScene;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

public enum GameState
{
    MainMenu,
    ShowInstructions,
    TeamOneDrawing,
    TeamTwoDrawing,
    Results
}

public enum VRScenes
{
    VRMainMenu,
    Instructions,
    GamePlay,
    Results
}