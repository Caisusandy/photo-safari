using UnityEngine;

namespace Safari.MapComponents
{
    public class Map : MonoBehaviour
    {
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
    }
}