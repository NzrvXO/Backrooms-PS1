using UnityEngine;
using System.Collections;

public class AnomalySystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float eventCheckInterval = 10f;
    [SerializeField] private float eventChance = 0.3f;

    [Header("References")]
    [SerializeField] private LevelGenerator levelGenerator;

    private float eventTimer = 0f;

    private void Start()
    {
        if (levelGenerator == null)
            levelGenerator = FindObjectOfType<LevelGenerator>();
    }

    private void Update()
    {
        eventTimer += Time.deltaTime;

        if (eventTimer >= eventCheckInterval)
        {
            eventTimer = 0f;
            CheckForAnomaly();
        }
    }

    private void CheckForAnomaly()
    {
        if (Random.value > eventChance)
            return;

        int anomalyType = Random.Range(0, 3);

        switch (anomalyType)
        {
            case 0:
                TriggerRoomDuplicate();
                break;
            case 1:
                TriggerWrongDoor();
                break;
            case 2:
                TriggerMannikin();
                break;
        }
    }

    private void TriggerRoomDuplicate()
    {
        Debug.Log("Anomaly: Room Duplicate");
        // Логика: комната меняется при возврате
        // Будет реализовано позже
    }

    private void TriggerWrongDoor()
    {
        Debug.Log("Anomaly: Wrong Door - Door changed!");
        // Логика: дверь меняет цвет или исчезает
    }

    private void TriggerMannikin()
    {
        Debug.Log("Anomaly: Mannikin appeared!");
        // Логика: манекен появляется в случайной комнате
        
        if (levelGenerator != null)
        {
            var rooms = levelGenerator.GetAllRooms();
            if (rooms.Count > 0)
            {
                Room randomRoom = rooms[Random.Range(0, rooms.Count)];
                
                // Спавним простой куб как манекен (позже заменить на модель)
                GameObject mannikin = new GameObject("Mannikin");
                mannikin.transform.position = randomRoom.transform.position + Vector3.up;
                mannikin.transform.parent = randomRoom.transform;

                // Добавляем кубик для визуализации
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = mannikin.transform;
                cube.transform.localPosition = Vector3.zero;
            }
        }
    }
}

