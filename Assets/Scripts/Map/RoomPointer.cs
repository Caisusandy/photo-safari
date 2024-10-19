using System;
using UnityEngine;

namespace Safari.MapComponents
{
    /// <summary>
    /// A pointer to the room instance
    /// </summary>
    [Serializable]
    public class RoomPointer
    {
        public Vector2Int origin;
        public RoomData roomData;
        public Room room;

        public RectInt Rect => new RectInt(origin, roomData.ChunkSize);
    }
}