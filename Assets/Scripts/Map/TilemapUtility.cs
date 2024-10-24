using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.MapComponents
{
    public static class TilemapUtility
    {
        public static void Paste(this Tilemap dst, Tilemap source, Vector3Int offset)
        {
            var count = source.GetUsedTilesCount();
            var positions = new Vector3Int[count];
            var tiles = new TileBase[count];
            var bounds = source.cellBounds;
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] += offset;
            }
            source.GetTilesRangeNonAlloc(bounds.min, bounds.max, positions, tiles);
            dst.SetTiles(positions, tiles);
        }
    }
}