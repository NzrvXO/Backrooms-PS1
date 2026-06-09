using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    [SerializeField] private GameObject[] roomPrefabs;

    [Header("Generation Settings")]
    [SerializeField] private int roomCount = 50;
    [SerializeField] private Vector3 roomSize = new Vector3(10, 3, 10);
    [SerializeField] private float spawnChance = 0.7f;

    [Header("Spawning")]
    [SerializeField] private GameObject fusePrefab;
    [SerializeField] private GameObject echoPrefab;
    [SerializeField] private GameObject liftPrefab;

    private List<Room> generatedRooms = new List<Room>();
    private RoomLayout roomLayout;

    private void Awake()
    {
        roomLayout = GetComponent<RoomLayout>();
        if (roomLayout == null)
            roomLayout = gameObject.AddComponent<RoomLayout>();
    }

    public void GenerateLevel()
    {
        Debug.Log("Starting level generation...");
        
        ClearLevel();
        GenerateRooms();
        
        if (roomLayout != null)
            roomLayout.BuildLayout(generatedRooms);
        
        SpawnLift();
        SpawnFuses();
        SpawnEchoes();

        Debug.Log($"Level generation complete! Generated {generatedRooms.Count} rooms.");
    }

    private void ClearLevel()
    {
        // Удаляем все существующие комнаты
        foreach (Room room in generatedRooms)
        {
            Destroy(room.gameObject);
        }
        generatedRooms.Clear();
    }

    private void GenerateRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            if (Random.value > spawnChance)
                continue;

            // Выбираем случайный префаб комнаты
            GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
            
            // Спавним комнату
            GameObject newRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, transform);
            Room roomScript = newRoom.GetComponent<Room>();
            
            if (roomScript != null)
            {
                generatedRooms.Add(roomScript);
            }
        }
    }

    private void SpawnLift()
    {
        if (liftPrefab == null || generatedRooms.Count == 0 || roomLayout == null)
            return;

        // Спавним лифт в конце уровня
        Room endRoom = roomLayout.GetEndRoom();
        Vector3 liftPosition = endRoom.transform.position + Vector3.up * 0.5f;
        
        Instantiate(liftPrefab, liftPosition, Quaternion.identity, endRoom.transform);
    }

    private void SpawnFuses()
    {
        if (fusePrefab == null)
            return;

        int fusesSpawned = 0;
        int maxFuses = 3;

        // Спавним предохранители в случайных комнатах (в основном технических)
        foreach (Room room in generatedRooms)
        {
            if (fusesSpawned >= maxFuses)
                break;

            if (room.TrySpawnFuse())
            {
                Vector3 spawnPosition = room.transform.position + Vector3.up;
                Instantiate(fusePrefab, spawnPosition, Quaternion.identity, room.transform);
                fusesSpawned++;
            }
        }

        Debug.Log($"Spawned {fusesSpawned} fuses.");
    }

    private void SpawnEchoes()
    {
        if (echoPrefab == null)
            return;

        int echoCount = Random.Range(1, 4); // 1-3 эхо

        for (int i = 0; i < echoCount; i++)
        {
            Room randomRoom = generatedRooms[Random.Range(0, generatedRooms.Count)];
            Vector3 spawnPosition = randomRoom.transform.position + Vector3.up;
            
            Instantiate(echoPrefab, spawnPosition, Quaternion.identity, randomRoom.transform);
        }

        Debug.Log($"Spawned {echoCount} echoes.");
    }

    public List<Room> GetAllRooms()
    {
        return generatedRooms;
    }
}

