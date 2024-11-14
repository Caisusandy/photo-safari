using Safari.MapComponents.Tiles;
using System.Collections.Generic;
using UnityEngine;

namespace Safari.MapComponents.Generators
{
    [CreateAssetMenu(menuName = "Safari/Level Preset")]
    public class LevelPreset : ScriptableObject
    {
        public RoomPreset initialRoom;
        public RoomPreset[] rooms;
        public RoomPreset finalRoom;

        public HallwayPreset hallwayPreset;
        public List<SafariRuleTile> decors;
    }
}