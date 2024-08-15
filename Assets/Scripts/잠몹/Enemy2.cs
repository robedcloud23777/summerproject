using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    private GameObject Player;
    private Player player1;
    public float stopRange = 1f;
    public float moveSpeed = 2f; // 이동 속도
    public float changeDirectionTime = 5f; // 방향 변경 주기
    public float wanderRange = 10f; // 랜덤 이동 범위
    public float rotationSpeed = 360f; // 회전 속도 (도 단위)
    public float detectionRadius = 10f; // 플레이어 탐지 반경
    public float raycastDistance = 20f; // Raycast 거리 (충돌 감지 거리)
    public LayerMask obstacleLayer; // 장애물 레이어
    public int hp = 10;
    public int drop = 0;
    public GameObject tnfbxks;

    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isExploding = false;

    private Rigidbody2D rb;

    private void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 참조
        SetNewRandomPosition(); // 초기 랜덤 위치 설정
        timer = changeDirectionTime;
    }

    private void Update()
    {
        if (isExploding) return; // 폭발 대기 상태라면 Update 중지

        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);
        followingPlayer = distanceToPlayer <= detectionRadius;

        if (hp <= 0)
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
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SetNewRandomPosition();
            timer = changeDirectionTime;
        }

        if (followingPlayer)
        {
            if (!IsPlayerObstructed())
            {
                MoveTowardsPlayer();
            }
            else
            {
                MoveTowardsTarget(); // 장애물이 있을 경우에는 타겟으로 이동
            }
        }
        else
        {
            MoveTowardsTarget();
        }

        if (distanceToPlayer <= stopRange)
        {
            StartCoroutine(ExplodeAfterDelay(1f)); // 1초 후 폭발
        }
    }

    private void RotateTowards(Vector2 direction)
    {
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void SetNewRandomPosition()
    {
        float randomX = Random.Range(-wanderRange, wanderRange);
        float randomY = Random.Range(-wanderRange, wanderRange);
        targetPosition = new Vector2(transform.position.x + randomX, transform.position.y + randomY);
    }

    private void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        if (!followingPlayer && Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewRandomPosition();
        }

        RotateTowards(direction);
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        RotateTowards(direction);
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        isExploding = true; // 폭발 대기 상태 활성화 
        yield return new WaitForSeconds(delay); // 1초 대기
        Explode();
    }

    private void Explode()
    {
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

        // 디버그: 레이캐스트 시각화
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
