using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    public float moveSpeed;
    public float jumpPower;
    public GameObject ground;
    bool iscontact = false;
    bool isJumping = false;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == ground) // ���߿� CompareTag �� ����
        {
            isJumping = false;
        }
        iscontact = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        iscontact = false;
    }

    void Move()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2 (xInput  * moveSpeed, rigid.velocity.y);
   
        /* or
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 moveVector = new Vector2 (hor, ver).normalized;
        transform.position += moveVector * moveSpeed * Time.deltaTime;  */
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rigid.AddForce(Vector2.up * jumpPower , ForceMode2D.Impulse);
            isJumping = true;
        }
    }

    void Item()
    //��â+�ݱ�
    {
        if (Input.GetKeyDown(KeyCode.E)) // GetKey�� �޸� GetKeyDown���� �ѹ��� �Է� ����
        {
            if (iscontact) // ��ü�� �����ߴ°�?
            {
                Debug.Log("�ȴ�");
            }
            else
            {
                Debug.Log("�ȴٰ�!!");
            }
        }

    }

    void OTshift()
    //����� ��ü
    {

    }

    void Damaged()
    //������ ����
    {

    }

    void Interaction()
    //������ ��ȣ�ۿ�
    {

    }

    void Start()
    {
        
    }


    void Update()
    {
        Move();
        Jump();
        Item();
    }
}
