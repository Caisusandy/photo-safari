using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.MapComponents
{
    public class Map : MonoBehaviour
    {
        public Tilemap geometry;
        public Tilemap floor;
        public Tilemap decor;
        internal MapData data;

        private void OnDrawGizmosSelected()
        {
            DrawGrid();
        }

        public static void DrawGrid()
        {
            var screen = new Vector2(Screen.width, Screen.height);
            //Debug.Log(screen);
#if UNITY_EDITOR
            var max = UnityEditor.SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(screen);
            var min = UnityEditor.SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(Vector3.zero);

            var fixedMax = Vector2Int.CeilToInt(max / Chunk.SIZE);
            var fixedMin = Vector2Int.FloorToInt(min / Chunk.SIZE);

            for (int x = fixedMin.x; x < fixedMax.x; x++)
            {
                Gizmos.DrawLine(new Vector3(x * Chunk.SIZE, fixedMin.y * Chunk.SIZE, 0), new Vector3(x * Chunk.SIZE, fixedMax.y * Chunk.SIZE, 0));
            }
            for (int y = fixedMin.y; y < fixedMax.y; y++)
            {
                Gizmos.DrawLine(new Vector3(fixedMin.x * Chunk.SIZE, y * Chunk.SIZE, 0), new Vector3(fixedMax.x * Chunk.SIZE, y * Chunk.SIZE, 0));
            }
#endif
        }

        /// <summary>
        /// Paste a room to the map
        /// </summary>
        /// <param name="chunkOrigin"></param>
        /// <param name="room"></param>
        public void Paste(Vector2Int chunkOrigin, Room room)
        {
            Vector3Int offset = Chunk.ToWorld(chunkOrigin);
            geometry.Paste(room.geometry, offset);
            floor.Paste(room.floor, offset);
            decor.Paste(room.decor, offset);
        }

    }
}