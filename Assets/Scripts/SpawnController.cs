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
                new Vector3Int(spawnPoint.x + 1, spawnPoint.y + 1),
                new Vector3Int(spawnPoint.x + 1, spawnPoint.y - 1),
                new Vector3Int(spawnPoint.x - 1, spawnPoint.y + 1),
                new Vector3Int(spawnPoint.x - 1, spawnPoint.y - 1),
            };
        }
    }

    internal void SpawnObjects()
    {
        foreach (GameObject objectToSpawn in objectsToSpawn)
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

            Room room = roomPointer.roomData.prefab.GetComponent<Room>();
            Vector3 spawnLocation = GetSpawnLocation(room, roomPointer);

            if (objectToSpawn.tag == "Stairs")
            {
                spawnLocation.Set(spawnLocation.x, spawnLocation.y, 1.5f);
                objectToSpawn.transform.position = spawnLocation;
                Debug.Log($"Stairs position is: {spawnLocation}");
                continue;
            }

            GameObject newInstance = Instantiate(objectToSpawn, spawnLocation, Quaternion.identity);
            newInstance.GetComponent<EntityController>().TargetPosition = spawnLocation;

            if (objectToSpawn.tag == "Animal")
            {
                EnemyManager.instance.enemies.Add(newInstance.GetComponent<EnemyController>());
            }
            else if(objectToSpawn.tag == "Player")
            {
                mainCameraController.player = newInstance.GetComponent<PlayerController>().transform;
            }
        }
    }

    private RoomPointer GetRandomRoomPointer()
    {
        int roomIndex = Random.Range(0, map.data.rooms.Count);
        RoomPointer roomPointer = map.data.rooms[roomIndex];
        while (roomPointer == null)
        {
            roomIndex = Random.Range(0, map.data.rooms.Count);
            roomPointer = map.data.rooms[roomIndex];
        }

        return roomPointer;
    }

    private Vector3 GetSpawnLocation(Room spawnRoom, RoomPointer spawnRoomPointer)
    {
        BoundsInt bounds = spawnRoom.floor.cellBounds;
        Vector2 spawnLocation = Vector2.zero;
        bool validSpawn = false;
        List<Vector3Int> positionsTried = new List<Vector3Int>();
        do
        {
            do
            {
                int xAdjustAmt = spawnRoomPointer.origin.x * Chunk.SIZE - bounds.xMax / 2;
                int yAdjustAmt = spawnRoomPointer.origin.y * Chunk.SIZE - bounds.yMax / 2;
                int randomX = Random.Range(bounds.xMin + xAdjustAmt, bounds.xMax + xAdjustAmt);
                int randomY = Random.Range(bounds.yMin + yAdjustAmt, bounds.yMax + yAdjustAmt);
                spawnPoint = new Vector3Int(randomX, randomY, 0);

            } while (positionsTried.Contains(spawnPoint));

            positionsTried.Add(spawnPoint);

            if (PosInWall(spawnPoint) || EntityController.positionMap.ContainsKey((Vector2Int)spawnPoint))
            {
                continue;
            }

            List<Vector3Int> adjacentWalls = GetAdjacentWalls();
            bool isInHallway = IsPosInHallway(spawnPoint);
            if (isInHallway)
            {
                // pick a random adjacent position
                Vector3Int validAdjacentPos = adjacentPositions[Random.Range(0, adjacentPositions.Count)];
                bool validDirection = false;
                while (!validDirection)
                {
                    validAdjacentPos = adjacentPositions[Random.Range(0, adjacentPositions.Count)];
                    validDirection = !adjacentWalls.Contains(validAdjacentPos);
                }

                // move in that direction until we are no longer in a hallway
                Vector3Int direction = validAdjacentPos - spawnPoint;
                while (isInHallway)
                {
                    spawnPoint += direction;
                    adjacentWalls = GetAdjacentWalls();
                    isInHallway = IsPosInHallway(spawnPoint);
                }
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
