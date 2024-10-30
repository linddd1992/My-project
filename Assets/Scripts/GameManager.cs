using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        LevelComplete
    }

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CurrentState = GameState.Playing;
    }

    public void StartGame(int number)
    {
        CurrentState = GameState.Playing;
        // LoadLevel(1);
        
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            Time.timeScale = 0;
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1;
        }
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        UIManager.Instance.OpenWindow("GameOverUI");
    }

    public void LevelComplete()
    {
        CurrentState = GameState.LevelComplete;
        UIManager.Instance.OpenWindow("PassView");
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(nextSceneIndex);
        }
        else
        {
            // 所有关卡都完成了
            LoadMainMenu();
        }
    }

    public void RestartLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        CurrentState = GameState.MainMenu;
        SceneManager.LoadScene("MainMenu");
    }

    private void LoadLevel(int levelIndex)
    {
        CurrentState = GameState.Playing;
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadCustomScene(string sceneName)
    {
        CurrentState = GameState.Playing;
        SceneManager.LoadScene(sceneName);
    }
}
