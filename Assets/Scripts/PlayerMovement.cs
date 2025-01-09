using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static PlayerMovement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
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
    [SerializeField] private AudioSource jumpSoundEffect;

    public MovementState playerState;
    public BoxCollider2D canChangeColl ;

    public Tilemap tilemap;
    public Tilemap nonemap; // 添加nonemap引用

    // Start is called before the first frame update    
    private void Start()
    {
        rb = playNode.GetComponent<Rigidbody2D>();
        coll = playNode.GetComponent<CapsuleCollider2D>();
        sprite = playNode.GetComponent<SpriteRenderer>();
        anim = playNode.GetComponent<Animator>();
        tilemap = MapManager.Instance.tilemap;
        nonemap = MapManager.Instance.nonemap;
    }

    public void onChangeState()
    {
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

        UpdateAnimationState();
    }

    //判断是否在箱子隔壁
    private bool IsNeerBox(float dirX){
        Vector2 size = new Vector2(0.1f, coll.bounds.size.y * 0.8f); // 检测区域大小
        float distance = 0.5f; // 检测距离
        
        if(dirX > 0f){
            return Physics2D.BoxCast(
                transform.position, 
                size, 
                0f, 
                Vector2.right, 
                distance, 
                BoxGround);
        }else if(dirX < 0f){
            return Physics2D.BoxCast(
                transform.position, 
                size, 
                0f, 
                Vector2.left, 
                distance, 
                BoxGround);
        }
        return false;
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

    public bool IsGrounded()
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

private void OnDrawGizmos()
    {

        // 根据mapState选择使用哪个Tilemap
        Tilemap currentMap = MapManager.Instance.CurrentState == MapState.Night ? nonemap : tilemap;
        
        // 绘制玩家当前位置的格子
        if (currentMap != null)
        {
            // 将玩家位置转换为格子坐标
            Vector3Int playerCell = tilemap.WorldToCell(transform.position);
            
            // 获取玩家下方一格和两格的坐标
            Vector3Int belowCell1 = new Vector3Int(playerCell.x, playerCell.y - 1, playerCell.z);
            Vector3Int belowCell2 = new Vector3Int(playerCell.x, playerCell.y - 2, playerCell.z);
            
            if( MapManager.Instance.CurrentState == MapState.Night)
            {
                 belowCell1 = new Vector3Int(playerCell.x, playerCell.y + 1, playerCell.z);
                 belowCell2 = new Vector3Int(playerCell.x, playerCell.y + 2, playerCell.z);   
            }

            // 检查玩家下方两格是否都有Tile
            if (currentMap.HasTile(belowCell1) && currentMap.HasTile(belowCell2))
            {
                // 绘制玩家自己的格子
                Vector3 playerCellCenter = currentMap.GetCellCenterWorld(playerCell);
                Vector3 cellSize = currentMap.cellSize;
                
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(playerCellCenter, cellSize);
                // return true;
            }
            
        }
        // return true;
    }

    public bool CheckCanChangeState()
    {
        Tilemap currentMap = MapManager.Instance.CurrentState == MapState.Night ? nonemap : tilemap;
        
        // 绘制玩家当前位置的格子
        if (currentMap != null)
        {
            // 将玩家位置转换为格子坐标
            Vector3Int playerCell = tilemap.WorldToCell(transform.position);
            
            // 获取玩家下方一格和两格的坐标
            Vector3Int belowCell1 = new Vector3Int(playerCell.x, playerCell.y - 1, playerCell.z);
            Vector3Int belowCell2 = new Vector3Int(playerCell.x, playerCell.y - 2, playerCell.z);
            
            if(MapManager.Instance.CurrentState == MapState.Night)
            {
                 belowCell1 = new Vector3Int(playerCell.x, playerCell.y + 1, playerCell.z);
                 belowCell2 = new Vector3Int(playerCell.x, playerCell.y + 2, playerCell.z);   
            }

            // 检查玩家下方两格是否都有Tile
            if (currentMap.HasTile(belowCell1) && currentMap.HasTile(belowCell2))
            {
                // 绘制玩家自己的格子
                return true;
            }
            
        }
        return false;
    }


    // void OnCollisionEnter2D(Collision2D collision)
    // {
    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("box")) // 假设道具标记为 "Prop"
        {

            // Rigidbody2D propRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            // if (propRigidbody!= null)
            // {

            //     // 根据玩家的移动方向施加力
            //     if (rb.velocity.x > 0)
            //     {
            //         propRigidbody.AddForce(new Vector2(rb.velocity.x * moveSpeed, 0));
            //     }
            //     else if (rb.velocity.x < 0)
            //     {
            //         propRigidbody.AddForce(new Vector2(-rb.velocity.x * moveSpeed, 0));
            //     }
            // }
        }
    }
}
