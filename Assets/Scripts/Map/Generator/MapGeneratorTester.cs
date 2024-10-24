using UnityEngine;

namespace Safari.MapComponents.Generators
{
    public class MapGeneratorTester : MonoBehaviour
    {
        public LevelPreset levelPreset;
        public Map map;
        public GeneratorParameter parameter;

        [ContextMenu(nameof(Run))]
        public void Run()
        {
            var generator = new MapGenerator();
            var mapData = generator.Generate(levelPreset, parameter);
            Debug.Log(mapData);
        }

        public void RunAndInstantiate()
        {
            var generator = new MapGenerator();
            var mapData = generator.Generate(levelPreset, parameter);
            Debug.Log(mapData);
            var instantiator = new MapInstantiator();
            instantiator.Instantiate(map, mapData);
        }
    }
}