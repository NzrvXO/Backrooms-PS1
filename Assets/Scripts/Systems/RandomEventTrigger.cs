using UnityEngine;
using System.Collections;

public class RandomEventTrigger : MonoBehaviour
{
    [Header("Event Trigger")]
    [SerializeField] private float minEventWait = 15f;
    [SerializeField] private float maxEventWait = 45f;

    private float eventWaitTime;
    private float eventTimer;

    private void Start()
    {
        ResetEventTimer();
    }

    private void Update()
    {
        eventTimer += Time.deltaTime;

        if (eventTimer >= eventWaitTime)
        {
            ResetEventTimer();
            TriggerRandomEvent();
        }
    }

    private void ResetEventTimer()
    {
        eventWaitTime = Random.Range(minEventWait, maxEventWait);
        eventTimer = 0f;
    }

    private void TriggerRandomEvent()
    {
        int eventType = Random.Range(0, 3);

        switch (eventType)
        {
            case 0:
                TriggerDimensionalShift();
                break;
            case 1:
                TriggerDistortedSounds();
                break;
            case 2:
                TriggerWeirdMovement();
                break;
        }
    }

    private void TriggerDimensionalShift()
    {
        Debug.Log("Event: Dimensional Shift - Things feel... off");
        // Игрок может почувствовать, что комнаты меняются вокруг них
        StartCoroutine(FlickerLights());
    }

    private void TriggerDistortedSounds()
    {
        Debug.Log("Event: Strange sounds echo through the backrooms...");
        // Звуки далеких голосов, гулкие звуки в коридорах
    }

    private void TriggerWeirdMovement()
    {
        Debug.Log("Event: Movement feels wrong...");
        // Сложности с управлением (добавляется шум в input)
    }

    private IEnumerator FlickerLights()
    {
        Light[] lights = FindObjectsOfType<Light>();
        
        for (int i = 0; i < 3; i++)
        {
            foreach (Light light in lights)
            {
                light.enabled = false;
            }
            
            yield return new WaitForSeconds(0.1f);
            
            foreach (Light light in lights)
            {
                light.enabled = true;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
}

