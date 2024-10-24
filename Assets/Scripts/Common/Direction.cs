using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using static Safari.Direction;

namespace Safari
{
    /// <summary>
    /// The direction that a room can open a gate
    /// </summary>
    [Flags]
    public enum Direction
    {
        None = 0,
        Right = 1,
        Up = 2,
        Left = 4,
        Down = 8,
    }

    [BurstCompile(CompileSynchronously = true)]
    public static class DirectionExtensions
    {
        //static Direction[] oppositeTable = new Direction[] {
        //    None,
        //    Left,
        //    Down,
        //    Down | Left,
        //    Right,
        //    Right | Left,
        //    Right | Down,
        //    Right | Down | Left,
        //    Up,
        //    Up | Left,
        //    Up | Down,
        //    Up | Down | Left,
        //    Up | Right,
        //    Up | Right | Left,
        //    Up | Right | Down,
        //    Up | Right | Down | Left,
        //};


        /// <summary>
        /// Get Opposite Direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        [BurstCompile(CompileSynchronously = true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction Opposite(this Direction direction)
        {
            return direction switch
            {
                // 1
                None => None,
                Right => Left,
                Up => Down,
                Left => Right,
                Down => Up,
                // 2 normal: 
                Right | Up => Left | Down,
                Right | Down => Left | Up,
                Left | Up => Right | Down,
                Left | Down => Right | Up,
                // 3
                Up | Right | Left => Left | Right | Down,
                Down | Right | Left => Left | Right | Up,
                Right | Up | Down => Left | Up | Down,
                Left | Up | Down => Right | Up | Down,
                // 4 
                Left | Right | Up | Down => Left | Right | Up | Down,
                // 2 opposite:
                Right | Left or Up | Down => direction,
                _ => None,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SignX(this Direction direction)
        {
            if (direction.HasFlag(Left)) return -1;
            if (direction.HasFlag(Right)) return 1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SignY(this Direction direction)
        {
            if (direction.HasFlag(Down)) return -1;
            if (direction.HasFlag(Up)) return 1;
            return 0;
        }




        /// <summary>
        /// Get the direction vector by <paramref name="direction"/>
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2(this Direction direction) { direction.ToVector2(out var result); return result; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile(CompileSynchronously = true)]
        public static void ToVector2(this Direction direction, out Vector2Int result)
        {
            switch (direction)
            {
                case Right:
                    result = Vector2Int.right;
                    break;
                case Up:
                    result = Vector2Int.up;
                    break;
                case Left:
                    result = Vector2Int.left;
                    break;
                case Down:
                    result = Vector2Int.down;
                    break;
                case Up | Right:
                    result = new Vector2Int(-1, 1);
                    break;
                case Up | Left:
                    result = new Vector2Int(1, 1);
                    break;
                case Down | Right:
                    result = new Vector2Int(1, -1);
                    break;
                case Down | Left:
                    result = new Vector2Int(-1, -1);
                    break;
                case None:
                    result = Vector2Int.zero;
                    break;
                default:
                    throw new ArgumentException("not a valid direction");
            }
        }

        /// <summary>
        /// Get the direction vector by <paramref name="direction"/>
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3(this Direction direction) { direction.ToVector3(out var result); return result; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile(CompileSynchronously = true)]
        public static void ToVector3(this Direction direction, out Vector3Int result)
        {
            switch (direction)
            {
                case Right:
                    result = Vector3Int.right;
                    break;
                case Up:
                    result = Vector3Int.up;
                    break;
                case Left:
                    result = Vector3Int.left;
                    break;
                case Down:
                    result = Vector3Int.down;
                    break;
                case Up | Right:
                    result = new Vector3Int(-1, 1);
                    break;
                case Up | Left:
                    result = new Vector3Int(1, 1);
                    break;
                case Down | Right:
                    result = new Vector3Int(1, -1);
                    break;
                case Down | Left:
                    result = new Vector3Int(-1, -1);
                    break;
                case None:
                    result = Vector3Int.zero;
                    break;
                default:
                    throw new ArgumentException("not a valid direction");
            }
        }





        /// <summary>
        /// Rotate the Direction by 90 degree
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction RotateCounterClockwise(this Direction direction)
        {
            return direction switch
            {
                Right => Up,
                Up => Left,
                Left => Down,
                Down => Right,
                _ => None,
            };
        }

        /// <summary>
        /// Rotate the Direction by -90 degree
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction RotateClockwise(this Direction direction)
        {
            return direction switch
            {
                Right => Down,
                Up => Right,
                Left => Up,
                Down => Left,
                _ => None,
            };
        }




        /// <summary>
        /// Get all single value of the Direction
        /// <para> You can also you Enum.GetValues to do exact same thing, this is a shortcut only</para> 
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction[] SingleValues(this Direction direction)
        {
            return direction.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SingleValues(this Direction direction, Span<Direction> directions)
        {
            var count = 0;
            if ((direction & Left) != 0)
            {
                directions[count++] = Left;
            }
            if ((direction & Right) != 0)
            {
                directions[count++] = Right;
            }
            if ((direction & Up) != 0)
            {
                directions[count++] = Up;
            }
            if ((direction & Down) != 0)
            {
                directions[count++] = Down;
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile(CompileSynchronously = true)]
        public static void SingleValues(this Direction direction, out NativeArray<Direction> directions, Allocator allocator = Allocator.Temp)
        {
            Span<Direction> flags = stackalloc Direction[4];
            var count = SingleValues(direction, flags);
            directions = new NativeArray<Direction>(count, allocator);
            for (int i = 0; i < count; i++)
            {
                directions[i] = flags[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<Direction> ToNativeArray(this Direction direction, Allocator allocator = Allocator.Temp)
        {
            direction.SingleValues(out var directions, allocator);
            return directions;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction[] ToArray(this Direction direction)
        {
            direction.SingleValues(out var directions, Allocator.Temp);
            using (directions) return directions.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile(CompileSynchronously = true)]
        /// <summary>
        /// Convert direction to a pivot
        /// </summary>
        /// <param name="direction">The direction</param> 
        /// <returns></returns>
        public static Pivot ToPivot(this Direction direction)
        {
            switch (direction)
            {
                case Up | Left:
                    return Pivot.UpperRight;
                case Up:
                    return Pivot.UpperCenter;
                case Up | Right:
                    return Pivot.UpperRight;

                case Left:
                    return Pivot.MiddleLeft;
                case None:
                    return Pivot.MiddleCenter;
                case Right:
                    return Pivot.MiddleRight;

                case Down | Left:
                    return Pivot.LowerLeft;
                case Down:
                    return Pivot.LowerCenter;
                case Down | Right:
                    return Pivot.LowerRight;

                default:
                    break;
            }
            return Pivot.UpperLeft;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile(CompileSynchronously = true)]
        /// <summary>
        /// Get the rotation degree of the Direction from the start Direction
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <param name="startWith">The starting direction</param>
        /// <returns></returns>
        public static float ToDegree(this Direction direction, Direction startWith)
        {
            return ToDegree(direction) - ToDegree(startWith);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile(CompileSynchronously = true)]
        /// <summary>
        /// Convert Direction to degree (let east as 0)
        /// </summary>
        /// <param name="direction">The direction</param> 
        /// <returns></returns>
        private static float ToDegree(Direction direction)
        {
            return direction switch
            {
                Right => 0,
                Up => 90,
                Left => 180,
                Down => 270,
                _ => (float)0,
            };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile(CompileSynchronously = true)]
        /// <summary>
        /// Convert Direction to rad (let east as 0)
        /// </summary>
        /// <param name="direction">The direction</param> 
        /// <returns></returns>
        public static float ToRad(this Direction direction, Direction startWith = Right)
        {
            return math.radians(ToDegree(direction, startWith));
        }






        /// <summary>
        /// The approximation used for Approximatly
        /// </summary>
        public enum ApproximationMode
        {
            four,
            eight,
        }

        /// <summary>
        /// Approximate a vector to Direction
        /// </summary>
        /// <param name="vector2">The vector</param>
        /// <param name="mode">four for single direction, eight allows NE, SE, SW, NW</param>
        /// <returns></returns>
        [BurstCompile(CompileSynchronously = true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction Approximatly(this in Vector2 vector2, ApproximationMode mode = ApproximationMode.four)
        {
            var normalized = vector2.normalized;
            switch (mode)
            {
                case ApproximationMode.four:
                    if (normalized.x > normalized.y)
                    {
                        return normalized.x switch
                        {
                            > 0 => Right,
                            < 0 => Left,
                            _ => None,
                        };
                    }
                    else
                    {
                        return normalized.y switch
                        {
                            > 0 => Up,
                            < 0 => Down,
                            _ => None
                        };
                    }
                case ApproximationMode.eight:
                    Direction verticle;
                    if (normalized.y > Mathf.Sin(Mathf.PI / 8))
                        verticle = Up;
                    else if (normalized.y < -Mathf.Sin(Mathf.PI / 8))
                        verticle = Down;
                    else verticle = None;

                    Direction horizontal;
                    if (normalized.x > Mathf.Sin(Mathf.PI / 8))
                        horizontal = Right;
                    else if (normalized.x < -Mathf.Sin(Mathf.PI / 8))
                        horizontal = Left;
                    else horizontal = None;
                    return verticle | horizontal;
                default:
                    break;
            }
            return None;
        }

        public static Direction FromVector(Vector2Int vector2Int)
        {
            Direction result = None;
            if (vector2Int.x < 0) result |= Left;
            else if (vector2Int.x > 0) result |= Right;
            if (vector2Int.y < 0) result |= Down;
            else if (vector2Int.y > 0) result |= Up;
            return result;
        }
    }
}