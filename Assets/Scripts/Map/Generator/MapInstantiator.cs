using Safari.MapComponents;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari
{
    /// <summary>
    /// class reponsible to instantiate a map
    /// </summary>
    public class MapInstantiator
    {
        const int hallwaySize = 4;
        const int wallThickness = 1;
        const int hallwayTotalSize = hallwaySize + wallThickness * 2;
        const int lowerLeft = Chunk.SIZE / 2 - hallwaySize / 2 - wallThickness;
        const int upperRight = Chunk.SIZE / 2 + hallwaySize / 2 + wallThickness;
        const int netLength = Chunk.SIZE / 2 - hallwaySize / 2 - wallThickness;

        private Map map;
        private MapData mapData;

        private static readonly Direction[] directions = { Direction.Right, Direction.Up, Direction.Left, Direction.Down };

        public void Instantiate(Map instance, MapData mapData)
        {
            this.map = instance;
            this.mapData = mapData;

            // places rooms
            foreach (var item in mapData.rooms)
            {
                Room room = item.roomData.prefab.GetComponent<Room>();
                var origin = item.origin;
                instance.Paste(origin, room);
            }

            // draw hallways
            for (int x = 0; x < mapData.RectSize.x; x++)
                for (int y = 0; y < mapData.RectSize.y; y++)
                {
                    ref Chunk chunk = ref mapData.chunks[x, y];
                    Vector2Int chunkPosition = new(x, y);

                    // maybe we want to do something with it
                    if (!chunk.isHallway) continue;

                    DrawHallwayCenter(chunkPosition);
                    // intersection space between hallway and room...
                    if (chunk.isRoom)
                    {
                    }
                    // simple case: just create the hallway according to its shape
                    else
                    {
                        foreach (var d in directions)
                        {
                            DrawDirection(chunkPosition, d, chunk.hallwayDirection.HasFlag(d));
                        }
                    }
                }
        }

        private void DrawHallwayCenter(Vector2Int chunkPosition)
        {
            var centerRect = new BoundsInt(lowerLeft, lowerLeft, 0, hallwayTotalSize, hallwayTotalSize, 1);
            centerRect.position += Chunk.ToWorld(chunkPosition);
            var arr = map.floor.GetTilesBlock(centerRect);// CreateFillArray(centerRect, mapData.levelPreset.hallwayPreset.floor);
            FillIfNull(arr, mapData.levelPreset.hallwayPreset.floor);
            map.floor.SetTilesBlock(centerRect, arr);
        }

        void DrawDirection(Vector2Int chunkPosition, Direction direction, bool hallway = true)
        {
            var floorTile = mapData.levelPreset.hallwayPreset.floor;
            TileBase geometryTile = mapData.levelPreset.hallwayPreset.geometry;

            // draw a wall
            if (!hallway)
            {
                var wallRect = direction switch
                {
                    Direction.Right => new BoundsInt(upperRight, lowerLeft, 0, 1, hallwayTotalSize, 1),
                    Direction.Up => new BoundsInt(lowerLeft, upperRight, 0, hallwayTotalSize, 1, 1),
                    Direction.Left => new BoundsInt(lowerLeft, lowerLeft, 0, 1, hallwayTotalSize, 1),
                    Direction.Down => new BoundsInt(lowerLeft, lowerLeft, 0, hallwayTotalSize, 1, 1),
                    _ => throw new ArgumentException("Direction is either composite or none"),
                };
                wallRect.position += Chunk.ToWorld(chunkPosition);
                var geometryTiles = CreateFillArray(wallRect, geometryTile);
                map.geometry.SetTilesBlock(wallRect, geometryTiles);
                return;
            }

            var floorRect = direction switch
            {
                Direction.Right => new RectInt(upperRight, lowerLeft, netLength, hallwayTotalSize),
                Direction.Up => new RectInt(lowerLeft, upperRight, hallwayTotalSize, netLength),
                Direction.Left => new RectInt(0, lowerLeft, netLength, hallwayTotalSize),
                Direction.Down => new RectInt(lowerLeft, 0, hallwayTotalSize, netLength),
                _ => throw new ArgumentException("Direction is either composite or none"),
            };
            // fill the floor
            var bounds = new BoundsInt((Vector3Int)floorRect.position, (Vector3Int)floorRect.size);
            bounds.position += Chunk.ToWorld(chunkPosition);
            //var tiles = map.floor.GetTilesBlock(bounds);
            var tiles = CreateFillArray(bounds, floorTile);
            bounds.zMax = 1;
            map.floor.SetTilesBlock(bounds, tiles);

            // geometry
            switch (direction)
            {
                case Direction.Right:
                case Direction.Left:
                    for (var x = bounds.xMin; x < bounds.xMax; x++)
                    {
                        map.geometry.SetTile(new Vector3Int(x, bounds.yMin), geometryTile);
                        map.geometry.SetTile(new Vector3Int(x, bounds.yMax), geometryTile);
                    }
                    break;
                case Direction.Up:
                case Direction.Down:
                    for (var y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        map.geometry.SetTile(new Vector3Int(bounds.xMin, y), geometryTile);
                        map.geometry.SetTile(new Vector3Int(bounds.xMax, y), geometryTile);
                    }
                    break;
                case Direction.None:
                default:
                    break;
            }
        }

        TileBase[] CreateFillArray(BoundsInt floorRect, TileBase tile)
        {
            TileBase[] tiles = new TileBase[floorRect.size.x * floorRect.size.y];
            Array.Fill(tiles, tile);
            return tiles;
        }

        void FillIfNull(TileBase[] tiles, TileBase tile)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == null) tiles[i] = tile;
            }
        }
    }
}