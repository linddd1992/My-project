using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cemara : MonoBehaviour
{

    Transform player;
    Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameObject.Find("blockmap").GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;
        Debug.Log(bounds);
        Vector3Int size = bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        if(!player){

            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        MoveCamera();
    }

    //移动摄像机
    public void MoveCamera()
    {
        if (player && tilemap)
        {
            // 获取摄像机的正交大小(即视野的一半高度)
            float cameraHeight = Camera.main.orthographicSize;
            float cameraWidth = cameraHeight * Camera.main.aspect;

            // 计算摄像机可移动的边界
            Vector3 minBound = tilemap.LocalToWorld(tilemap.cellBounds.min);
            Vector3 maxBound = tilemap.LocalToWorld(tilemap.cellBounds.max);

            // 向内收缩32像素
            float shrinkAmount = 1f / tilemap.cellSize.x; // 假设x和y的cellSize相同
            minBound += Vector3.one * shrinkAmount;
            maxBound -= Vector3.one * shrinkAmount;

            // 限制摄像机的位置
            float newX = Mathf.Clamp(player.position.x, minBound.x + cameraWidth, maxBound.x - cameraWidth);
            float newY = Mathf.Clamp(player.position.y, minBound.y + cameraHeight, maxBound.y - cameraHeight);

            // 更新摄像机位置
            transform.position = new Vector3(newX, newY, transform.position.z);
        }
    }
}
