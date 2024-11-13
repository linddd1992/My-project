using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private AudioSource jumpSoundEffect;

    public MovementState playerState;
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
        }
        else
        {
            // 获取角色的脚底位置
            Vector3 pivotPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z); // 获取脚底位置

            // 翻转 SpriteRenderer 的 Y 轴
            playNode.transform.GetComponent<SpriteRenderer>().flipY = !playNode.transform.GetComponent<SpriteRenderer>().flipY;
            // playNode.transform.position = playNode.transform.position + new Vector3(0, 0.5f, 0);
            playNode.transform.localPosition = new Vector3(playNode.transform.localPosition.x, playNode.transform.localPosition.y + 1f, playNode.transform.localPosition.z);

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
                rb.velocity = new Vector2(rb.velocity.x,  jumpForce);
            }
        }


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
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
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

    bool CheckCanChangeState(){
        if (!IsGrounded() && playerState == MovementState.jumping){
            return false;
        }
        else{
            
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







