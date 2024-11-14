using Safari.Animals;
using Safari.MapComponents;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnController : MonoBehaviour
{
    public Map map;
    public List<GameObject> objectsToSpawn;

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

    private void Start()
    {
        foreach (GameObject objectToSpawn in objectsToSpawn)
        {
            if (objectToSpawn.tag == "Entity")
            {
                SpawnEntity(objectToSpawn.GetComponent<EntityController>());
            }
            else
            {
                SpawnObject(objectToSpawn);
            }
        }
    }

    public void SpawnEntity(EntityController controller) {
        SpawnObject(controller.gameObject);
        controller.TargetPosition = controller.transform.position;
    }

    public void SpawnObject(GameObject objectToSpawn)
    {
        // Get the bounds of where the player can spawn
        BoundsInt bounds = map.floor.cellBounds;

        // pick a random position for the player to spawn in
        Vector2 spawnLocation = Vector2.zero;
        bool validSpawn = false;

        while (!validSpawn)
        {
            int randomX = Random.Range(bounds.xMin, bounds.xMax);
            int randomY = Random.Range(bounds.yMin, bounds.yMax);
            spawnPoint = new Vector3Int(randomX, randomY, 0);

            if (!PosInWall(spawnPoint) && !PosIsOccupied(spawnPoint))
            {
                List<Vector3Int> adjacentWalls = GetAdjacentWalls();
                bool isInHallway = PosInHallway(spawnPoint, adjacentWalls);
                if (isInHallway)
                {
                    // need to move to outside of the hallway
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
                        isInHallway = !PosInHallway(spawnPoint, adjacentWalls);
                    }
                }

                spawnLocation = map.floor.CellToWorld(spawnPoint);
                validSpawn = true;
            }
        }

        objectToSpawn.transform.position = new Vector3(spawnLocation.x + 0.5f, spawnLocation.y + 0.5f, 0);
    }

    bool PosInWall(Vector3Int position)
    {
        TileBase floorTile = map.floor.GetTile(position);
        TileBase wallTile = map.geometry.GetTile(position);

        bool notInWall = floorTile != null && wallTile is null;
        return !notInWall;
    }

    bool PosIsOccupied(Vector3Int position)
    {
        Vector3 positionVector = map.floor.CellToWorld(position);
        foreach (GameObject gameObject in objectsToSpawn)
        {
            if (gameObject.activeSelf && gameObject.transform.position == positionVector)
            {
                return true;
            }
        }

        return false;
    }

    bool PosInHallway(Vector3Int positionToCheck, List<Vector3Int> adjacentWalls)
    {
        bool isInHallway = adjacentWalls.Count >= 5 && (adjacentWalls[0].x == adjacentWalls[1].x || adjacentWalls[0].y == adjacentWalls[1].y);
        return isInHallway;
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