using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Safari.MapComponents
{
    [CreateAssetMenu(menuName = "Safari/Room Data")]
    public class RoomPreset : ScriptableObject
    {
        public GameObject prefab;
        [FormerlySerializedAs("ChunkSize")]
        public Vector2Int chunkSize;
        public string roomName;

        public void SetPrefab(GameObject prefab)
        {
            this.prefab = prefab;
        }

        public void SetShapeSize(Vector2Int chunkSize)
        {
            this.chunkSize = chunkSize;
        }
    }
}