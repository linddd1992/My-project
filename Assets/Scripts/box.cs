using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;



public class box : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    private bool isMainState = true;

    [SerializeField]public LayerMask layerMask;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onChangeState()
    {
        // 获取碰撞体的世界空间边界
        Bounds bounds = boxCollider.bounds;
        // 转换边界到瓦片地图坐标

    }


    // Update is called once per frame
    void Update()
    {

    }
    bool IsCompletelyInside(BoxCollider2D outerCollider, BoxCollider2D innerCollider)
    {
        // 获取两个碰撞体的全局边界
        Bounds outerBounds = outerCollider.bounds;
        Bounds innerBounds = innerCollider.bounds;

        // 获取全局坐标下的最小和最大点
        Vector2 outerMin = outerCollider.transform.TransformPoint(outerBounds.min);
        Vector2 outerMax = outerCollider.transform.TransformPoint(outerBounds.max);
        Vector2 innerMin = innerCollider.transform.TransformPoint(innerBounds.min);
        Vector2 innerMax = innerCollider.transform.TransformPoint(innerBounds.max);

        // 检查内碰撞体是否完全在外部碰撞体内部
        return innerMin.x >= outerMin.x && innerMax.x <= outerMax.x &&
               innerMin.y >= outerMin.y && innerMax.y <= outerMax.y;
    }

    bool CheckCanMove(){
        //判断盒子上方是否有人或者盒子
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 0.5f, layerMask);
        if (hit.collider != null)
        {
            
        }
        return true;
    }

    public void SetColliderSize(BoxCollider2D colliderA, BoxCollider2D colliderB)
    {
        Bounds boundsA = colliderA.bounds;
        if (colliderB.bounds.Intersects(boundsA))
        {
            Bounds boundsB = colliderB.bounds;

            Vector3 overlapMin = new Vector3(
                Mathf.Max(boundsA.min.x, boundsB.min.x),
                Mathf.Max(boundsA.min.y, boundsB.min.y),
                0
            );

            Vector3 overlapMax = new Vector3(
                Mathf.Min(boundsA.max.x, boundsB.max.x),
                Mathf.Min(boundsA.max.y, boundsB.max.y),
                0
            );
            if (overlapMin.x < overlapMax.x && overlapMin.y < overlapMax.y)
            {
                Vector2 newSize = colliderB.size; 
                newSize.x -= (overlapMax.x - overlapMin.x);
                newSize.y -= (overlapMax.y - overlapMin.y); 

                newSize.x = Mathf.Max(newSize.x, 0.1f);
                newSize.y = 1;

                Vector3 originalPosition = colliderB.transform.position;

                Vector3 scaleChange = new Vector3(newSize.x / colliderB.size.x, newSize.y / colliderB.size.y, 1);
                colliderB.offset = new Vector2(colliderB.offset.x, 0);
                float offsetX = (1 - scaleChange.x) / 2;
                // 更新物体位置，确保左对齐
                colliderB.transform.localScale = new Vector3(scaleChange.x,1,1);

                var newPosx = originalPosition.x + offsetX;


                // 确定对齐方式
                if (boundsA.min.x < boundsB.min.x) // 判断左对齐
                {
                    // 左对齐时，保持左侧位置不变
                    colliderB.transform.position += new Vector3((colliderB.size.x - scaleChange.x) / 2, 0, 0);
                }
                else // 右对齐
                {
                    // 右对齐时，保持右侧位置不变
                    colliderB.transform.position -= new Vector3((colliderB.size.x - scaleChange.x) / 2, 0, 0);
                }


            }
        }
    }
}
