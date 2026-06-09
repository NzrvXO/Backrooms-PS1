using UnityEngine;
using System.Collections;

public class LiftManager : MonoBehaviour
{
    public static LiftManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Light liftLight;
    [SerializeField] private AudioSource generatorSound;
    [SerializeField] private AudioSource liftBell;

    [Header("Settings")]
    [SerializeField] private float blackoutDuration = 3f;
    [SerializeField] private float doorOpenDistance = 2f;

    private bool powered = false;
    private bool doorsOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PowerUp()
    {
        if (powered)
            return;

        powered = true;
        StartCoroutine(PowerUpSequence());
    }

    private IEnumerator PowerUpSequence()
    {
        // Гасим свет
        if (liftLight != null)
            liftLight.enabled = false;

        yield return new WaitForSeconds(0.5f);

        // Звук генератора
        if (generatorSound != null)
            generatorSound.Play();

        yield return new WaitForSeconds(1f);

        // Звук лифта
        if (liftBell != null)
            liftBell.Play();

        // Включаем свет
        if (liftLight != null)
            liftLight.enabled = true;

        yield return new WaitForSeconds(blackoutDuration);

        // Двери лифта открываются
        OpenDoors();
    }

    private void OpenDoors()
    {
        doorsOpen = true;
        Debug.Log("Lift doors are open!");
        
        // Визуальная смена состояния дверей (можно добавить анимацию)
        Animator doorsAnimator = GetComponentInChildren<Animator>();
        if (doorsAnimator != null)
            doorsAnimator.SetBool("Open", true);
    }

    public bool CanPlayerEscape()
    {
        if (!powered)
            return false;

        float distance = Vector3.Distance(transform.position, FindObjectOfType<PlayerController>().transform.position);
        return distance < doorOpenDistance && doorsOpen;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && CanPlayerEscape())
        {
            GameManager.Instance?.CompleteLevel();
        }
    }
}

