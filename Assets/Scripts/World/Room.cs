using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Empty,
        Corridor,
        Office,
        Storage,
        Technical
    }

    [SerializeField] private RoomType roomType = RoomType.Empty;
    [SerializeField] private Transform[] doorPositions;
    
    private int connectedRoomsCount = 0;
    private bool hasSpawnedFuse = false;

    public RoomType Type => roomType;
    public Transform[] DoorPositions => doorPositions;

    private void OnEnable()
    {
        // Можно добавить логику активации комнаты
        Debug.Log($"Room enabled: {roomType}");
    }

    public bool TrySpawnFuse()
    {
        if (hasSpawnedFuse)
            return false;

        // Предохранители чаще спавнятся в технических помещениях
        float spawnChance = roomType == RoomType.Technical ? 0.6f : 0.2f;

        if (Random.value < spawnChance)
        {
            hasSpawnedFuse = true;
            return true;
        }

        return false;
    }

    public void SetConnectedRoomsCount(int count)
    {
        connectedRoomsCount = count;
    }

    public int GetConnectedRoomsCount()
    {
        return connectedRoomsCount;
    }

    public void PlaceObject(GameObject prefab, Vector3 position)
    {
        if (prefab != null)
        {
            Instantiate(prefab, position, Quaternion.identity, transform);
        }
    }
}

