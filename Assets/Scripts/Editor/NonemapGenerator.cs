using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class NonemapGenerator : EditorWindow
{
    private Tilemap tilemap;
    private Tilemap nightMap;
    private Tilemap gressmap;
    private GameObject tilePrefab;
    public Tilemap blockMap;
    public TileBase gressTile;
    public TileBase nightgressTile;

    public Tilemap nightgressmap;
    public Vector3 gressTileOffset = Vector3.zero;

    [MenuItem("Tools/Generate nightMap")]
    public static void ShowWindow()
    {
        var window = GetWindow<NonemapGenerator>("nightMap Generator");
        
        // 自动查找场景中的Tilemap
        var maps = FindObjectsOfType<Tilemap>();
        window.tilemap = maps.FirstOrDefault(m => m.name.Contains("Tilemap"));
        window.nightMap = maps.FirstOrDefault(m => m.name.Contains("nightMap"));
        window.blockMap = maps.FirstOrDefault(m => m.name.Contains("blockMap"));
        window.gressmap = maps.FirstOrDefault(m => m.name.Contains("gressMap"));
        window.nightgressmap = maps.FirstOrDefault(m => m.name.Contains("nightGressMap"));
        
        // Load grass5 tile from resources
        window.gressTile = Resources.Load<TileBase>("res/gress/grass5");
        window.nightgressTile = Resources.Load<TileBase>("res/gress/grass5-night");
        
        // Load nonetile from resources
        window.tilePrefab = Resources.Load<GameObject>("res/block");

        if (window.tilemap == null || window.nightMap == null || window.blockMap == null)
        {
            Debug.LogWarning("未能自动找到所有需要的Tilemap，请手动指定");
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("nightMap Generation Settings", EditorStyles.boldLabel);

        tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);
        nightMap = (Tilemap)EditorGUILayout.ObjectField("夜间 地图", nightMap, typeof(Tilemap), true);
        blockMap = (Tilemap)EditorGUILayout.ObjectField("外围地图", blockMap, typeof(Tilemap), true);
        gressmap = (Tilemap)EditorGUILayout.ObjectField("草地", gressmap, typeof(Tilemap), true);
        nightgressmap = (Tilemap)EditorGUILayout.ObjectField("夜间草地", nightgressmap, typeof(Tilemap), true);
        tilePrefab = (GameObject)EditorGUILayout.ObjectField("夜间格子 Tile", tilePrefab, typeof(GameObject), false);
        gressTile = (TileBase)EditorGUILayout.ObjectField("草地 Tile", gressTile, typeof(TileBase), false);
        nightgressTile = (TileBase)EditorGUILayout.ObjectField("夜间草地 Tile", nightgressTile, typeof(TileBase), false);
        gressTileOffset = EditorGUILayout.Vector3Field("Gress Tile Offset", gressTileOffset);

        if(GUILayout.Button("生成地图")){
            GenerateNightMap();
            CheckAboveAndDrawGress();
            CheckAboveNonemapAndDrawGress();
        }

        if (GUILayout.Button("Generate Nonemap"))
        {
            GenerateNightMap();
        }
        if (GUILayout.Button("clearmap"))
        {
             nightMap.ClearAllTiles();
             gressmap.ClearAllTiles();
             nightgressmap.ClearAllTiles();
        }

        if (GUILayout.Button("Rotate GridNodes 180°"))
        {
            RotateGridNodes();
            if (nightMap != null) nightMap.gameObject.SetActive(true);
            if (tilemap != null) tilemap.gameObject.SetActive(false);
        }

        if (GUILayout.Button("Reset GridNodes Rotation"))
        {
            ResetGridNodes();
            if (nightMap != null) nightMap.gameObject.SetActive(false);
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
        if (nightMap == null || gressmap == null || gressTile == null)
        {
            Debug.LogError("Please assign all required references");
            return;
        }
        
        BoundsInt bounds = nightMap.cellBounds;
        for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
        {
            for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                Vector3Int abovePos = new Vector3Int(x, y - 1, 0);

                if (nightMap.HasTile(currentPos) && !nightMap.HasTile(abovePos))
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

    private void GenerateNightMap()
    {
        if (tilemap == null || nightMap == null || tilePrefab == null)
        {
            Debug.LogError("Please assign all required references");
            return;
        }
        nightMap.ClearAllTiles();
        var newTile = ScriptableObject.CreateInstance<CustomTile>();
        ((CustomTile)newTile).gameObjectToPlace = tilePrefab;
        
        BoundsInt blockMapBounds = blockMap.cellBounds;
        Debug.Log($"BlockMap Bounds: {blockMapBounds}");
        for (int x = blockMapBounds.x + 1; x < blockMapBounds.x + blockMapBounds.size.x - 1; x++)
        {
            for (int y = blockMapBounds.y + 1; y < blockMapBounds.y + blockMapBounds.size.y - 1; y++)
            {
                if (!tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    nightMap.SetTile(position, newTile);
                }
            }
        }
    }
}
