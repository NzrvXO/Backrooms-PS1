using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI fuseCountText;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (fuseCountText == null)
            fuseCountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateFuseCount(int current, int required)
    {
        if (fuseCountText != null)
            fuseCountText.text = $"FUSES: {current} / {required}";
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
        }
    }

    public void ShowEscapeMessage()
    {
        ShowMessage("ESCAPED!");
    }

    public void HideMessage()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
}

