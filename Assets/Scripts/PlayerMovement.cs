using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    private bool isOnGround = false;
    [SerializeField] private LayerMask jumpableGround;

    [SerializeField] private LayerMask BoxGround;

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 7f;
    public GameObject playNode;

    public enum MovementState { idle, running, jumping, falling }
    public int mapState = 0;
    [SerializeField] private AudioSource jumpSoundEffect;

    public MovementState playerState;
    public BoxCollider2D canChangeColl ;

    public Tilemap tilemap;

    // Start is called before the first frame update    
    private void Start()
    {
        rb = playNode.GetComponent<Rigidbody2D>();
        coll = playNode.GetComponent<BoxCollider2D>();
        sprite = playNode.GetComponent<SpriteRenderer>();
        anim = playNode.GetComponent<Animator>();

    }

    public void onChangeState()
    {
        // CheckCanChangeState();
        CheckBelowTilemap();
        // Debug.Log("CheckCanChangeState",);
        rb.bodyType = RigidbodyType2D.Static;
        if (!playNode.transform.GetComponent<SpriteRenderer>().flipY)
        {
            
            // 获取角色的脚底位置
            Vector3 pivotPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z); // 获取脚底位置
            // 翻转 SpriteRenderer 的 Y 轴
            playNode.transform.GetComponent<SpriteRenderer>().flipY = !playNode.transform.GetComponent<SpriteRenderer>().flipY;
            playNode.transform.localPosition = new Vector3(playNode.transform.localPosition.x, playNode.transform.localPosition.y - 1f, playNode.transform.localPosition.z);
            // 绕脚底位置旋转 180 度
            // transform.RotateAround(pivotPoint, Vector3.forward, 180f);
            // Camera.main.transform.RotateAround(pivotPoint, Vector3.forward, 180f); // 旋转相机
            canChangeColl.transform.localRotation = Quaternion.Euler(0, 0, 180f);


        }
        else
        {
            // 获取角色的脚底位置
            Vector3 pivotPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z); // 获取脚底位置

            // 翻转 SpriteRenderer 的 Y 轴
            playNode.transform.GetComponent<SpriteRenderer>().flipY = !playNode.transform.GetComponent<SpriteRenderer>().flipY;
            // playNode.transform.position = playNode.transform.position + new Vector3(0, 0.5f, 0);
            playNode.transform.localPosition = new Vector3(playNode.transform.localPosition.x, playNode.transform.localPosition.y + 1f, playNode.transform.localPosition.z);
            // Camera.main.transform.RotateAround(pivotPoint, Vector3.forward, 0); // 旋转相机
            canChangeColl.transform.localRotation = Quaternion.Euler(0, 0, 0);


            // 绕脚底位置旋转 180 度
            // transform.RotateAround(pivotPoint, Vector3.forward, 0);
        }
        rb.bodyType = RigidbodyType2D.Dynamic;

    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        if(dirX != 0f){
            if(IsNeerBox(dirX)){
                Debug.Log("is near box");
            }
        }
        if (Input.GetButtonDown("Jump")){
            if(IsGrounded())
            {
                Debug.Log("jump");
                rb.velocity = new Vector2(rb.velocity.x,  jumpForce);
            }
        }

        CheckBelowTilemap();
        UpdateAnimationState();
    }

    //判断是否在箱子隔壁
    private bool IsNeerBox(float dirX){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.5f, BoxGround);
        if(dirX > 0f){
             hit =  Physics2D.Raycast(transform.position, Vector2.right, 0.5f, BoxGround);
        }else if(dirX < 0f){
             hit =  Physics2D.Raycast(transform.position, Vector2.left, 0.5f, BoxGround);
        }
        return hit.collider != null;
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            if (!playNode.transform.GetComponent<SpriteRenderer>().flipY){
                sprite.flipX = false;
            }else{
                sprite.flipX = true;
            }
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            if (!playNode.transform.GetComponent<SpriteRenderer>().flipY){
                sprite.flipX = true;
            }else{
                sprite.flipX = false;
            }
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }
        else
        {

        }
        playerState = state;
        anim.SetInteger("state", (int)state);

    }

    private bool IsGrounded()
    {
        // return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
            // 定义 Raycast 的起始点
        Vector2 origin = coll.bounds.center;
        // 定义 Raycast 的长度
        float length = 1f; // 可以根据需要调整此值

        // 使用 Raycast 向下发射一条射线，检查与地面层的碰撞
        RaycastHit2D hit1 = Physics2D.Raycast(origin + new Vector2(-0.4f, 0), Vector2.down, length, jumpableGround);
        RaycastHit2D hit2 = Physics2D.Raycast(origin + new Vector2(0.4f, 0), Vector2.down, length, jumpableGround);

        // 返回是否检测到地面
        return hit1.collider != null && hit2.collider != null;
    }

    public bool CheckCanChangeState(){
        // 获取 BoxCollider2D 的边界
        Bounds bounds = canChangeColl.bounds;

        // 调整检测参数
        Vector2 detectionSize = new Vector2(bounds.size.x * 0.8f, 0.3f); // 宽度缩小20%，高度增加
        float detectionDistance = 0.3f; // 增加检测距离

        // 检测下方碰撞
        RaycastHit2D hitBottom = Physics2D.BoxCast(
            bounds.center, 
            detectionSize, 
            0f, 
            Vector2.down, 
            detectionDistance, 
            jumpableGround);

        // 绘制完整的检测区域
        Vector2 boxCenter = bounds.center + Vector3.down * (detectionDistance/2);
        Debug.DrawLine(
            boxCenter + new Vector2(-detectionSize.x/2, -detectionSize.y/2),
            boxCenter + new Vector2(detectionSize.x/2, -detectionSize.y/2), 
            Color.red, 
            1f);
        Debug.DrawLine(
            boxCenter + new Vector2(-detectionSize.x/2, detectionSize.y/2),
            boxCenter + new Vector2(detectionSize.x/2, detectionSize.y/2), 
            Color.green, 
            1f);

        // 如果没有下方碰撞则返回true
        if (hitBottom.collider == null)
        {
            Debug.LogWarning($"Can change state - no collision below (检测区域: {detectionSize}, 距离: {detectionDistance})");
            return true;
        }
        
        Debug.LogWarning($"Cannot change state - collision with {hitBottom.collider.name} at {hitBottom.point}");
        return false;
    }

    private Vector3Int? lastHitCell = null; // 记录上一次碰撞的格子
    private void CheckBelowTilemap()
    {
        Vector2 origin = coll.bounds.center;
        origin.y = coll.bounds.min.y ; // 从底部发射射线

        // 定义射线的长度
        float length = 2f; // 可以根据需要调整此值

        // 发射射线
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, length, jumpableGround);
        Debug.DrawRay(origin, Vector2.down * length, Color.red);
        // 检测是否碰撞到 Tilemap
        if (hit.collider != null)
        {
            // 获取射线末端的格子坐标
            Vector3Int cellPosition = tilemap.WorldToCell(hit.point);

            // 判断末端是否是 Tilemap
            if (tilemap.HasTile(cellPosition))
            {
                Debug.Log("Raycast末端是 Tilemap，格子坐标: " + cellPosition);
                lastHitCell = cellPosition; // 记录当前碰撞的格子
            }
            else
            {
                Debug.Log("Raycast末端不是 Tilemap");
            }
        }
        else
        {
            Debug.Log("Raycast未碰撞到 Tilemap");
        }
    }
private void OnDrawGizmos()
    {
        if (lastHitCell.HasValue)
        {
            // 获取格子的世界坐标中心
            Vector3 cellCenter = tilemap.GetCellCenterWorld(lastHitCell.Value);
            Vector3 cellSize = tilemap.cellSize;

            // 绘制格子的线框
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(cellCenter, cellSize);
        }
    }
    // void OnCollisionEnter2D(Collision2D collision)
    // {

    //     if (collision.gameObject.CompareTag("box")) // 假设道具标记为 "Prop"
    //     {

    //         Rigidbody2D propRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
    //         if (propRigidbody!= null)
    //         {

    //             // 根据玩家的移动方向施加力
    //             if (rb.velocity.x > 0)
    //             {
    //                 propRigidbody.AddForce(new Vector2(rb.velocity.x * moveSpeed, 0));
    //             }
    //             else if (rb.velocity.x < 0)
    //             {
    //                 propRigidbody.AddForce(new Vector2(-rb.velocity.x * moveSpeed, 0));
    //             }
    //         }
    //     }
    // }
}
