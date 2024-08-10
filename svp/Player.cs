using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float downlaserRange = 2.5f;
    public float debugDistance = 0.3f;
    public float laserRange = 10f;

    private bool isGrounded;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private LayerMask platformLayerMask;

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        platformLayerMask = LayerMask.GetMask("Platform");
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 이동
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 방향 전환
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
            animator.SetBool("Run",true);
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
            animator.SetBool("Run",true);
        } if(moveInput==0){
            animator.SetBool("Run",false);
        }

        // 점프 (스페이스바 사용)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }
        if(Input.GetMouseButton(0)){
             ShootLaser();
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Ground",true);
            animator.SetTrigger("Ground2");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("Ground",false);
        }
    }
    void ShootLaser(){
        // setting laset's direction and start position.
        Vector2 direction =  spriteRenderer.flipX?Vector2.left:Vector2.right;
        Vector2 startPos = transform.position;


        //레이저 마스크 설정
        int layerMask = LayerMask.GetMask("Target");

        //레이저 발사
        RaycastHit2D hit = Physics2D.Raycast(startPos,direction,laserRange,layerMask);
        Debug.DrawRay(startPos,direction*laserRange,Color.red);

        if(hit.collider!=null){
            Debug.Log("적중 : "+hit.collider.gameObject.name);
            Destroy(hit.collider.gameObject,0.5f);
        }
        if(!isGrounded){
            RaycastHit2D downHit = Physics2D.Raycast(startPos,Vector2.down,downlaserRange,platformLayerMask);
            Debug.DrawRay(startPos,Vector2.down*downlaserRange,Color.yellow);

            if(downHit.collider!=null&&downHit.distance>=debugDistance){
                Debug.Log("아래쪽 레이가 0.3 이상 들어감");
            }
        }
    }
}