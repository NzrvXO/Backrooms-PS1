using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Components;

namespace World
{
    public class LevelGeneratorGPT : MonoBehaviour
    {
        private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

        [Header("Generation")]
        [SerializeField] private int roomCount = 40;
        [SerializeField] private float roomSize = 10f;
        [SerializeField] private float roomHeight = 3f;

        [Header("Materials")]
        [SerializeField] private Material floorMaterial;
        [SerializeField] private Material ceilingMaterial;
        [SerializeField] private Material wallMaterial;

        [Header("Lamp / Lighting")]
        [SerializeField] private Material lampMaterial;
        [SerializeField] private Color lampColor = Color.white;
        [SerializeField] private float lampIntensity = 1.5f;
        [SerializeField] private float lampRange = 8f;
        [SerializeField] private float lampSize = 1f;
        [SerializeField] private bool spawnLamp = true;

        // New serialized fields for elevator / enemy / fuse prefabs and settings
        [Header("Prefabs & Gameplay")]
        [SerializeField] private GameObject elevatorPrefab;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject fusePrefab;
        [SerializeField] private int requiredFuses = 3;
        [SerializeField] private int elevatorSpawnRadiusRooms = 2; // how many rooms away from enemy we try to spawn elevator

        private Dictionary<Vector2Int, RoomData> _rooms = new();

        private class RoomData
        {
            public Vector2Int Position;

            public bool North;
            public bool South;
            public bool East;
            public bool West;

            public GameObject Root;
        }

        private void Start()
        {
            Generate();
        }

        private void Generate()
        {
            GenerateLayout();
            CreateExtraConnections(0.2f);
            BuildRooms();

            // Теперь спавним врага, лифт и предохранители в нужном порядке
            GameObject spawnedEnemy = SpawnEnemy();
            GameObject spawnedElevator = SpawnElevatorNearEnemy(spawnedEnemy);
            SpawnFuses(spawnedElevator);
        }

        private void GenerateLayout()
        {
            // Инициализируем стартовую комнату в центре
            RoomData start = new()
            {
                Position = Vector2Int.zero
            };

            _rooms.Add(Vector2Int.zero, start);

            // Для каждой новой комнаты выбираем случайную существующую как родителя,
            // чтобы получались развилки и дерево, а не одна длинная "змейка".
            for (int i = 0; i < roomCount; i++)
            {
                RoomData parent =
                    _rooms.Values.ElementAt(Random.Range(0, _rooms.Count));

                Vector2Int current = parent.Position;

                List<Vector2Int> neighbours = new()
                {
                    current + Vector2Int.up,
                    current + Vector2Int.down,
                    current + Vector2Int.left,
                    current + Vector2Int.right
                };

                neighbours.RemoveAll(x => _rooms.ContainsKey(x));

                if (neighbours.Count == 0)
                    continue;

                Vector2Int next =
                    neighbours[Random.Range(0, neighbours.Count)];

                RoomData room = new()
                {
                    Position = next
                };

                _rooms.Add(next, room);

                Connect(current, next);
            }
        }

        private void CreateExtraConnections(float chance)
        {
            // Пробегаем по всем комнатам и с вероятностью "chance" создаём дополнительную
            // связь с уже существующим соседем (если есть). Это добавляет тупики, кольца и
            // альтернативные маршруты.
            foreach (var kvp in _rooms)
            {
                Vector2Int pos = kvp.Key;

                Vector2Int[] dirs =
                {
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right
                };

                foreach (var dir in dirs)
                {
                    Vector2Int neighbour = pos + dir;

                    if (!_rooms.ContainsKey(neighbour))
                        continue;

                    if (Random.value > chance)
                        continue;

                    Connect(pos, neighbour);
                }
            }
        }

        private void Connect(Vector2Int a, Vector2Int b)
        {
            Vector2Int dir = b - a;

            if (dir == Vector2Int.up)
            {
                _rooms[a].North = true;
                _rooms[b].South = true;
            }

            if (dir == Vector2Int.down)
            {
                _rooms[a].South = true;
                _rooms[b].North = true;
            }

            if (dir == Vector2Int.right)
            {
                _rooms[a].East = true;
                _rooms[b].West = true;
            }

            if (dir == Vector2Int.left)
            {
                _rooms[a].West = true;
                _rooms[b].East = true;
            }
        }

        private void BuildRooms()
        {
            foreach (var room in _rooms.Values)
            {
                BuildRoom(room);
            }
        }

