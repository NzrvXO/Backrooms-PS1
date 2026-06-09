using UnityEngine;
using System.Collections.Generic;

public class RoomLayout : MonoBehaviour
{
    [Header("Layout Settings")]
    [SerializeField] private Vector3 roomSpacing = new Vector3(12, 0, 12);
    
    private Dictionary<Vector2Int, Room> roomGrid = new Dictionary<Vector2Int, Room>();
    private List<Room> allRooms = new List<Room>();

    public void BuildLayout(List<Room> rooms)
    {
        allRooms = rooms;
        roomGrid.Clear();

        // Распределяем комнаты в сетку
        int roomsPerRow = Mathf.CeilToInt(Mathf.Sqrt(rooms.Count));
        
        for (int i = 0; i < rooms.Count; i++)
        {
            int gridX = i % roomsPerRow;
            int gridZ = i / roomsPerRow;
            Vector2Int gridPos = new Vector2Int(gridX, gridZ);

            // Позиционируем комнату
            rooms[i].transform.position = new Vector3(gridX * roomSpacing.x, 0, gridZ * roomSpacing.z);
            roomGrid[gridPos] = rooms[i];

            // Подключаем соседние комнаты
            ConnectAdjacentRooms(gridPos, rooms[i]);
        }

        Debug.Log($"Layout built: {roomsPerRow}x{Mathf.CeilToInt((float)rooms.Count / roomsPerRow)} grid");
    }

    private void ConnectAdjacentRooms(Vector2Int gridPos, Room room)
    {
        Room[] adjacent = GetAdjacentRooms(gridPos);
        room.SetConnectedRoomsCount(adjacent.Length);
    }

    private Room[] GetAdjacentRooms(Vector2Int gridPos)
    {
        List<Room> adjacent = new List<Room>();

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.right,    // Справа
            Vector2Int.left,     // Слева
            Vector2Int.up,       // Спереди
            Vector2Int.down      // Сзади
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = gridPos + dir;
            if (roomGrid.ContainsKey(checkPos))
            {
                adjacent.Add(roomGrid[checkPos]);
            }
        }

        return adjacent.ToArray();
    }

    public Room GetStartRoom()
    {
        return allRooms.Count > 0 ? allRooms[0] : null;
    }

    public Room GetEndRoom()
    {
        return allRooms.Count > 0 ? allRooms[allRooms.Count - 1] : null;
    }

    public Vector3 GetGridSize()
    {
        int roomsPerRow = Mathf.CeilToInt(Mathf.Sqrt(allRooms.Count));
        return new Vector3(
            roomsPerRow * roomSpacing.x,
            roomSpacing.y,
            Mathf.CeilToInt((float)allRooms.Count / roomsPerRow) * roomSpacing.z
        );
    }

    private void OnDrawGizmos()
    {
        if (roomGrid.Count == 0)
            return;

        Gizmos.color = Color.cyan;
        Vector3 gridSize = GetGridSize();
        Gizmos.DrawWireCube(gridSize / 2, gridSize);
    }
}

