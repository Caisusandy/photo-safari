using Minerva.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Safari.MapComponents.Tiles
{
    [Serializable]
    public class RandomModule
    {
        public RandomOption option;
        [DisplayIf(nameof(option), RandomOption.stable)][Range(0, 1)] public float scale;
    }

    public enum RandomOption
    {
        none,
        stable,
        unstable,
    }

    public static class TileRandomOptions
    {
        public static T Randomize<T>(this RandomOption option, List<T> list, Vector3Int position, float scale)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }
            int index;
            switch (option)
            {
                case RandomOption.none:
                    return list[0];
                case RandomOption.stable:
                    index = Mathf.Clamp(Mathf.FloorToInt(TileUtility.GetPerlinValue(position, scale, 1000f) * list.Count), 0, list.Count - 1);
                    break;
                case RandomOption.unstable:
                    index = UnityEngine.Random.Range(0, list.Count);
                    break;
                default:
                    return default;
            }
            return list[index];
        }


        public static T Randomize<T>(this RandomOption option, T[] list, Vector3Int position, float scale)
        {
            if (list == null || list.Length == 0)
            {
                return default;
            }
            int index;
            switch (option)
            {
                case RandomOption.none:
                    return list[0];
                case RandomOption.stable:
                    index = Mathf.Clamp(Mathf.FloorToInt(TileUtility.GetPerlinValue(position, scale, 1000f) * list.Length), 0, list.Length - 1);
                    break;
                case RandomOption.unstable:
                    index = UnityEngine.Random.Range(0, list.Length);
                    break;
                default:
                    return default;
            }
            return list[index];
        }

        public static int Randomize(this RandomOption option, Vector3Int position, int minInclusive, int maxExclusive, float scale)
        {
            if (minInclusive == maxExclusive)
            {
                return minInclusive;
            }
            // reverse
            if (minInclusive > maxExclusive)
            {
                minInclusive ^= maxExclusive;   // a = a ^ b,           b = b
                maxExclusive ^= minInclusive;   // a = a ^ b,           b = b ^ a ^ b = a
                minInclusive ^= maxExclusive;   // a = a ^ b ^ a = b,   b = a
            }
            return option switch
            {
                RandomOption.none => minInclusive,
                RandomOption.stable => (int)Mathf.Lerp(minInclusive, maxExclusive, TileUtility.GetPerlinValue(position, scale, 1000f)),
                RandomOption.unstable => UnityEngine.Random.Range(minInclusive, maxExclusive),
                _ => minInclusive,
            };
        }

        public static bool Boolean(this RandomOption option, Vector3Int position, float scale)
        {
            return option switch
            {
                RandomOption.none => false,
                RandomOption.stable => Mathf.FloorToInt(TileUtility.GetPerlinValue(position, scale, 1000f)) > 0.5f,
                RandomOption.unstable => UnityEngine.Random.value > 0.5f,
                _ => false,
            };
        }
    }
}