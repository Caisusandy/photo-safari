using UnityEngine;

namespace Safari.MapComponents.Generators
{
    [CreateAssetMenu(menuName = "Safari/Level Preset")]
    public class LevelPreset : ScriptableObject
    {
        public RoomData initialRoom;
        public RoomData[] rooms;
        public RoomData finalRoom;
    }
}