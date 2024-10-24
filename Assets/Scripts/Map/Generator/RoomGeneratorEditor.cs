using Safari.MapComponents;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Safari.Editor
{

    public class RoomGeneratorEditor : EditorWindow
    {
        public TileBase wall;
        public TileBase background;

        public Grid grid;
        public Room room;
        public RoomPreset roomPreset;

        public Vector2Int chunkSize;
        public string tempName = "Room_";




        // Add menu item named "My Window" to the Window menu
        [MenuItem("Window/Room Generator")]
        public static RoomGeneratorEditor ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            var window = GetWindow(typeof(RoomGeneratorEditor), false, "Room Generator");
            window.name = "Room Generator";
            return window as RoomGeneratorEditor;
        }


        void OnGUI()
        {
            EditorGUIUtility.wideMode = true;
            EditorGUILayout.LabelField("Room Generator");
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(this), typeof(RoomGeneratorEditor), false);
            GUI.enabled = true;
            EditorGUILayout.LabelField("Environments");
            EditorGUI.indentLevel++;
            grid = EditorGUILayout.ObjectField("Grid", grid, typeof(Grid), true) as Grid;
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Tiles");
            EditorGUI.indentLevel++;
            wall = EditorGUILayout.ObjectField("Wall Tile", wall, typeof(TileBase), true) as TileBase;
            background = EditorGUILayout.ObjectField("Background Tile", background, typeof(TileBase), true) as TileBase;
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Data");
            EditorGUI.indentLevel++;
            if (roomPreset) roomPreset.roomName = EditorGUILayout.TextField("Name", roomPreset.roomName);
            else
            {
                tempName = EditorGUILayout.TextField("Name", tempName);
            }
            roomPreset = EditorGUILayout.ObjectField("Room Preset", roomPreset, typeof(RoomPreset), true) as RoomPreset;
            room = EditorGUILayout.ObjectField("Room", room, typeof(Room), true) as Room;
            chunkSize = EditorGUILayout.Vector2IntField("Size", chunkSize);
            EditorGUI.indentLevel--;

            // grid not found, try to use map in the scene
            if (!grid)
            {
                var map = GameObject.Find("Map");
                if (map) grid = map.GetComponent<Grid>();
            }




            if (GUILayout.Button("Generate"))
            {
                Generate();
            }
            if (GUILayout.Button("Clear Settings"))
            {
                room = null;
                roomPreset = null;
            }
            if (GUILayout.Button("Clear All Tiles"))
            {
                ClearAllTiles();
            }
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            if (!room)
            {
                var newRoomObj = new GameObject("New Room", typeof(Room));
                room = newRoomObj.GetComponent<Room>();
                room.transform.SetParent(grid.transform);
            }


            //create all layer of room
            room.CreateAllLayer();
            var bound = new BoundsInt(0, 0, 0, chunkSize.x * 16, chunkSize.y * 16, 1);

            //Room.Geometry.SetTilesBlock(bound, tileArray);

            var tiles = new TileBase[chunkSize.x * chunkSize.y * 256];
            Array.Fill(tiles, wall, 0, chunkSize.x * 16);
            for (int y = 1; y < chunkSize.y * 16; y++)
            {
                tiles[chunkSize.x * 16 * y] = wall;
                tiles[chunkSize.x * 16 * (y + 1) - 1] = wall;
            }
            Array.Fill(tiles, wall, chunkSize.x * 16 * (chunkSize.y * 16 - 1), chunkSize.x * 16);
            room.Geometry.SetTilesBlock(bound, tiles);


            Array.Fill(tiles, background);
            //Vector3Int[] positionArray = (chunkSize * 16).Enumerate().Select(n => (Vector3Int)n).ToArray();
            Debug.Log(tiles.Length);
            room.Floor.SetTilesBlock(bound, tiles);

            Save();
        }

        private void Save()
        {
            if (roomPreset != null)
            {
                string path = AssetDatabase.GetAssetPath(roomPreset);
                path = path.Replace(roomPreset.name + ".asset", roomPreset.name + ".prefab");
                var prefab = PrefabUtility.SaveAsPrefabAsset(room.gameObject, path);
                roomPreset.SetPrefab(prefab);
                roomPreset.SetShapeSize(chunkSize);
                DestroyImmediate(room.gameObject);
                room = prefab.GetComponent<Room>();
                EditorUtility.DisplayDialog("Room Prefab Created", "Room prefab created in the same folder of the preset file", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Room GameObject Created", "Room GameObject created under the grid", "OK");
            }
        }

        [ContextMenu("Clear All Tiles")]
        public void ClearAllTiles()
        {
            room.Geometry.ClearAllTiles();
            room.Floor.ClearAllTiles();
        }

        public static string GetCurrentAssetDirectory()
        {
            foreach (var obj in Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path))
                    continue;

                if (System.IO.Directory.Exists(path))
                    return path;
                else if (System.IO.File.Exists(path))
                    return System.IO.Path.GetDirectoryName(path);
            }

            return "Assets";
        }

        //static void ConvertToLibraryTile()
        //{
        //    var @new = CreateInstance<LibraryTile>();
        //    @new.name = old.name + " new";
        //    Debug.Log(AssetDatabase.GetAssetPath(old));
        //    AssetDatabase.CreateAsset(@new, AssetDatabase.GetAssetPath(old)); //.Replace(old.name + ".asset", old.name + " new.asset"));
        //    EditorUtility.SetDirty(@new);
        //    @new.sprite = old.sprite;
        //    @new.gameObject = old.gameObject;
        //    @new.colliderType = old.colliderType;
        //    @new.color = old.color;
        //    @new.flags = old.flags;
        //    @new.transform = old.transform;
        //    AssetDatabase.SaveAssets();
        //    //   @new.name = old.name;
        //    //   AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(old));
        //}
    }

}