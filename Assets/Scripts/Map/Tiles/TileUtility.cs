using Unity.Burst;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.MapComponents.Tiles
{
    [BurstCompile(CompileSynchronously = true)]
    public static class TileUtility
    {
        public static TileRect GetRect(this ISafariTile libraryTile) => libraryTile.GetRect(default, default);




        /// <summary>
        /// Get the tile rect by given sprite
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static TileRect GetTileRect(Sprite sprite)
        {
            if (!sprite)
            {
                return TileRect.SINGLE_TILE;
            }
            GetTileRect((float)sprite.pixelsPerUnit, sprite.rect, sprite.pivot, out var result);
            return result;
        }

        /// <summary>
        /// Get the tile rect by given sprite
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        [BurstCompile(CompileSynchronously = true)]
        public static void GetTileRect(float pixelsPerUnit, in Rect rect, in Vector2 pivot, out TileRect result)
        {
            var size = Vector2Int.RoundToInt(rect.size / pixelsPerUnit);
            var anchor = Vector2Int.FloorToInt(pivot / pixelsPerUnit);
            result = new TileRect(anchor, size);
        }

        /// <summary>
        /// Returns a Perlin Noise value based on the given inputs.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="scale">The Perlin Scale factor of the Tile.</param>
        /// <param name="offset">Offset of the Tile on the Tilemap.</param>
        /// <returns>A Perlin Noise value based on the given inputs.</returns>
        public static float GetPerlinValue(Vector3Int position, float scale, float offset)
        {
            return Mathf.PerlinNoise((position.x + position.z + offset) * scale, (position.y + position.z + offset) * scale);
        }

        public static void SetGameObjectPosition(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            if (go)
            {
                position.z = 0;
                Vector3 vector3 = tilemap.GetComponent<Transform>().position + position;

                //var sprite = tilemap.GetSprite(position);
                //var size = GetTileRect(sprite);
                vector3.x += 0.5f;
                vector3.y += 0.5f;
                go.transform.position = vector3;

                //var local = go.transform.localPosition;
                //local.z = 0;
                //go.transform.localPosition = local;
            }
        }

        public static void SetRotation(Vector3Int position, RandomModule rotation, ref TileData tileData)
        {
            //rotation
            if (rotation.option == RandomOption.none) return;
            var deg = rotation.option.Randomize(position, 0, 4, rotation.scale) * 90;
            tileData.flags |= TileFlags.LockTransform;
            tileData.transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, -deg), Vector3.one);
        }

        public static void SetFlip(Vector3Int position, RandomModule rotation, ref TileData tileData)
        {
            //rotation
            if (rotation.option == RandomOption.none) return;
            var flipX = rotation.option.Boolean(position, rotation.scale);
            var flipY = rotation.option.Boolean(position, rotation.scale);
            tileData.flags |= TileFlags.LockTransform;
            Vector3 scale = Vector3.one;
            scale.x = flipX ? -1 : 1;
            scale.y = flipY ? -1 : 1;
            tileData.transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0), scale);
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/Tilemap/Print all missing tile")]
        public static void CreateScriptable(UnityEditor.MenuCommand menuCommand)
        {
            var tilemap = menuCommand.context as Tilemap;
            foreach (var item in tilemap.cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(item);
                if (!tile)
                {
                    continue;
                }
                if (tile is not ISafariTile)
                {
                    Debug.Log(item + " is not a Library tile");
                }
            }
        }
#endif
    }

}