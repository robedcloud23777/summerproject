using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    private GameObject Player;
    private Player player1;
    public float stopRange = 1f;
    public float moveSpeed = 2f;
    public float changeDirectionTime = 5f;
    public float wanderRange = 10f;
    public float rotationSpeed = 360f;
    public float detectionRadius = 10f;
    public float raycastDistance = 20f;
    public LayerMask obstacleLayer;
    public int hp = 10;
    public int drop = 0;
    public GameObject tnfbxks;

    public float stopTime = 1f;
    public float stopChance = 0.5f;

    public GameObject explosionEffect; // 폭발 효과를 위한 프리팹

    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isExploding = false;
    private Animator animator;
    private float stopTimer;
    private bool isStopped;

    private Rigidbody2D rb;

    private void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        SetNewRandomPosition();
        timer = changeDirectionTime;
        stopTimer = stopTime;
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isExploding)
        {
            isStopped = true;
        }
        if (isStopped)
        {
            animator.SetBool("move", false);
            animator.SetBool("stop", true);
        }
        else
        {
            animator.SetBool("move", true);
            animator.SetBool("stop", false);
        }
        if (isExploding) return;

        if (hp <= 0)
        {
            HandleDeath();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);
        followingPlayer = distanceToPlayer <= detectionRadius;

        if (followingPlayer && !IsPlayerObstructed())
        {
            targetPosition = Player.transform.position;
            moveSpeed = 2f;
        }
        else
        {
            HandleRandomMovement();
        }

        if (distanceToPlayer <= stopRange)
        {
            StartCoroutine(ExplodeAfterDelay(1f));
        }

        MoveTowardsTarget();
        HandleDirection();
    }

    private void HandleDeath()
    {
        drop = Random.Range(1, 10);
        if (drop == 1)
        {
            if (tnfbxks != null)
            {
                GameObject obj = Instantiate(tnfbxks, transform.position, Quaternion.Euler(0, 0, 90));
                Rigidbody2D objRb = obj.GetComponent<Rigidbody2D>();
                if (objRb != null)
                {
                    objRb.AddForce(obj.transform.up * -2, ForceMode2D.Impulse);
                }
            }
        }
        Destroy(gameObject);
    }

    private void HandleRandomMovement()
    {
        if (isStopped)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0)
            {
                isStopped = false;
                moveSpeed = 2f;
                SetNewRandomPosition();
                timer = changeDirectionTime;
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (Random.value < stopChance)
                {
                    isStopped = true;
                    stopTimer = stopTime;
                    moveSpeed = 0f;
                }
                else
                {
                    SetNewRandomPosition();
                    timer = changeDirectionTime;
                }
            }
        }
    }

    private void SetNewRandomPosition()
    {
        float randomX = Random.Range(-wanderRange, wanderRange);
        float randomY = Random.Range(-wanderRange, wanderRange);
        targetPosition = new Vector2(transform.position.x + randomX, transform.position.y + randomY);
    }

    private void MoveTowardsTarget()
    {
        if (moveSpeed > 0f)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            if (!followingPlayer && Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetNewRandomPosition();
            }
        }
    }

    private void HandleDirection()
    {
        Vector2 target = followingPlayer ? Player.transform.position : targetPosition;

        // 이동 방향에 따른 시선 처리
        Vector2 velocity = rb.velocity; // Rigidbody2D의 속도를 이용해 이동 방향을 얻음

        if (velocity.x > 0) // 오른쪽으로 이동 중일 때
        {
            GetComponent<SpriteRenderer>().flipX = false; // 오른쪽을 바라봄
        }
        else if (velocity.x < 0) // 왼쪽으로 이동 중일 때
        {
            GetComponent<SpriteRenderer>().flipX = true; // 왼쪽을 바라봄
        }
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        isExploding = true;
        yield return new WaitForSeconds(delay);
        Explode();
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (player1 != null)
        {
            player1.hp -= 1;
        }
        Destroy(gameObject);
    }

    private bool IsPlayerObstructed()
    {
        Vector2 directionToPlayer = (Player.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, raycastDistance, obstacleLayer);

        Debug.DrawRay(transform.position, directionToPlayer * raycastDistance, Color.red);

        return hit.collider != null && hit.collider.gameObject != Player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullet"))
        {
            hp -= 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Map") || collision.gameObject.CompareTag("Enemy"))
        {
            ReverseDirection();
        }
    }

    private void ReverseDirection()
    {
        Vector2 currentDirection = (targetPosition - (Vector2)transform.position).normalized;
        targetPosition = (Vector2)transform.position - currentDirection * wanderRange;
    }
}