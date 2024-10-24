using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.MapComponents
{
    public class Room : MonoBehaviour
    {
        public RoomPreset preset;

        public Tilemap geometry;
        public Tilemap floor;
        public Tilemap decor;

        public Tilemap Geometry => geometry;
        public Tilemap Floor => floor;




        private void OnDrawGizmos()
        {
            Map.DrawGrid();
        }

        public void CreateAllLayer()
        {
            GenerateFor("Geometry", ref geometry, 2);
            GenerateFor("Floor", ref floor, 1);
            GenerateFor("Decor", ref decor, 0);
        }

        private void GenerateFor(string name, ref Tilemap tilemap, int layer)
        {
            if (tilemap == null)
            {
                var newGO = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
                var newTransform = newGO.transform;
                newTransform.SetParent(transform);
                tilemap = newGO.GetComponent<Tilemap>();
                TilemapRenderer tilemapRenderer = newGO.GetComponent<TilemapRenderer>();
                tilemapRenderer.sortingLayerID = layer;
            }
        }
    }
}