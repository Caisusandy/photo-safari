using UnityEngine;

namespace Safari.MapComponents.Generators
{
    public class MapGeneratorTester : MonoBehaviour
    {
        public LevelPreset levelPreset;
        public GeneratorParameter parameter;

        [ContextMenu(nameof(Run))]
        public void Run()
        {
            var generator = new MapGenerator();
            var mapData = generator.Generate(levelPreset, parameter);
            Debug.Log(mapData);
        }
    }
}