        private void BuildRoom(RoomData room)
        {
            Vector3 center =
                new Vector3(
                    room.Position.x * roomSize,
                    0,
                    room.Position.y * roomSize);

            GameObject root =
                new GameObject($"Room_{room.Position}");

            room.Root = root;

            root.transform.parent = transform;

            // Устанавливаем позицию корня на центр комнаты, чтобы все объекты и спавн происходили относительно этой позиции
            root.transform.position = center;
            
            CreateCube(
                root.transform,
                Vector3.zero,
                new Vector3(roomSize, 0.2f, roomSize),
                "Floor",
                floorMaterial);

            CreateCube(
                root.transform,
                Vector3.up * roomHeight,
                new Vector3(roomSize, 0.2f, roomSize),
                "Ceiling",
                ceilingMaterial);

            // Создаём лампу на потолке и источник света (как в backrooms)
            if (spawnLamp)
            {
                // Позиция лампы чуть ниже потолка, по центру комнаты
                Vector3 lampPos = Vector3.up * (roomHeight - 0.2f);

                // Создаём Quad и разворачиваем его так, чтобы он смотрел вниз в комнату
                GameObject lampGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
                lampGo.name = "CeilingLamp";
                lampGo.transform.parent = root.transform;
                lampGo.transform.localPosition = lampPos;
                // Поворачиваем вниз (нормаль будет смотреть вниз в комнату)
                lampGo.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                lampGo.transform.localScale = new Vector3(lampSize, lampSize, 1f);

                var lampRend = lampGo.GetComponent<Renderer>();
                if (lampMaterial != null)
                {
                    lampRend.sharedMaterial = lampMaterial;
                    var inst = lampRend.material;
                    if (inst != null)
                    {
                        inst.EnableKeyword("_EMISSION");
                        inst.SetColor(EmissionColorID, lampColor * Mathf.LinearToGammaSpace(lampIntensity));
                        // Масштабируем текстуру лампы под размер
                        inst.mainTextureScale = new Vector2(Mathf.Max(0.1f, lampSize), Mathf.Max(0.1f, lampSize));
                    }
                }
                else
                {
                    // Если материал не задан, покрасим quad в эмиссный цвет
                    var mat = new Material(Shader.Find("Standard"));
                    mat.EnableKeyword("_EMISSION");
                    mat.color = lampColor;
                    mat.SetColor(EmissionColorID, lampColor * Mathf.LinearToGammaSpace(lampIntensity));
                    lampRend.material = mat;
                }

                // Отключаем тени для quad лампы, чтобы он не влиял на освещение сцены тенями
                lampRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lampRend.receiveShadows = false;

                // Создаём точечный источник света под лампой
                GameObject lightObj = new GameObject("LampLight");
                lightObj.transform.parent = root.transform;
                lightObj.transform.localPosition = lampPos + Vector3.down * 0.1f; // слегка ниже_quad, чтобы свет падал в комнату
                var lampLight = lightObj.AddComponent<Light>();
                lampLight.type = LightType.Point;
                lampLight.color = lampColor;
                lampLight.intensity = lampIntensity;
                lampLight.range = lampRange;
                lampLight.shadows = LightShadows.None;
            }

            if (!room.North)
                CreateCube(
                    root.transform,
                    new Vector3(0, roomHeight / 2, roomSize / 2),
                     new Vector3(roomSize, roomHeight, 0.2f),
                     "NorthWall",
                     wallMaterial);

            if (!room.South)
                CreateCube(
                    root.transform,
                    new Vector3(0, roomHeight / 2, -roomSize / 2),
                     new Vector3(roomSize, roomHeight, 0.2f),
                     "SouthWall",
                     wallMaterial);

            if (!room.East)
                CreateCube(
                    root.transform,
                    new Vector3(roomSize / 2, roomHeight / 2, 0),
                     new Vector3(0.2f, roomHeight, roomSize),
                     "EastWall",
                     wallMaterial);

            if (!room.West)
                CreateCube(
                    root.transform,
                    new Vector3(-roomSize / 2, roomHeight / 2, 0),
                     new Vector3(0.2f, roomHeight, roomSize),
                     "WestWall",
                     wallMaterial);
        }

