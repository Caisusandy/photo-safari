using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.MapComponents.Tiles
{
    /// <summary>
    /// The common interface of all tiles used in the game
    /// </summary>
    public interface ISafariTile
    {
#if UNITY_EDITOR
        /// <summary> The tile group (IN EDITOR) </summary>
        List<string> GroupData { get; }
#endif 
        /// <summary> The tile group (IN RUNTIME) </summary>
        HashSet<string> Group { get; }
        /// <summary> Is tile a standard tile? (base shape & construct of a room that allow to be replaced by theme preset)</summary>
        bool IsStandardTile { get; }
        string name { get; }
        public Sprite sprite { get; }


        /// <summary> Is tile has descriptive informaton? </summary>
        bool GetDescriptive(Vector3Int position, Tilemap tilemap);

        /// <summary> Get rect of the tile </summary>
        TileRect GetRect(Vector3Int position, Tilemap tilemap);




        public float DistanceTo(Vector2 position, Vector3Int tileLocalPosition, Tilemap tilemap)
        {
            var rect = GetRect(tileLocalPosition, tilemap);
            var tilePosition = Vector2Int.FloorToInt(tilemap.transform.position + tileLocalPosition);
            return rect.DistanceTo(tilePosition, position);
        }
        public float DistanceTo(Vector2 position, Vector3Int tileGlobalPosition)
        {
            var rect = GetRect(tileGlobalPosition, null);
            Vector2Int tilePosition = (Vector2Int)tileGlobalPosition;
            return rect.DistanceTo(tilePosition, position);
        }
    }
}