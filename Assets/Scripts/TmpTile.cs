using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.Tile;

[CreateAssetMenu(fileName = "New Custom Tile", menuName = "Tiles/Custom Tile")]
public class CustomTile : TileBase
{
    public GameObject gameObjectToPlace;

    public override void GetTileData(Vector3Int position, ITilemap tileMap, ref TileData tileData)
    {
        base.GetTileData(position, tileMap, ref tileData);
        // ���� TileData �� GameObject Ϊ���ǵ��Զ��� GameObject
        tileData.gameObject = gameObjectToPlace;
        tileData.color = Color.white;
        tileData.transform = Matrix4x4.identity;
        tileData.flags = TileFlags.None;
        tileData.gameObject.layer = LayerMask.NameToLayer("Tilemap");
        tileData.gameObject.name = position.ToString();
        tileData.colliderType = ColliderType.Grid;

    }

    // ��������� Tilemap �ϻ��� Tile ʱ������
    public override void RefreshTile(Vector3Int position, ITilemap tileMap)
    {
        
        tileMap.RefreshTile(position);
        
    }
}