using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float moveSpeed = 7f;
    Rigidbody2D rb;
    public float hp = 10f;



    void Update()
    {
        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Vector2.left *moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
}