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

    private BrushManager brushManager;

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
        Team1Drawings = new List<GameObject>();
        Team2Drawings = new List<GameObject>();
        //onGameStateChanged += VRGameManager_onGameStateChanged;
    }

    private void VRGameManager_onGameStateChanged(GameState obj)
    {
        VRUIManager.instance.UpdateMessage();
    }

    public GameState GetGameState()
    {
        return currentGameState;
    }

    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        VRUIManager.instance.UpdateMessage();
        //onGameStateChanged?.Invoke(currentGameState);
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
            
            //On entering the scene, its always team 1 chance first
            SetGameState(GameState.TeamOneDrawing);
        }
        if (scene.name == VRScenes.Results.ToString())
        {
            currentScene = VRScenes.Results;
            nextScene = VRScenes.VRMainMenu;
        }

        onSceneChanged?.Invoke(currentScene);
    }

    public void BrushManager_onBrushSaved(GameObject obj)
    {
        if (VRGameManager.instance.GetGameState() == GameState.TeamOneDrawing)
        {
            Team1Drawings.Add(obj);
        }
        if (VRGameManager.instance.GetGameState() == GameState.TeamTwoDrawing)
        {
            Team2Drawings.Add(obj);
        }
        obj.SetActive(false);
        VRUIManager.instance.UpdateMessage();
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