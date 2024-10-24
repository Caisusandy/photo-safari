using System.Collections.Generic;
using UnityEngine;
using Minerva.Module;
using Random = UnityEngine.Random;
using System.Drawing;

namespace Safari.MapComponents.Generators
{

    /// <summary>
    /// The generator
    /// </summary>
    public class MapGenerator
    {
        private const int SINGLE_ROOM_MAX_TRIAL = 100;
        private MapData mapData;
        private List<RoomPointer> rooms => mapData.rooms;

        public MapData Generate(LevelPreset levelPreset, GeneratorParameter generatorParameter)
        {
            ValidateParameter(levelPreset, generatorParameter);

            mapData = new MapData(generatorParameter.size);

            // intial
            PlaceAt(2, 2, levelPreset.initialRoom);

            // try to generate for each room
            PlaceRooms(levelPreset, generatorParameter);

            // place final room
            RunPlaceRoom(levelPreset.finalRoom);

            return mapData;
        }

        private void PlaceRooms(LevelPreset levelPreset, GeneratorParameter generatorParameter)
        {
            List<RoomPreset> rooms = new List<RoomPreset>(levelPreset.rooms);
            for (int i = 0; i < generatorParameter.length && rooms.Count > 0; i++)
            {
                var room = rooms.RandomPop();
                RunPlaceRoom(room);
            }
        }

        private void RunPlaceRoom(RoomPreset room)
        {
            RectInt rect = this.rooms[^1].Rect;
            int j = 0;
            for (; j < SINGLE_ROOM_MAX_TRIAL; j++)
            {
                // placing nearby
                Vector2Int position = PlaceRectNearby(rect, room.chunkSize).position;
                if (TryGenerateNextRoom(room, position))
                    break;
            }
            if (j >= SINGLE_ROOM_MAX_TRIAL)
            {
                Debug.LogError("A single room failed to spawn in given max trial count");
            }

            // generate a reference around the last room
            //(int, int) RandomPosition()
            //{
            //var lastRoom = this.rooms[rooms.Count - 1];
            //var min = lastRoom.origin - room.ChunkSize - Vector2.one;
            //var max = lastRoom.origin + lastRoom.roomData.ChunkSize + room.ChunkSize;
            //var rect = lastRoom.Rect;
            //var position = new Vector2Int(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));

            //while (rect.Contains(position))
            //    position = new Vector2Int(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));

            //return (position.x, position.y);
            //}
        }


        // Method to place a new RectInt next to an existing RectInt with a 1-space gap
        public static RectInt PlaceRectNearby(RectInt original, Vector2Int newSize)
        {
            // Randomly pick a side: 0 = left, 1 = right, 2 = top, 3 = bottom
            int side = Random.Range(0, 4);

            // Variable to store the new origin of the RectInt
            Vector2Int newOrigin = Vector2Int.zero;

            // Determine new origin based on the side chosen
            // Ensure the new origin is non-negative (clamping at 0)
            int minInclusiveY = Mathf.Max(0, original.yMin - newSize.y + 1);
            int minInclusiveX = Mathf.Max(0, original.xMin - newSize.x + 1);
            switch (side)
            {
                case 0: // Place to the left
                    newOrigin = new Vector2Int(Mathf.Max(0, original.xMin - newSize.x - 1), Random.Range(minInclusiveY, original.yMax));
                    break;
                case 1: // Place to the right
                    newOrigin = new Vector2Int(original.xMax + 1, Random.Range(minInclusiveY, original.yMax));
                    break;
                case 2: // Place above
                    newOrigin = new Vector2Int(Random.Range(minInclusiveX, original.xMax), original.yMax + 1);
                    break;
                case 3: // Place below
                    newOrigin = new Vector2Int(Random.Range(minInclusiveX, original.xMax), Mathf.Max(0, original.yMin - newSize.y - 1));
                    break;
            }

            // Create and return the new RectInt
            return new RectInt(newOrigin, newSize);
        }

