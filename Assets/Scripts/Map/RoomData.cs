using UnityEngine;

namespace Safari.MapComponents
{
    [CreateAssetMenu(menuName = "Safari/Room Data")]
    public class RoomData : ScriptableObject
    {
        public GameObject prefab;
        public Vector2Int ChunkSize;
    }
}