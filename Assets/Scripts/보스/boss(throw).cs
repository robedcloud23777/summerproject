using Unity.VisualScripting;
using UnityEngine;

public class bossthrow : MonoBehaviour
{
    public GameObject bombOb;

    private GameObject Player;
    private Player player1;
    public float moveSpeed = 2f; // 이동 속도
    public float changeDirectionTime = 5f; // 방향 변경 주기
    public float wanderRange = 10f; // 랜덤 이동 범위
    public float rotationSpeed = 360f; // 회전 속도 (도 단위)
    public float detectionRadius = 5f; // 플레이어 탐지 반경
    public float shootDistance = 3f; // 총알 발사 거리
    public float fireRate = 1f; // 연사 속도 (초 단위)
    public Transform throwPoint; // 총알 발사 위치
    public float raycastDistance = 20f; // Raycast 거리 (충돌 감지 거리)
    public LayerMask obstacleLayer; // 장애물 레이어
    public int hp = 100;
    public surutan surutan;

    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isthrowing;
    private float lastthrowTime;
    private Rigidbody2D rb;

    private void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        timer = changeDirectionTime;
        lastthrowTime = -fireRate; // 처음 발사 시간을 초기화하여 첫 발사가 가능하도록 설정
        SetNewRandomPosition();
    }

    private void Update()
    {

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
        if (Player == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
            return;
        }

        // 플레이어와의 거리 체크
        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);
        followingPlayer = distanceToPlayer <= detectionRadius;
        isthrowing = distanceToPlayer <= shootDistance;

        if (isthrowing)
        {
            // 총알 발사
            TrythrowAtPlayer();
            // 멈춤
            moveSpeed = 0f;
        }
        else if (followingPlayer && !IsPlayerObstructed())
        {
            // 플레이어를 향해 이동
            targetPosition = Player.transform.position;
            // 이동 속도 재설정
            moveSpeed = 2f;
        }
        else
        {
            // 랜덤 이동
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SetNewRandomPosition();
                timer = changeDirectionTime;
            }
            moveSpeed = 2f;
        }

        MoveTowardsTarget();
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
        rb.AddForce(direction * moveSpeed);

        
        RotateTowards(direction);
    }

    private void RotateTowards(Vector2 direction)
    {
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void TrythrowAtPlayer()
    {
        // 총알 발사 시간을 체크하여 일정 시간 간격으로 총알 발사
        if (Time.time - lastthrowTime >= fireRate)
        {
            throwAtPlayer();
            lastthrowTime = Time.time;
        }
    }

    public void throwAtPlayer()
    {

        GameObject newBomb = Instantiate(bombOb, transform.position, throwPoint.transform.rotation * Quaternion.Euler(0, 0, 90));
        newBomb.GetComponent<Rigidbody2D>().AddForce(newBomb.transform.up * -5, ForceMode2D.Impulse);
    }

    private bool IsPlayerObstructed()
    {
        Vector2 directionToPlayer = (Player.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, raycastDistance, obstacleLayer);

        // 디버그: 레이캐스트 시각화
        Debug.DrawRay(transform.position, directionToPlayer * raycastDistance, Color.red);

        return hit.collider != null && hit.collider.gameObject != Player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Map"))
        {
            ReverseDirection();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ReverseDirection();
        }
    }

    private void ReverseDirection()
    {
        // 현재 이동 방향의 반대 방향으로 새로운 목표 위치를 설정
        Vector2 currentDirection = (targetPosition - (Vector2)transform.position).normalized;
        targetPosition = (Vector2)transform.position - currentDirection * wanderRange;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트가 적인지 확인
        if (collision.CompareTag("bullet"))
        {
            hp -= 1;
        }
    }


}