        // Spawns an enemy (placeholder if no prefab) in a random existing room.
        private GameObject SpawnEnemy()
        {
            if (_rooms.Count == 0)
                return null;

            var roomsList = _rooms.Values.ToList();
            RoomData chosen = roomsList[Random.Range(0, roomsList.Count)];

            Debug.Log($"SpawnEnemy: chosen room {chosen.Position}");

            GameObject enemy;
            if (enemyPrefab != null)
            {
                enemy = Instantiate(enemyPrefab, chosen.Root.transform.position + Vector3.up * 0.5f, Quaternion.identity, chosen.Root.transform);
            }
            else
            {
                enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                enemy.name = "Enemy_Placeholder";
                enemy.transform.position = chosen.Root.transform.position + Vector3.up * 0.5f;
                enemy.transform.parent = chosen.Root.transform;
                // Disable collider visual interference
                var col = enemy.GetComponent<Collider>();
                if (col != null) col.isTrigger = false;
            }

            Debug.Log($"Enemy spawned at {enemy.transform.position}");

             return enemy;
         }

        // Spawn elevator near the spawned enemy (tries to pick a nearby room).
        private GameObject SpawnElevatorNearEnemy(GameObject enemy)
        {
            // Find furthest by default if we can't place near enemy
            RoomData targetRoom = null;

            if (enemy != null)
            {
                // Determine which room the enemy is parented to (if any)
                Transform parent = enemy.transform.parent;
                var room = _rooms.Values.FirstOrDefault(r => r.Root != null && r.Root.transform == parent);
                if (room != null)
                {
                    // Find candidate rooms within radius (in grid steps)
                    var candidates = _rooms.Values.Where(r => Vector2Int.Distance(r.Position, room.Position) <= elevatorSpawnRadiusRooms && r != room).ToList();
                    if (candidates.Count > 0)
                    {
                        // Prefer a random room within radius
                        targetRoom = candidates[Random.Range(0, candidates.Count)];
                    }
                }
            }

            // Fallback: furthest room
            if (targetRoom == null)
            {
                // Если не удалось найти близкую к врагу комнату — выберем просто случайную комнату
                var allRooms = _rooms.Values.ToList();
                if (allRooms.Count > 0)
                    targetRoom = allRooms[Random.Range(0, allRooms.Count)];
                else
                    return null;
            }

            Debug.Log($"SpawnElevator: target room {targetRoom.Position}");

             GameObject elevator;
             if (elevatorPrefab != null)
             {
                 elevator = Instantiate(elevatorPrefab, targetRoom.Root.transform.position + Vector3.up, Quaternion.identity, targetRoom.Root.transform);
             }
             else
             {
                 elevator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                 elevator.name = "Elevator";
                 elevator.transform.position = targetRoom.Root.transform.position + Vector3.up;
                 elevator.transform.localScale = new Vector3(2, 2, 2);
                 elevator.transform.parent = targetRoom.Root.transform;
             }

            Debug.Log($"Elevator spawned at {elevator.transform.position}");

             // Ensure elevator has ElevatorController component and it's configured
             var ec = elevator.GetComponent<ElevatorController>();
             if (ec == null)
                 ec = elevator.AddComponent<ElevatorController>();

             ec.requiredFuses = Mathf.Max(1, requiredFuses);

             // If there's a LiftManager in scene, we don't automatically power it here - ElevatorController will call it when activated.

             return elevator;
         }

        // Spawn fuses in strictly different directions relative to origin (north/south/east or fallback random)
        private void SpawnFuses(GameObject elevator)
        {
            var elevatorController = elevator != null ? elevator.GetComponent<ElevatorController>() : null;

            // Use elevator's room as center; fallback to origin
            Vector2Int centerPos = Vector2Int.zero;
            if (elevator != null)
            {
                var elevRoom = _rooms.Values.FirstOrDefault(r => r.Root != null && r.Root.transform == elevator.transform.parent);
                if (elevRoom != null)
                    centerPos = elevRoom.Position;
            }

            // Directions to try (three distinct) — up, down, right (north, south, east)
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.right };

            HashSet<RoomData> chosenRooms = new();

