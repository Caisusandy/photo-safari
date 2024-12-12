using Safari;
using Safari.Animals;
using Safari.MapComponents;
using Safari.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnController : MonoBehaviour
{
    public List<GameObject> objectsToSpawn;
    public EnemyManager enemyManager;
    public MainCameraController mainCameraController;
    public Map map;

    [Header("Spawn Rates (in %)")]
    public float jaguarSpawnRate = 50;
    public float frogSpawnRate = 50;
    public float capybaraSpawnRate = 50;
    public float butterflySpawnRate = 50;

    [Header("Max Amount of each Animal")]
    public int maxJaguars = 5;
    public int maxFrogs = 5;
    public int maxCapybaras = 5;
    public int maxButterflies = 5;

    [Header("Debug")]
    [Tooltip("Spawns everything in the initial room.")]
    public bool spawnInInitialRoom = false;

    private Vector3Int spawnPoint;
    private List<Vector3Int> adjacentPositions
    {
        get
        {
            return new List<Vector3Int>()
            {
                new Vector3Int(spawnPoint.x + 1, spawnPoint.y),
                new Vector3Int(spawnPoint.x - 1, spawnPoint.y),
                new Vector3Int(spawnPoint.x, spawnPoint.y + 1),
                new Vector3Int(spawnPoint.x, spawnPoint.y - 1),
            };
        }
    }

    internal void SpawnObjects()
    {
        foreach (GameObject objectToSpawn in objectsToSpawn)
        {
            Vector3? testSpawnLocation = GetSpawnLocation(objectToSpawn);
            if (!testSpawnLocation.HasValue)
            {
                Debug.LogError("Failed to find a spawn point");
                break;
            }

            Vector3 spawnLocation = testSpawnLocation.Value;
            GameObject newInstance;
            if (objectToSpawn.name == "Stairs")
            {
                newInstance = Instantiate(objectToSpawn, spawnLocation, Quaternion.identity);
                GameManager.instance.stairs = newInstance.transform;
            }
            else if (objectToSpawn.CompareTag("Animal"))
            {
                // how many more of that animal to spawn
                int numAnimalsToSpawn = 0;
                if (objectToSpawn.name.Contains("frog", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    numAnimalsToSpawn = Random.Range(2, maxFrogs);
                }
                else if (objectToSpawn.name.Contains("jaguar", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    numAnimalsToSpawn = Random.Range(2, maxJaguars);
                }
                else if (objectToSpawn.name.Contains("capybara", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    numAnimalsToSpawn = Random.Range(2, maxCapybaras);
                }
                else if (objectToSpawn.name.Contains("butterfly", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    numAnimalsToSpawn = Random.Range(2, maxButterflies);
                }

                for (int i = 0; i < numAnimalsToSpawn; i++)
                {
                    TrySpawnAnimal(objectToSpawn);
                }
            }
            else if (objectToSpawn.CompareTag("Player"))
            {
                newInstance = Instantiate(objectToSpawn, spawnLocation, Quaternion.identity);
                PlayerController newPlayer = newInstance.GetComponent<PlayerController>();
                mainCameraController.player = newPlayer.transform;
                newPlayer.TargetPosition = spawnLocation;
            }
            else
            {
                newInstance = Instantiate(objectToSpawn, spawnLocation, Quaternion.identity);
            }
        }
    }

    public void TrySpawnAnimal(GameObject objectToSpawn)
    {
        Vector3? testSpawnLocation = GetSpawnLocation(objectToSpawn);
        if (!testSpawnLocation.HasValue)
        {
            Debug.LogError("Failed to find a spawn point");
            return;
        }
        var spawnLocation = testSpawnLocation.Value;
        var newInstance = Instantiate(objectToSpawn, spawnLocation, Quaternion.identity);
        newInstance.name = newInstance.name.Remove(newInstance.name.IndexOf("("));
        newInstance.GetComponent<EntityController>().TargetPosition = spawnLocation;
        EnemyManager.instance.enemies.Add(newInstance.GetComponent<EnemyController>());
    }

    private Vector3? GetSpawnLocation(GameObject objectToSpawn)
    {
        RoomPointer roomPointer = GetSpawnRoomPointer(objectToSpawn);
        Room room = roomPointer.roomData.prefab.GetComponent<Room>();
        var spawnLocation = GetSpawnVector(room, roomPointer);
        return spawnLocation;
    }

    private RoomPointer GetSpawnRoomPointer(GameObject objectToSpawn)
    {
        RoomPointer roomPointer;
        if (spawnInInitialRoom || objectToSpawn.tag == "Player")
        {
            roomPointer = map.data.chunks[2, 2].instancePointer;
        }
        else
        {
            roomPointer = GetRandomRoomPointer();
        }

        return roomPointer;
    }

    private RoomPointer GetRandomRoomPointer()
    {
        int roomIndex = Random.Range(0, map.data.rooms.Count);
        RoomPointer roomPointer = map.data.rooms[roomIndex] ?? throw new System.NullReferenceException();
        return roomPointer;
    }

    private Vector3? GetSpawnVector(Room spawnRoom, RoomPointer spawnRoomPointer)
    {
        BoundsInt bounds = spawnRoom.floor.cellBounds;
        Vector2 spawnLocation = Vector2.zero;
        bool validSpawn = false;
        HashSet<Vector3Int> positionsTried = new HashSet<Vector3Int>();
        do
        {
            do
            {
                if (positionsTried.Count > 10000)
                {
                    Debug.LogError("Failed to find a point");
                    return null;
                }

                // these adjust amounts are accounting for that (0,0) is for the current room, not for the entire map
                int xAdjustAmt = spawnRoomPointer.origin.x * Chunk.SIZE - bounds.xMax / 2;
                int yAdjustAmt = spawnRoomPointer.origin.y * Chunk.SIZE - bounds.yMax / 2;

                int randomX = Random.Range(bounds.xMin + xAdjustAmt, bounds.xMax + xAdjustAmt);
                int randomY = Random.Range(bounds.yMin + yAdjustAmt, bounds.yMax + yAdjustAmt);
                spawnPoint = new Vector3Int(randomX, randomY, 0);
            } while (positionsTried.Contains(spawnPoint));


            if (PosInWall(spawnPoint) || EntityController.positionMap.ContainsKey((Vector2Int)spawnPoint))
            {
                positionsTried.Add(spawnPoint);
                continue;
            }

            List<Vector3Int> adjacentWalls = GetAdjacentWalls();
            bool isInHallway = IsPosInHallway(spawnPoint);
            while (isInHallway)
            {
                // pick an adjacent position with no wall
                Vector3Int validAdjacentPos = adjacentPositions[Random.Range(0, adjacentPositions.Count)];
                bool validDirection = !adjacentWalls.Contains(validAdjacentPos);
                while (!validDirection)
                {
                    validAdjacentPos = adjacentPositions[Random.Range(0, adjacentPositions.Count)];
                    validDirection = !adjacentWalls.Contains(validAdjacentPos);
                }

                // move in that direction until we are no longer in a hallway or find another wall
                Vector3Int direction = validAdjacentPos - spawnPoint;
                while (isInHallway && validDirection)
                {
                    spawnPoint += direction;
                    adjacentWalls = GetAdjacentWalls();
                    isInHallway = IsPosInHallway(spawnPoint);

                    Vector3Int nextPos = spawnPoint + direction;
                    validDirection = !adjacentWalls.Contains(nextPos);
                }
            }

            if (EntityController.positionMap.ContainsKey((Vector2Int)spawnPoint))
            {
                positionsTried.Add(spawnPoint);
                continue;
            }

            spawnLocation = map.floor.CellToWorld(spawnPoint);
            validSpawn = true;
        }
        while (!validSpawn);

        return new Vector3(spawnLocation.x + 0.5f, spawnLocation.y + 0.5f, 0);
    }

    bool PosInWall(Vector3Int position)
    {
        TileBase floorTile = map.floor.GetTile(position);
        TileBase wallTile = map.geometry.GetTile(position);

        bool notInWall = floorTile != null && wallTile is null;
        return !notInWall;
    }

    bool IsPosInHallway(Vector3Int positionToCheck)
    {
        var chunkPosition = Chunk.ToChunk(positionToCheck);
        if (map.data.IsOutOfBound(chunkPosition)) return false;
        Chunk currentChunk = map.data.chunks[Mathf.Clamp(chunkPosition.x, 0, 31), Mathf.Clamp(chunkPosition.y, 0, 31)];
        if (currentChunk.instancePointer != null)
        {
            return currentChunk.isHallway;
        }

        return true; // find another place to spawn since a room doesn't exist there.
    }

    List<Vector3Int> GetAdjacentWalls()
    {
        List<Vector3Int> adjacentWalls = new List<Vector3Int>();
        foreach (Vector3Int position in adjacentPositions)
        {
            // check if the position is a wall
            TileBase wallTile = map.geometry.GetTile(position);
            if (wallTile != null)
            {
                adjacentWalls.Add(position);
            }
        }

        return adjacentWalls;
    }
}
