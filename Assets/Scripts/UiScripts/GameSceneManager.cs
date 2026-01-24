using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Load scene by name
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // Ensure game is unpaused when loading
        SceneManager.LoadScene(sceneName);
    }

    // Load scene by number
    public void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1f; // Ensure game is unpaused when loading
        SceneManager.LoadScene(sceneIndex);
    }

    // Reload current scene
    public void ReloadScene()
    {
        Time.timeScale = 1f; // Ensure game is unpaused when loading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Load next scene
    public void LoadNextScene()
    {
        Time.timeScale = 1f; // Ensure game is unpaused when loading
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }

    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    // Resume the game
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    // Toggle pause state
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Check if game is paused
    public bool IsPaused()
    {
        return isPaused;
    }

    // Quit game
    public void QuitGame()
    {
        Application.Quit();
    }
}