            foreach (var dir in dirs)
            {
                // Select rooms whose position projects positively on this dir from elevator center
                var candidates = _rooms.Values.Where(r => ((r.Position - centerPos).x * dir.x + (r.Position - centerPos).y * dir.y) > 0).ToList();
                RoomData pick = null;
                if (candidates.Count > 0)
                {
                    // pick the farthest in that direction for better separation
                    var ordered = candidates.OrderByDescending(r => ((r.Position - centerPos).x * dir.x + (r.Position - centerPos).y * dir.y)).Take(3).ToList();
                    pick = ordered[Random.Range(0, ordered.Count)];
                }

                if (pick == null)
                {
                    // fallback: any room not already chosen
                    pick = _rooms.Values.Except(chosenRooms).FirstOrDefault();
                }

                if (pick == null)
                    continue;

                chosenRooms.Add(pick);

                Debug.Log($"SpawnFuses: chosen fuse room {pick.Position} for dir {dir}");

                 GameObject fuse;
                 if (fusePrefab != null)
                 {
                     fuse = Instantiate(fusePrefab, pick.Root.transform.position + Vector3.up * 0.5f, Quaternion.identity, pick.Root.transform);
                 }
                 else
                 {
                     fuse = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                     fuse.name = $"Fuse_{dir}";
                     fuse.transform.position = pick.Root.transform.position + Vector3.up * 0.5f;
                     fuse.transform.localScale = Vector3.one * 0.5f;
                     fuse.transform.parent = pick.Root.transform;
                 }

                Debug.Log($"Fuse spawned at {fuse.transform.position}");

                 // Add fuse interaction component and link to elevator controller if available
                 var fs = fuse.GetComponent<FuseSwitch>();
                 if (fs == null)
                     fs = fuse.AddComponent<FuseSwitch>();

                 fs.linkedElevator = elevatorController;
             }

            // If we didn't spawn enough fuses (e.g. fewer directions than requiredFuses), spawn more in random distinct rooms
            int spawned = chosenRooms.Count;
            var remainingRooms = _rooms.Values.Except(chosenRooms).ToList();
            while (spawned < requiredFuses && remainingRooms.Count > 0)
            {
                var pick = remainingRooms[Random.Range(0, remainingRooms.Count)];
                remainingRooms.Remove(pick);
                chosenRooms.Add(pick);

                GameObject fuse;
                if (fusePrefab != null)
                {
                    fuse = Instantiate(fusePrefab, pick.Root.transform.position + Vector3.up * 0.5f, Quaternion.identity, pick.Root.transform);
                }
                else
                {
                    fuse = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    fuse.name = $"Fuse_extra";
                    fuse.transform.position = pick.Root.transform.position + Vector3.up * 0.5f;
                    fuse.transform.localScale = Vector3.one * 0.5f;
                    fuse.transform.parent = pick.Root.transform;
                }

                var fs = fuse.GetComponent<FuseSwitch>();
                if (fs == null)
                    fs = fuse.AddComponent<FuseSwitch>();

                fs.linkedElevator = elevatorController;

                spawned++;
            }
        }

        private void CreateCube(
            Transform parent,
            Vector3 position,
            Vector3 scale,
            string objName,
            Material material = null)
        {
            GameObject cube =
                GameObject.CreatePrimitive(
                    PrimitiveType.Cube);

            cube.name = objName;

            cube.transform.parent = parent;

            // Устанавливаем локальную позицию, так как parent может быть расположен по центру комнаты
            cube.transform.localPosition = position;
            cube.transform.localScale = scale;

            if (material != null)
            {
                var meshRenderer = cube.GetComponent<Renderer>();
                if (meshRenderer != null)
                {
                    // Назначим sharedMaterial сначала (повторно используемый экземпляр)
                    meshRenderer.sharedMaterial = material;

                    // Определяем желаемое масштабирование текстуры в зависимости от типа объекта
                    Vector2 tiling = Vector2.one;

                    if (objName.Contains("Floor") || objName.Contains("Ceiling"))
                    {
                        // Для пола/потолка используем X/Z
                        tiling = new Vector2(Mathf.Max(1f, scale.x), Mathf.Max(1f, scale.z));
                    }
                    else if (objName.Contains("Wall"))
                    {
                        // Для стен используем горизонтальный размер и высоту.
                        // Выбираем большую горизонтальную компоненту (scale.x или scale.z)
                        float horizontal = Mathf.Max(scale.x, scale.z);
                        tiling = new Vector2(Mathf.Max(1f, horizontal), Mathf.Max(1f, scale.y));
                    }

                    // Если нужно задать масштабирование текстуры, создаём экземпляр материала и применяем tiling
                    if (tiling != Vector2.one)
                    {
                        // meshRenderer.material создаст инстанцированный материал для этого рендера
                        var inst = meshRenderer.material;
                        if (inst != null)
                        {
                            inst.mainTextureScale = tiling;
                        }
                    }
                }
            }
        }
    }
}

