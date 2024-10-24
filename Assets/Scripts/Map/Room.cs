using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.MapComponents
{
    public class Room : MonoBehaviour
    {
        public Tilemap geometry;
        public Tilemap floor;
        public Tilemap decor;

        public Tilemap Geometry => geometry;
        public Tilemap Floor => floor;

        public void CreateAllLayer()
        {
            GenerateFor("Geometry", ref geometry);
            GenerateFor("Floor", ref floor);
            GenerateFor("Decor", ref decor);
        }

        private void GenerateFor(string name, ref Tilemap tilemap)
        {
            if (tilemap == null)
            {
                var newGO = new GameObject(name, typeof(Tilemap));
                var newTransform = newGO.transform;
                tilemap = newGO.GetComponent<Tilemap>();
            }
        }
    }
}