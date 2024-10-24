using System;
using Unity.Burst;
using UnityEngine;

namespace Safari.MapComponents.Tiles
{
    [BurstCompile(CompileSynchronously = true)]
    public struct TileRect : IEquatable<TileRect>, IFormattable
    {
        public static readonly TileRect SINGLE_TILE = new(0, 0, 1, 1);

        public Vector2Int size;
        public Vector2Int anchor;

        public TileRect(int anchorX, int anchorY, int sizeX, int sizeY) : this()
        {
            size = new Vector2Int(sizeX, sizeY);
            anchor = new Vector2Int(anchorX, anchorY);
        }

        public TileRect(Vector2Int anchor, Vector2Int size) : this()
        {
            this.size = size;
            this.anchor = anchor;
        }

        /// <summary>
        /// Return a rect represent the tile space in given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public RectInt Transform(in Vector2Int position)
        {
            return new RectInt(position - anchor, size);
        }

        public float DistanceTo(in Vector2 tilePosition, in Vector2 position) => DistanceTo(Vector2Int.FloorToInt(tilePosition), position);
        public float DistanceTo(in Vector2Int tilePosition, in Vector2 position)
        {
            var rect = Transform(tilePosition);
            var dx = Mathf.Max(rect.min.x - position.x, 0, position.x - rect.max.x);
            var dy = Mathf.Max(rect.min.y - position.y, 0, position.y - rect.max.y);
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public bool Equals(TileRect other)
        {
            return size == other.size && anchor == other.anchor;
        }

        public static explicit operator RectInt(TileRect tileRect)
        {
            return new RectInt(tileRect.anchor, tileRect.size);
        }

        public static bool operator ==(TileRect a, TileRect b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TileRect a, TileRect b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is TileRect rect && Equals(rect);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{size} {anchor}";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{size.ToString(format, formatProvider)} {anchor.ToString(format, formatProvider)}";
        }
    }
}