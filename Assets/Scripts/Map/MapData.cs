﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Safari.MapComponents
{
    /// <summary>
    /// Map data
    /// </summary>
    public class MapData
    {
        public Chunk[,] chunks;
        public List<RoomPointer> rooms = new();

        /// <summary>
        /// Size of the map
        /// </summary>
        public Vector2Int RectSize => new Vector2Int(chunks.GetLength(0), chunks.GetLength(1));

        public ref Chunk this[int x, int y] => ref chunks[x, y];


        public MapData(Vector2Int size)
        {
            this.chunks = new Chunk[size.x, size.y];
        }



        public bool IsOutOfBound(RectInt rect)
        {
            var size = this.RectSize;
            return rect.xMin < 0 || rect.yMin < 0 || rect.xMax >= size.x || rect.yMax >= size.y;
        }

        public bool IsEmpty(RectInt rect, bool outOfBoundAsFalse = false)
        {
            var size = this.RectSize;
            if (outOfBoundAsFalse && (rect.xMin < 0 || rect.yMin < 0 || rect.xMax >= size.x || rect.yMax >= size.y)) return false;
            var fixedRect = rect;
            fixedRect.xMin = Mathf.Max(fixedRect.xMin, 0);
            fixedRect.yMin = Mathf.Max(fixedRect.yMin, 0);
            fixedRect.xMax = Mathf.Min(fixedRect.xMax, this.RectSize.x);
            fixedRect.yMax = Mathf.Min(fixedRect.yMax, this.RectSize.y);
            foreach (var p in fixedRect.allPositionsWithin)
            {
                if (!IsEmpty(p)) return false;
            }
            return true;
        }

        public bool IsEmpty(Vector2Int rect)
        {
            return !chunks[rect.x, rect.y].isOccupied;
        }

        internal bool IsNoRoom(Vector2Int point)
        {
            return !chunks[point.x, point.y].isRoom;
        }

        internal bool IsAny(Vector2Int point, params RoomPointer[] roomPointer)
        {
            return roomPointer.Any(p => p == chunks[point.x, point.y].instancePointer);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var size = this.RectSize;
            for (int y = size.y - 1; y >= 0; y--)
            {
                for (int x = 0; x < size.x; x++)
                {
                    sb.Append(this[x, y].asHallway ? "H" : (this[x, y].isRoom ? "R" : "O"));
                    sb.Append('\t');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// A chunk
    /// </summary>
    public struct Chunk
    {
        public const int SIZE = 16;

        public RoomPointer instancePointer;
        public bool asHallway;

        public bool isOccupied => isRoom || asHallway;
        public bool isRoom => instancePointer != null;
        public Room room => instancePointer?.room;
    }
}