        /// <summary>
        /// Connect two existing room by creating hallway
        /// </summary>
        /// <param name="roomPointer1"></param>
        /// <param name="roomPointer2"></param>
        private bool TryConnect(RoomPointer roomPointer1, RoomPointer roomPointer2, out List<Vector2Int> path)
        {
            var rect1 = roomPointer1.Rect;
            var rect2 = roomPointer2.Rect;
            var center1 = Vector2Int.FloorToInt(rect1.center);
            var center2 = Vector2Int.FloorToInt(rect2.center);

            Debug.Log($"From {center1} to {center2} ({rect1.center} to {rect2.center})");

            if (Random.Range(0, 2) == 1)
            {
                path = PathXFirst(center1, center2) ?? PathYFirst(center1, center2);
            }
            else path = PathYFirst(center1, center2) ?? PathXFirst(center1, center2);

            return path != null;

            // Method to generate path starting with X-axis movement
            List<Vector2Int> PathXFirst(Vector2Int start, Vector2Int end)
            {
                List<Vector2Int> path = new List<Vector2Int>();

                // Move along the X-axis first
                int v1 = (int)Mathf.Sign(end.x - start.x);
                for (int x = start.x; x != end.x; x += v1)
                {
                    Vector2Int point = new Vector2Int(x, start.y);
                    if (mapData.IsAny(point, roomPointer1, roomPointer2))
                    { }
                    else if (mapData.IsNoRoom(point))
                        path.Add(point);
                    else return null;
                }

                // Then move along the Y-axis
                int v = (int)Mathf.Sign(end.y - start.y);
                for (int y = start.y; y != end.y; y += v)
                {
                    Vector2Int point = new Vector2Int(end.x, y);
                    if (mapData.IsAny(point, roomPointer1, roomPointer2))
                    { }
                    else if (mapData.IsNoRoom(point))
                        path.Add(point);
                    else return null;
                }

                // Add the final point
                if (!mapData.IsAny(end, roomPointer1, roomPointer2))
                    path.Add(end);
                Debug.Log($"From {start} to {end}: {string.Join('|', path)}");
                return path;
            }

            // Method to generate path starting with Y-axis movement
            List<Vector2Int> PathYFirst(Vector2Int start, Vector2Int end)
            {
                List<Vector2Int> path = new List<Vector2Int>();

                // Move along the Y-axis first
                int v = (int)Mathf.Sign(end.y - start.y);
                for (int y = start.y; y != end.y; y += v)
                {
                    Vector2Int point = new Vector2Int(start.x, y);
                    if (mapData.IsAny(point, roomPointer1, roomPointer2))
                    { }
                    else if (mapData.IsNoRoom(point))
                        path.Add(point);
                    else return null;
                }

                // Then move along the X-axis
                int v1 = (int)Mathf.Sign(end.x - start.x);
                for (int x = start.x; x != end.x; x += v1)
                {
                    Vector2Int point = new Vector2Int(x, end.y);
                    if (mapData.IsAny(point, roomPointer1, roomPointer2))
                    { }
                    else if (mapData.IsNoRoom(point))
                        path.Add(point);
                    else return null;

                }

                // Add the final point
                if (!mapData.IsAny(end, roomPointer1, roomPointer2))
                    path.Add(end);
                Debug.Log($"From {start} to {end}: {string.Join('|', path)}");
                return path;
            }
        }

        /// <summary>
        /// Set every point on the path as a hallway
        /// </summary>
        /// <param name="path"></param>
        private void Connect(List<Vector2Int> path)
        {
            foreach (Vector2Int point in path)
            {
                ref var chunk = ref mapData.chunks[point.x, point.y];
                if (!chunk.isRoom) chunk.asHallway = true;
            }
        }



        private bool TryGenerateNextRoom(RoomPreset roomData, Vector2Int? position = null)
        {
            var rp = position ?? RandomPosition();
            var x = rp.x;
            var y = rp.y;

            RectInt rect = new RectInt(x, y, roomData.chunkSize.x, roomData.chunkSize.y);
            RectInt emptyBound = rect;
            emptyBound.min -= Vector2Int.one;
            emptyBound.max += Vector2Int.one;
            /// check room rect will be empty
            if (mapData.IsOutOfBound(rect) || !mapData.IsEmpty(emptyBound))
            {
                return false;
            }

            var lastRoom = rooms[^1];
            var newRoomPointer = PlaceAt(x, y, roomData);
            Debug.Log("Try connect");
            // if failed to build a path
            if (!TryConnect(lastRoom, newRoomPointer, out var path))
            {
                Remove(x, y, newRoomPointer);
                return false;
            }

            // else set the path
            Connect(path);
            return true;

            Vector2Int RandomPosition()
            {
                return new(Random.Range(0, mapData.RectSize.x), Random.Range(0, mapData.RectSize.y));
            }
        }


        /// <summary>
        /// Place the room at the position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="roomData"></param>
        private RoomPointer PlaceAt(int x, int y, RoomPreset roomData)
        {
            var pointer = new RoomPointer() { origin = new Vector2Int(x, y), roomData = roomData };
            rooms.Add(pointer);
            for (int i = 0; i < roomData.chunkSize.x; i++)
                for (int j = 0; j < roomData.chunkSize.y; j++)
                {
                    mapData.chunks[i + x, j + y] = new Chunk() { instancePointer = pointer };
                }
            return pointer;
        }

        /// <summary>
        /// Remove a room from it
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="roomPointer"></param>
        private void Remove(int x, int y, RoomPointer roomPointer)
        {
            for (int i = 0; i < roomPointer.roomData.chunkSize.x; i++)
                for (int j = 0; j < roomPointer.roomData.chunkSize.y; j++)
                {
                    if (roomPointer == mapData.chunks[i + x, j + y].instancePointer)
                        mapData.chunks[i + x, j + y] = new Chunk();
                }
        }



        /// <summary>
        /// Do a test about level preset with parameter is valid
        /// </summary>
        /// <param name="levelPreset"></param>
        /// <param name="generatorParameter"></param>
        private void ValidateParameter(LevelPreset levelPreset, GeneratorParameter generatorParameter)
        {

        }
    }
}