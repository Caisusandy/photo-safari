using UnityEngine;

namespace Safari.MapComponents.Generators
{
    public class MapGeneratorRunner : MonoBehaviour
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

        [ContextMenu(nameof(RunAndInstantiate))]
        public void RunAndInstantiate()
        {
            var generator = new MapGenerator();
            var mapData = generator.Generate(levelPreset, parameter);
            Debug.Log(mapData);
            var instantiator = new MapInstantiator();
            instantiator.Instantiate(map, mapData);
            map.data = mapData;
        }

        [ContextMenu(nameof(ResetMapComponent))]
        public void ResetMapComponent()
        {
            map.geometry.ClearAllTiles();
            map.floor.ClearAllTiles();
            map.decor.ClearAllTiles();
        }
    }
}