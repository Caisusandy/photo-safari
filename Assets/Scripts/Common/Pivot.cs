using Unity.Burst;
using UnityEngine;

namespace Safari
{
    public enum Pivot
    {
        //
        // Summary:
        //     anchored in upper left corner.
        UpperLeft,
        //
        // Summary:
        //     anchored in upper side, centered horizontally.
        UpperCenter,
        //
        // Summary:
        //     anchored in upper right corner.
        UpperRight,
        //
        // Summary:
        //     anchored in left side, centered vertically.
        MiddleLeft,
        //
        // Summary:
        //     centered both horizontally and vertically.
        MiddleCenter,
        //
        // Summary:
        //     anchored in right side, centered vertically.
        MiddleRight,
        //
        // Summary:
        //     anchored in lower left corner. 
        LowerLeft,
        //
        // Summary:
        //     anchored in lower side, centered horizontally.
        LowerCenter,
        //
        // Summary:
        //     anchored in lower right corner.
        LowerRight
    }

    [BurstCompile(CompileSynchronously = true)]
    public static class Pivots
    {
        /// <summary>
        /// Get the normalized pivot (0~1)
        /// </summary>
        /// <param name="pivot"></param>
        /// <returns></returns>
        public static Vector2 Normalize(this Pivot pivot)
        {
            switch (pivot)
            {
                case Pivot.UpperLeft:
                    return new Vector2(0, 1);
                case Pivot.UpperCenter:
                    return new Vector2(0.5f, 1);
                case Pivot.UpperRight:
                    return new Vector2(1, 1);
                case Pivot.MiddleLeft:
                    return new Vector2(0, 0.5f);
                case Pivot.MiddleCenter:
                    return new Vector2(0.5f, 0.5f);
                case Pivot.MiddleRight:
                    return new Vector2(1, 0.5f);
                case Pivot.LowerLeft:
                    return new Vector2(0, 0);
                case Pivot.LowerCenter:
                    return new Vector2(0.5f, 0);
                case Pivot.LowerRight:
                    return new Vector2(1, 0);
                default:
                    return new Vector2(0.5f, 0.5f);
            }
        }

        /// <summary>
        /// Get the reversed normalized pivot (note that Pivot.UpperLeft means pivot in upper left, which mean the element is in lower right)
        /// </summary>
        /// <param name="pivot"></param>
        /// <returns></returns>
        public static Vector2 ReverseNormalize(this Pivot pivot)
        {
            return Vector2.one - pivot.Normalize();
        }

        /// <summary>
        /// Get the Unit pivot (-1 ~ 1)
        /// </summary>
        /// <param name="pivot"></param>
        /// <returns></returns>
        public static Vector2 Unit(this Pivot pivot)
        {
            switch (pivot)
            {
                case Pivot.UpperLeft:
                    return new Vector2(-1, 1);
                case Pivot.UpperCenter:
                    return new Vector2(0, 1);
                case Pivot.UpperRight:
                    return new Vector2(1, 1);
                case Pivot.MiddleLeft:
                    return new Vector2(-1, 0);
                case Pivot.MiddleCenter:
                    return new Vector2(0, 0);
                case Pivot.MiddleRight:
                    return new Vector2(1, 0);
                case Pivot.LowerLeft:
                    return new Vector2(-1, -1);
                case Pivot.LowerCenter:
                    return new Vector2(0, -1);
                case Pivot.LowerRight:
                    return new Vector2(1, -1);
                default:
                    return new Vector2(0, 0);
            }

        }

        public static Vector2 ConvertOffset(this Pivot pivot, Vector2 offset)
        {
            return offset * pivot.Unit();
        }

        public static RectInt GetRect(this Pivot pivot, in Vector2Int pos, in Vector2Int size)
        {
            pivot.GetRect(in pos, in size, out var rect);
            return rect;
        }

        [BurstCompile(CompileSynchronously = true)]
        public static void GetRect(this Pivot pivot, in Vector2Int pos, in Vector2Int size, out RectInt rect)
        {
            rect = new RectInt(pos, size);
            switch (pivot)
            {
                case Pivot.UpperLeft:
                    rect.position = new Vector2Int(pos.x, pos.y - size.y + 1); break;
                case Pivot.UpperCenter:
                    rect.position = new Vector2Int(pos.x - size.x / 2, pos.y - size.y + 1); break;
                case Pivot.UpperRight:
                    rect.position = new Vector2Int(pos.x - size.x + 1, pos.y - size.y + 1); break;
                case Pivot.MiddleLeft:
                    rect.position = new Vector2Int(pos.x, pos.y - size.y / 2); break;
                case Pivot.MiddleCenter:
                    rect.position = new Vector2Int(pos.x - size.x / 2, pos.y - size.y / 2); break;
                case Pivot.MiddleRight:
                    rect.position = new Vector2Int(pos.x - size.x + 1, pos.y - size.y / 2); break;

                //offset default mode: lower right
                case Pivot.LowerLeft:
                    return;

                case Pivot.LowerCenter:
                    rect.position = new Vector2Int(pos.x - size.x / 2, pos.y);
                    break;
                case Pivot.LowerRight:
                    rect.position = new Vector2Int(pos.x - size.x + 1, pos.y); break;
                default:
                    break;
            }
        }

        public static BoundsInt GetBounds(this Pivot pivot, in Vector3Int pos, in Vector3Int size)
        {
            pivot.GetBounds(in pos, in size, out var rect);
            return rect;
        }

        public static BoundsInt GetBounds(this Pivot pivot, in Vector3Int pos, in Vector2Int size)
        {
            var d3Size = new Vector3Int(size.x, size.y, 1);
            pivot.GetBounds(in pos, in d3Size, out var rect);
            return rect;
        }

        [BurstCompile(CompileSynchronously = true)]
        public static void GetBounds(this Pivot pivot, in Vector3Int pos, in Vector3Int size, out BoundsInt rect)
        {
            rect = new BoundsInt(pos, size);
            switch (pivot)
            {
                case Pivot.UpperLeft:
                    rect.position = new Vector3Int(pos.x, pos.y - size.y + 1, pos.z); break;
                case Pivot.UpperCenter:
                    rect.position = new Vector3Int(pos.x - size.x / 2, pos.y - size.y + 1, pos.z); break;
                case Pivot.UpperRight:
                    rect.position = new Vector3Int(pos.x - size.x + 1, pos.y - size.y + 1, pos.z); break;
                case Pivot.MiddleLeft:
                    rect.position = new Vector3Int(pos.x, pos.y - size.y / 2, pos.z); break;
                case Pivot.MiddleCenter:
                    rect.position = new Vector3Int(pos.x - size.x / 2, pos.y - size.y / 2, pos.z); break;
                case Pivot.MiddleRight:
                    rect.position = new Vector3Int(pos.x - size.x + 1, pos.y - size.y / 2, pos.z); break;

                //offset default mode: lower right
                case Pivot.LowerLeft:
                    return;

                case Pivot.LowerCenter:
                    rect.position = new Vector3Int(pos.x - size.x / 2, pos.y, pos.z);
                    break;
                case Pivot.LowerRight:
                    rect.position = new Vector3Int(pos.x - size.x + 1, pos.y, pos.z); break;
                default:
                    break;
            }
        }
    }
}