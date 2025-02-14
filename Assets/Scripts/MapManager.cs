using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UIElements;
struct HideCollider
{
    public GameObject blockObj;
    public Vector3Int tilePos;
    public Vector3 position;
}
public enum MapState
{
    Normal,
    Night
}

public class MapManager : Singleton<MapManager>
{
    public event Action<MapState> OnStateChanged;
    public MapState CurrentState { get; private set; } = MapState.Normal;
    public Tilemap tilemap;
    public Tilemap nightMap;
    public Tilemap thingMap;
    public Tilemap tmpMap;
    public Tilemap blockMap;
    public Tilemap gressmap;
    public Tilemap nightgressmap;
    public TileBase gressTile;
    public TileBase nightgressTile;
    public PlayerMovement Player;
    public GameObject PlayerNode;
    public List<Rigidbody2D> boxList = new List<Rigidbody2D>();
    public GameObject tilePrefab;
    public GameObject RotateNode;
    List<HideCollider> hideColliderList ;

    // Start is called before the first frame update
    public TileBase newTile; // 要生成的新格子的瓦片
    [SerializeField] private float rotationDuration = 1.0f; // 旋转持续时间（秒）
    [SerializeField] private float angle = 180.0f; // 旋转角度（度）

    public GameObject GridNode;
    [SerializeField]private Vector3Int mapSize;
    

    void Awake()
    {
        // tilemap.cellBounds = new BoundsInt(new Vector3Int(0, 0, 0), new Vector3Int(10, 10, 0)); 
    }

    

    void Start()
    {
        //onChangeState();
        initNoneMap();
        OnNewScene();
    }
    
    void OnNewScene(){
        hideColliderList = new List<HideCollider>();

        blockMap = GameObject.Find("blockMap").GetComponent<Tilemap>();
        nightMap = GameObject.Find("nightMap").GetComponent<Tilemap>();
        gressmap = GameObject.Find("gressMap").GetComponent<Tilemap>();
        nightgressmap = GameObject.Find("nightGressMap").GetComponent<Tilemap>();
        gressTile = Resources.Load<TileBase>("res/gress/grass5");
        nightgressTile = Resources.Load<TileBase>("res/gress/grass5-night");
        nightMap.gameObject.SetActive(false);

        // if(GameObject.Find("box")){
        //     box = GameObject.Find("box").GetComponent<Rigidbody2D>();
        // }
                // 查找所有带有 "box" 标签的游戏对象
        GameObject[] boxObjects = GameObject.FindGameObjectsWithTag("box");

        // 遍历这些游戏对象并获取它们的 Rigidbody2D 组件
        foreach (GameObject box in boxObjects)
        {
            Rigidbody2D rb = box.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                boxList.Add(rb);
            }
        }

        // 或者使用 LINQ 一次性完成
        boxList = GameObject.FindGameObjectsWithTag("box")
                            .Select(box => box.GetComponent<Rigidbody2D>())
                            .Where(rb => rb != null)
                            .ToList();
                            
