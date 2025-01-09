using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class NonemapGenerator : EditorWindow
{
    private Tilemap tilemap;
    private Tilemap nonemap;
    private Tilemap gressmap;
    private GameObject tilePrefab;
    public Tilemap blockMap;
    public TileBase gressTile;
    public TileBase nightgressTile;

    public Tilemap nightgressmap;
    public Vector3 gressTileOffset = Vector3.zero;

    [MenuItem("Tools/Generate Nonemap")]
    public static void ShowWindow()
    {
        var window = GetWindow<NonemapGenerator>("Nonemap Generator");
        
        // 自动查找场景中的Tilemap
        var maps = FindObjectsOfType<Tilemap>();
        window.tilemap = maps.FirstOrDefault(m => m.name.Contains("Tilemap"));
        window.nonemap = maps.FirstOrDefault(m => m.name.Contains("nonemap"));
        window.blockMap = maps.FirstOrDefault(m => m.name.Contains("blockmap"));
        window.gressmap = maps.FirstOrDefault(m => m.name.Contains("gressmap"));
        window.nightgressmap = maps.FirstOrDefault(m => m.name.Contains("nightgressmap"));
        
        // Load grass5 tile from resources
        window.gressTile = Resources.Load<TileBase>("res/gress/grass5");
        window.nightgressTile = Resources.Load<TileBase>("res/gress/grass5-night");
        
        // Load nonetile from resources
        window.tilePrefab = Resources.Load<GameObject>("res/block");

        if (window.tilemap == null || window.nonemap == null || window.blockMap == null)
        {
            Debug.LogWarning("未能自动找到所有需要的Tilemap，请手动指定");
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Nonemap Generation Settings", EditorStyles.boldLabel);

        tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);
        nonemap = (Tilemap)EditorGUILayout.ObjectField("nonemap", nonemap, typeof(Tilemap), true);
        blockMap = (Tilemap)EditorGUILayout.ObjectField("blockMap", blockMap, typeof(Tilemap), true);
        gressmap = (Tilemap)EditorGUILayout.ObjectField("gressmap", gressmap, typeof(Tilemap), true);
        nightgressmap = (Tilemap)EditorGUILayout.ObjectField("nightgressmap", nightgressmap, typeof(Tilemap), true);
        tilePrefab = (GameObject)EditorGUILayout.ObjectField("None Tile", tilePrefab, typeof(GameObject), false);
        gressTile = (TileBase)EditorGUILayout.ObjectField("Gress Tile", gressTile, typeof(TileBase), false);
        nightgressTile = (TileBase)EditorGUILayout.ObjectField("nightgress Tile", nightgressTile, typeof(TileBase), false);
        gressTileOffset = EditorGUILayout.Vector3Field("Gress Tile Offset", gressTileOffset);

        if (GUILayout.Button("Generate Nonemap"))
        {
            GenerateNonemap();
        }
        if (GUILayout.Button("clearmap"))
        {
             nonemap.ClearAllTiles();
             gressmap.ClearAllTiles();
             nightgressmap.ClearAllTiles();
        }

        if (GUILayout.Button("Rotate GridNodes 180°"))
        {
            RotateGridNodes();
            if (nonemap != null) nonemap.gameObject.SetActive(true);
            if (tilemap != null) tilemap.gameObject.SetActive(false);
        }

        if (GUILayout.Button("Reset GridNodes Rotation"))
        {
            ResetGridNodes();
            if (nonemap != null) nonemap.gameObject.SetActive(false);
            if (tilemap != null) tilemap.gameObject.SetActive(true);
        }

        if (GUILayout.Button("Check Above and Draw Gress"))
        {
            CheckAboveAndDrawGress();
        }

        if (GUILayout.Button("Check Above Nonemap and Draw Gress"))
        {
            CheckAboveNonemapAndDrawGress();
        }
    }

    private void CheckAboveNonemapAndDrawGress()
    {
        if (nonemap == null || gressmap == null || gressTile == null)
        {
            Debug.LogError("Please assign all required references");
            return;
        }
        
        BoundsInt bounds = nonemap.cellBounds;
        for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
        {
            for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                Vector3Int abovePos = new Vector3Int(x, y - 1, 0);

                if (nonemap.HasTile(currentPos) && !nonemap.HasTile(abovePos))
                {
                    nightgressmap.SetTile(abovePos, nightgressTile);
                    nightgressmap.SetTransformMatrix(abovePos, Matrix4x4.TRS(new Vector3(0,0.472f,0), Quaternion.Euler(0, 0, 180), Vector3.one));
                }
            }
        }
    }

    private void RotateGridNodes()
    {
        var gridNodes = FindObjectsOfType<Transform>()
            .Where(t => t.name.Contains("Grid"))
            .ToList();

        foreach (var node in gridNodes)
        {
            Undo.RecordObject(node, "Rotate GridNode");
            node.Rotate(0, 0, 180);
            EditorUtility.SetDirty(node);
        }

        Debug.Log($"Rotated {gridNodes.Count} GridNodes");
    }

    private void ResetGridNodes()
    {
        var gridNodes = FindObjectsOfType<Transform>()
            .Where(t => t.name.Contains("Grid"))
            .ToList();

        foreach (var node in gridNodes)
        {
            Undo.RecordObject(node, "Reset GridNode Rotation");
            node.rotation = Quaternion.identity;
            EditorUtility.SetDirty(node);
        }

        Debug.Log($"Reset {gridNodes.Count} GridNodes");
    }

    private void CheckAboveAndDrawGress()
    {
        if (tilemap == null || gressmap == null || gressTile == null)
        {
            Debug.LogError("Please assign all required references");
            return;
        }
        gressmap.ClearAllTiles();
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
        {
            for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                Vector3Int abovePos = new Vector3Int(x, y + 1, 0);

                if (tilemap.HasTile(currentPos) && !tilemap.HasTile(abovePos))
                {
                    gressmap.SetTile(abovePos, gressTile);
                    gressmap.SetTransformMatrix(abovePos, Matrix4x4.TRS(new Vector3(0,-0.5f,0), Quaternion.identity, Vector3.one));
                }
            }
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
        for (int x = blockMapBounds.x; x < blockMapBounds.x + blockMapBounds.size.x - 1; x++)
        {
            for (int y = blockMapBounds.y + 1; y < blockMapBounds.y + blockMapBounds.size.y - 1; y++)
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
