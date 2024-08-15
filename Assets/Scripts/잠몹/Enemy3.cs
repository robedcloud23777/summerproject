using UnityEngine;

public class Enemy3 : MonoBehaviour
{
    private GameObject Player;
    private Player player1;
    public float moveSpeed = 2f; // 이동 속도
    public float changeDirectionTime = 7f; // 방향 변경 주기
    public float wanderRange = 6f; // 랜덤 이동 범위
    public float rotationSpeed = 360f; // 회전 속도 (도 단위)
    public float detectionRadius = 10f; // 플레이어 탐지 반경
    public float shootDistance = 3f; // 총알 발사 거리
    public float slashRate = 2f; //  속도 (초 단위)
    public float raycastDistance = 20f; // Raycast 거리 (충돌 감지 거리)
    public LayerMask obstacleLayer; // 장애물 레이어
    public int hp = 10;
    public int drop = 0;
    
    public Transform slashPoint; // 총알 발사 위치
    public float attackrange = 1f;
    public float attackAngle = 110f;
    public GameObject tnfbxks;

    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isslashing;
    private float lastslashTime;
    
    private Rigidbody2D rb;

    private void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        timer = changeDirectionTime;
        lastslashTime = -slashRate; // 처음 발사 시간을 초기화하여 첫 발사가 가능하도록 설정
    }

    private void Update()
    {
        if (hp <= 0)
        {
            drop = Random.Range(1, 10);
            if (drop == 1)
            {
                Instantiate(tnfbxks, transform.position, slashPoint.transform.rotation * Quaternion.Euler(0, 0, 90));
                tnfbxks.GetComponent<Rigidbody2D>().AddForce(tnfbxks.transform.up * -2, ForceMode2D.Impulse);
            }
            Destroy(gameObject);
        }

        // 플레이어와의 거리 체크
        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);
        followingPlayer = distanceToPlayer <= detectionRadius;
        isslashing = distanceToPlayer <= shootDistance;

        if (isslashing)
        {
            // 검 휘두르기
            TryslashAtPlayer();
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

        if (!followingPlayer && Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewRandomPosition();
        }

        RotateTowards(direction);
    }

    private void RotateTowards(Vector2 direction)
    {
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void TryslashAtPlayer()
    {
        // 총알 발사 시간을 체크하여 일정 시간 간격으로 총알 발사
        if (Time.time - lastslashTime >= slashRate)
        {
            slashAtPlayer();
            lastslashTime = Time.time;
        }
    }

    public void slashAtPlayer()
    {
        
        Vector2 directionToEnemy = (Player.transform.transform.position - slashPoint.position).normalized;

        // 공격 방향 (플레이어가 향하고 있는 방향)
        Vector2 attackDirection = transform.right; // 캐릭터의 오른쪽 방향 (앞 방향)

        // 공격 방향과 적 방향 사이의 각도 계산
        float angleToEnemy = Vector2.Angle(attackDirection, directionToEnemy);

        // 적이 부채꼴 범위 내에 있는지 확인
        if (angleToEnemy <= attackAngle / 2)
        {
            player1 = GameObject.FindWithTag("Player").GetComponent<Player>();
            player1.hp -= 1;

        }
    }
    private bool IsPlayerObstructed()
    {
        Vector2 directionToPlayer = (Player.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, raycastDistance, obstacleLayer);

        // 디버그: 레이캐스트 시각화
        Debug.DrawRay(transform.position, directionToPlayer * raycastDistance, Color.red);

        return hit.collider != null && hit.collider.gameObject != Player;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트가 적인지 확인
        if (collision.CompareTag("bullet"))
        {
            hp -= 1;
        }
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
}