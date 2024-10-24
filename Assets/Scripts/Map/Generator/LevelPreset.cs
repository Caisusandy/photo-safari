using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.MapComponents.Generators
{
    [CreateAssetMenu(menuName = "Safari/Level Preset")]
    public class LevelPreset : ScriptableObject
    {
        public RoomPreset initialRoom;
        public RoomPreset[] rooms;
        public RoomPreset finalRoom;

        public HallwayPreset hallwayPreset;
    }

    public class HallwayPreset
    {
        public TileBase floor;
        public TileBase geometry;
    }
}