        if(GameObject.Find("Player")){
            Player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        }
        // initNoneMap();
        
    }
    
    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing){
            return;
        }
        if(!RotateNode){
            RotateNode = GameObject.Find("GridNode");
        }
            // 将目标节点的位置设置为摄像机的位置
        if (Input.GetMouseButtonDown(0) && CheckCanChangeState())
        {
            GetPlayerTranfotm();
            // Camera.main.GetComponent<Cemara>().enabled  = false;
            nightMap.gameObject.SetActive(!nightMap.isActiveAndEnabled);
            tilemap.gameObject.SetActive(!tilemap.isActiveAndEnabled);
            //box.GetComponent<box>().onChangeState();
            // 计算旋转
            // Quaternion rotation = Quaternion.Euler(0, 180, 0);

            // 将摄像头的中心点作为旋转的轴点
            Vector3 axisPoint = Camera.main.transform.position;

            // 创建一个围绕轴点旋转的旋转
            Quaternion aroundCameraRotation = Quaternion.AngleAxis(180, Vector3.up);
            if(Player){
                Player.onChangeState();
            }

            // Rotate180Degrees();
            onChangeState();
            // GridNode.transform.RotateAround(axisPoint, new Vector3(0, 0, 1), 180);
            // PlayerNode.transform.RotateAround(axisPoint, new Vector3(0, 0, 1), 180);

                // 获取当前屏幕的中心点的视口坐标
            Vector3 screenCenter = new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane);
                // 记录旋转前的位置
            Vector3 originalGridNodePosition = GridNode.transform.position;
            Vector3 originalPlayerNodePosition = PlayerNode.transform.position;
            // 将视口坐标转换为世界坐标
            Vector3 centerPoint = Camera.main.ViewportToWorldPoint(screenCenter);

            // 确定旋转的角度
            float rotationAngle = 180f; // 或者其他角度
            //Debug.Log("centerPoint : " + centerPoint);
            // 进行旋转
            GridNode.transform.RotateAround(centerPoint, Vector3.forward, rotationAngle);
            GridNode.transform.position = Vector3.zero;

            CheckAboveNonemapAndDrawGress();
        }

    }

    bool CheckCanChangeState(){
        if(Player){
            if (!Player.CheckCanChangeState() || !Player.IsGrounded())
            {
                Debug.Log("Can't change state 1" + Player.CheckCanChangeState());
                Debug.Log("Can't change state 2" +Player.IsGrounded());
                return false;
            }
        }
        return true;
    
    }

    void Rotate180Degrees()
    {
        var gridNode = GridNode.transform;
        var playerNode = PlayerNode.transform;
        // 获取屏幕中心点的世界坐标
        Vector3 screenCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
        Debug.Log("Screen Center: " + screenCenter);

        // 计算 GridNode 相对于屏幕中心点的局部坐标
        Vector3 offset = gridNode.position - screenCenter;
        Debug.Log("Offset: " + offset);

        // 应用180度旋转
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        Vector3 rotatedOffset = rotation * offset;
        Debug.Log("Rotated Offset: " + rotatedOffset);

        // 将局部坐标转换回世界坐标
        Vector3 newPosition = screenCenter + rotatedOffset;
        Debug.Log("New Position: " + newPosition);

        // 更新 GridNode 的位置
        gridNode.position = newPosition;
        // 更新 GridNode 的旋转
        gridNode.rotation = rotation * gridNode.rotation;

        playerNode.position = newPosition;
        playerNode.rotation = rotation * playerNode.rotation;
        
    }

    IEnumerator RotateNodeAroundCenter(float degrees)
    {
        // 获取当前节点的位置
        Vector3 currentPosition = GridNode.transform.position;

        // 围绕世界中心点旋转
        Vector3 center = Vector3.zero; // 世界中心点

        // 计算旋转的总角度
        float totalAngle = degrees;

        // 每帧旋转的角度
        // float anglePerFrame = totalAngle / (rotationDuration / Time.deltaTime);
        float anglePerFrame = 180;
        // 当前已旋转的角度
        float currentAngle = 0.0f;

        while (currentAngle < totalAngle)
        {
            // 更新节点的位置和旋转
            GridNode.transform.RotateAround(center, Vector3.forward, anglePerFrame * Time.deltaTime);

            // 更新已旋转的角度
            currentAngle += anglePerFrame * Time.deltaTime;

            yield return null;
        }

        // 确保最终旋转到位
        GridNode.transform.RotateAround(center, Vector3.up, totalAngle - currentAngle);
        //GridNode.transform.rotation = Quaternion.identity;
        for (int i = 0; i < boxList.Count; i++)
        {
            var box = boxList[i];
            box.GetComponent<box>().onChangeState();
        }
    }


    void initNoneMap(){
        // nightMap.ClearAllTiles();
        newTile = ScriptableObject.CreateInstance<CustomTile>();
        ((CustomTile)newTile).gameObjectToPlace = tilePrefab; // 设置你的GameObject
        // mapSize = blockmap.
        // mapSize = blockMap.cellBounds.size;
        // BoundsInt blockMapBounds = blockMap.cellBounds;
        // for (int x = blockMapBounds.x; x < blockMapBounds.x + blockMapBounds.size.x; x++) {
        //     for (int y = blockMapBounds.y; y < blockMapBounds.y + blockMapBounds.size.y; y++) {

        //         if (!tilemap.HasTile(new Vector3Int(x, y, 0))){
        //             Vector3Int position = new Vector3Int(x, y, 0);
        //             nightMap.SetTile(position, newTile); // 将Tile设置到Tilemap的指定位置
        //         }
        //     }
        // }
        // nightMap.gameObject.SetActive(false);
    }

    

    public void onChangeState()
    {
        var newState = CurrentState == MapState.Normal ? MapState.Night : MapState.Normal;
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
        
        if (CurrentState == MapState.Night)
        {
            for (int i = 0; i < boxList.Count; i++)
            {
                var box = boxList[i];
                // 获取碰撞体的世界空间边界
                if (box) {
                    var collider2D = box.GetComponent<BoxCollider2D>();
                    Bounds bounds = box.GetComponent<BoxCollider2D>().bounds;
                    // 转换边界到瓦片地图坐标
                    Vector3Int min = nightMap.WorldToCell(bounds.min);
                    Vector3Int max = nightMap.WorldToCell(bounds.max);
                    // 遍历区域内的所有瓦片
                    for (int x = min.x -1; x <= max.x+1; x++)
                    {
                        for (int y = min.y-1; y <= max.y+1; y++)
                        {
                            Vector3Int position = new Vector3Int(x, y, 0);
                            TileBase tile = nightMap.GetTile(position);
                            //Debug.Log("position"+position);

                            if (tile != null)
                            {
                                var tileGameObject = nightMap.GetInstantiatedObject(position);
                                // 检查是否有其他碰撞体接触这个瓦片
                                // Collider2D[] colliders = Physics2D.OverlapPointAll(nonemap.GetCellCenterWorld(position));
                                // foreach (var otherCollider in colliders)
                                {
                                    // if (otherCollider == collider2D) // 排除自身
                                    {
                                        //tileGameObject = nonemap.GetInstantiatedObject(position);

                                        if (tileGameObject != null)
                                        {
                                            Bounds tileBounds = tileGameObject.GetComponent<Renderer>().bounds;
                                            HideCollider collider = new HideCollider();
                                            collider.blockObj = tileGameObject;
                                            collider.position = tileGameObject.transform.position;
                                            collider.tilePos = position;
                                            hideColliderList.Add(collider);

                                            if (IsCompletelyInside(collider2D.bounds, tileBounds, box.transform, tileGameObject.transform))
                                            {
                                                nightMap.SetTile(position, null);
                                                nightMap.RefreshTile(position);
                                                continue;
                                            }
                                            SetColliderSize(collider2D.bounds, tileBounds, tileGameObject.transform,position);
                                            nightMap.RefreshTile(position);

                                        }
                                    }
                                }
                            }
                        }
                    }
                    box.gameObject.SetActive(false);
                }
            }
            
        }
        else
        {
            foreach (var obj in hideColliderList)
            {
                if(obj.blockObj == null){
                    nightMap.SetTile(obj.tilePos, newTile);
                }
            }
            hideColliderList.Clear();
            for (int i = 0; i < boxList.Count; i++)
            {
                var box = boxList[i];
                if (box) {
                    box.gameObject.SetActive(true);
                }
            }
            tmpMap.ClearAllTiles();

        }
        nightMap.RefreshAllTiles();
        
        TilemapCollider2D tilemapCollider = nightMap.GetComponent<TilemapCollider2D>();
        if (tilemapCollider != null)
        {
            tilemapCollider.ProcessTilemapChanges();
        }
        
        CompositeCollider2D compositeCollider = nightMap.GetComponent<CompositeCollider2D>();
        if (compositeCollider != null)
        {
            compositeCollider.GenerateGeometry();
        }
    }

    bool IsCompletelyInside(Bounds outerBounds, Bounds innerBounds,Transform outTransform,Transform innerTransform)
    {

        Vector2 outerMin = outerBounds.min;
        Vector2 outerMax = outerBounds.max;
        Vector2 innerMin = innerBounds.min;
        Vector2 innerMax = innerBounds.max;

        // 检查内碰撞体是否完全在外部碰撞体内部
        return innerMin.x >= outerMin.x && innerMax.x <= outerMax.x &&
               innerMin.y >= outerMin.y && innerMax.y <= outerMax.y;
    }

    void SetColliderSize(Bounds boundsA, Bounds boundsB, Transform innerTransform,Vector3Int vector3Int)
    {
        // Debug.Log(boundsB);
        // 计算两个Bounds的相交区域

        Bounds intersectionBounds = CalculateIntersection(boundsA, boundsB);
        // 如果没有交集，则不需要调整
        if (Math.Round(intersectionBounds.size.y, 1) == 0 && Math.Round(intersectionBounds.size.x, 1) == 0)
        {
            return;
        }

        // 调整Transform的位置到相交区域的中心
        var oldposition = intersectionBounds.center;
        var boxScale = Vector3.one;
        // 调整Transform的缩放到相交区域的大小
        if(Math.Round(intersectionBounds.size.y, 1) > 0 && Math.Round(intersectionBounds.size.x, 1) > 0)
        {
            var localScale = new Vector3(innerTransform.localScale.x - intersectionBounds.size.x, innerTransform.localScale.y, innerTransform.localScale.z);
            if(localScale.x <= 0.05 || localScale.y <= 0.05)
            {
                nightMap.SetTile(vector3Int, null);
                nightMap.RefreshTile(vector3Int);
                return;
            }
            boxScale = localScale;
            // innerTransform.localScale = localScale;
            //if (boundsB.min.x < boundsA.min.x)
            //{
            //    innerTransform.position = new Vector3(innerTransform.position.x - intersectionBounds.size.x / 2, innerTransform.position.y, innerTransform.position.z);
            //}
            //else
            //{
            //    innerTransform.position = new Vector3(innerTransform.position.x + intersectionBounds.size.x / 2, innerTransform.position.y, innerTransform.position.z);
            //}
        }

        GameObject innerObject = GameObject.Instantiate(innerTransform.gameObject);

        nightMap.SetTile(vector3Int, null);
        nightMap.RefreshTile(vector3Int);
        var collidernewTile = ScriptableObject.CreateInstance<TmpTile>();
        ((TmpTile)collidernewTile).gameObjectToPlace = innerObject; // 设置你的GameObject
        tmpMap.SetTile(vector3Int, collidernewTile); // 将Tile设置到Tilemap的指定位置
        tmpMap.RefreshTile(vector3Int);
        var tileGameObject = tmpMap.GetInstantiatedObject(vector3Int).transform;
        BoxCollider2D collider = tileGameObject.GetComponent<BoxCollider2D>();
        tileGameObject.localScale = boxScale;
        if (Math.Round(intersectionBounds.size.y, 1) > 0 && Math.Round(intersectionBounds.size.x, 1) > 0)
        {
            if (boundsB.min.x < boundsA.min.x)
            {
                tileGameObject.position = new Vector3(tileGameObject.position.x - intersectionBounds.size.x / 2, tileGameObject.position.y, tileGameObject.position.z);
            }
            else
            {
                tileGameObject.position = new Vector3(tileGameObject.position.x + intersectionBounds.size.x / 2, tileGameObject.position.y, tileGameObject.position.z);
            }

        }
        Destroy(innerObject);
    }
    private static Bounds CalculateIntersection(Bounds boundsA, Bounds boundsB)
    {
        Vector3 intersectionMin = Vector3.Max(boundsA.min, boundsB.min);
        Vector3 intersectionMax = Vector3.Min(boundsA.max, boundsB.max);

        if (intersectionMin.x > intersectionMax.x || intersectionMin.y > intersectionMax.y || intersectionMin.z > intersectionMax.z)
        {
            return new Bounds(Vector3.zero, Vector3.zero);
        }
        else
        {
            return new Bounds((intersectionMin + intersectionMax) * 0.5f, intersectionMax - intersectionMin);
        }
    }

    public void OpenPassView(){
        UIManager.Instance.OpenWindow("PassView");
    }

    /// <summary>
    /// 清空nightMap指定坐标的格子
    /// </summary>
    /// <param name="position">要清空的格子坐标</param>
    public void ClearNightMapTile(Vector3Int position)
    {
        nightMap.SetTile(position, null);
        nightMap.RefreshTile(position);
    }

    public Vector3Int GetPlayerTranfotm()
    {
        Vector3 pos = Player.transform.position;
        var TilePos = tilemap.WorldToCell(pos);
        Debug.Log( TilePos.ToString() );
        return TilePos;
    }

    private void CheckAboveNonemapAndDrawGress()
    {
        nightgressmap.ClearAllTiles();
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
}
