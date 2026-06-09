using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{
    private static StartupManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log("=== BACKROOMS: ECHOES - Starting ===");

        // Проверяем наличие GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null)
        {
            Debug.LogError("GameManager not found in scene!");
            return;
        }

        // Проверяем наличие Player
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Player with PlayerController not found!");
            return;
        }

        // Проверяем наличие UIManager
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui == null)
        {
            Debug.LogWarning("UIManager not found, some UI features may not work");
        }

        // Проверяем наличие LevelGenerator
        LevelGenerator lg = FindObjectOfType<LevelGenerator>();
        if (lg == null)
        {
            Debug.LogError("LevelGenerator not found in scene!");
            return;
        }

        Debug.Log("=== All systems initialized successfully ===");
        Debug.Log("Game is ready to play!");
    }

    private void Update()
    {
        // Перезагрузка сцены на R
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Restarting level...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Выход на ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Quitting game...");
            Application.Quit();
        }
    }
}

