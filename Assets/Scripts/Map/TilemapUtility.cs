using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.MapComponents
{
    public static class TilemapUtility
    {
        public static void Paste(this Tilemap dst, Tilemap source, Vector3Int offset)
        {
            var bounds = source.cellBounds;
            var count = source.GetTilesRangeCount(bounds.min, bounds.max);
            var positions = new Vector3Int[count];
            var tiles = new TileBase[count];
            Debug.Log(count + " " + offset);
            // get tiles
            source.GetTilesRangeNonAlloc(bounds.min, bounds.max, positions, tiles);
            // update position
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] += offset;
            }
            // set tiles
            dst.SetTiles(positions, tiles);
        }
    }
}