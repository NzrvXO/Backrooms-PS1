using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private int fusesRequired = 3;
    private int fusesActivated = 0;

    [Header("References")]
    [SerializeField] private LevelGenerator levelGenerator;

    private bool gameOver = false;
    private bool levelComplete = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (levelGenerator == null)
            levelGenerator = FindObjectOfType<LevelGenerator>();

        if (levelGenerator != null)
            levelGenerator.GenerateLevel();
    }

    public void ActivateFuse()
    {
        if (gameOver || levelComplete)
            return;

        fusesActivated++;
        Debug.Log($"Fuses activated: {fusesActivated}/{fusesRequired}");

        if (fusesActivated >= fusesRequired)
        {
            OnAllFusesActivated();
        }

        // Обновляем UI
        UIManager.Instance?.UpdateFuseCount(fusesActivated, fusesRequired);
    }

    private void OnAllFusesActivated()
    {
        levelComplete = true;
        Debug.Log("All fuses activated! Lift is powered!");
        
        // Эффект: свет гаснет, звук генератора
        LiftManager.Instance?.PowerUp();
    }

    public void CompleteLevel()
    {
        if (levelComplete && !gameOver)
        {
            gameOver = true;
            Debug.Log("Level complete! Escaped!");
            UIManager.Instance?.ShowEscapeMessage();
        }
    }

    public bool AreAllFusesActivated()
    {
        return fusesActivated >= fusesRequired;
    }

    public int GetFuseCount()
    {
        return fusesActivated;
    }

    public void ResetLevel()
    {
        fusesActivated = 0;
        gameOver = false;
        levelComplete = false;
    }
}

