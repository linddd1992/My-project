using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NonemapGenerator : EditorWindow
{
    private Tilemap tilemap;
    private Tilemap nonemap;
    private GameObject tilePrefab;
    public Tilemap blockMap;

    [MenuItem("Tools/Generate Nonemap")]
    public static void ShowWindow()
    {
        GetWindow<NonemapGenerator>("Nonemap Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Nonemap Generation Settings", EditorStyles.boldLabel);

        tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);
        nonemap = (Tilemap)EditorGUILayout.ObjectField("Nonemap", nonemap, typeof(Tilemap), true);
        blockMap = (Tilemap)EditorGUILayout.ObjectField("Nonemap", blockMap, typeof(Tilemap), true);
        tilePrefab = (GameObject)EditorGUILayout.ObjectField("None Tile", tilePrefab, typeof(GameObject), false);

        if (GUILayout.Button("Generate Nonemap"))
        {
            GenerateNonemap();
        }
        if (GUILayout.Button("clearmap"))
        {
             nonemap.ClearAllTiles();
        }
    }

    private void GenerateNonemap()
    {
        if (tilemap == null || nonemap == null || tilePrefab == null)
        {
            Debug.LogError("Please assign all required references");
            return;
        }

        nonemap.ClearAllTiles();
        BoundsInt bounds = tilemap.cellBounds;
        int count = 0;
        nonemap.ClearAllTiles();
        var newTile = ScriptableObject.CreateInstance<CustomTile>();
        ((CustomTile)newTile).gameObjectToPlace = tilePrefab; // 设置你的GameObject
        // mapSize = blockmap.
        var mapSize = blockMap.cellBounds.size;
        BoundsInt blockMapBounds = blockMap.cellBounds;
        for (int x = blockMapBounds.x; x < blockMapBounds.x + blockMapBounds.size.x; x++)
        {
            for (int y = blockMapBounds.y; y < blockMapBounds.y + blockMapBounds.size.y; y++)
            {

                if (!tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    nonemap.SetTile(position, newTile); // 将Tile设置到Tilemap的指定位置
                }
            }
        }

    }
}
