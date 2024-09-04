using UnityEngine;

public class bosssword : MonoBehaviour
{
    private GameObject Player;
    private Player player1;
    public float moveSpeed = 2f; // 이동 속도
    public float changeDirectionTime = 6f; // 방향 변경 주기
    public float wanderRange = 6f; // 랜덤 이동 범위
    public float rotationSpeed = 360f; // 회전 속도 (도 단위)
    public float detectionRadius = 10f; // 플레이어 탐지 반경
    public float shootDistance = 3f; // 총알 발사 거리
    public float slashRate = 1.5f; //  속도 (초 단위)
    public float raycastDistance = 20f; // Raycast 거리 (충돌 감지 거리)
    public LayerMask obstacleLayer; // 장애물 레이어
    public int hp = 100;
    
    public Transform slashPoint; // 총알 발사 위치
    public float attackrange = 1f;
    public float attackAngle = 110f;
    
    public float stopTime = 1f; // 멈춤 시간
    public float stopChance = 0.5f; // 멈춤 확률 (0.0 ~ 1.0 사이)

    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isslashing;
    private float lastslashTime;
    private float stopTimer;
    private bool isStopped;
    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        timer = changeDirectionTime;
        lastslashTime = -slashRate; // 처음 발사 시간을 초기화하여 첫 발사가 가능하도록 설정
        stopTimer = stopTime; // 멈춤 타이머 초기화
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isStopped)
        {
            animator.SetBool("move",false);
            animator.SetBool("stop",true);
        }
        else
        {
            animator.SetBool("move",true);
            animator.SetBool("stop",false);
        }
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
            // 랜덤 멈춤 및 이동
            if (!isStopped)
            {
                // 이동 중
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
            else
            {
                
                // 멈춤 중
                stopTimer -= Time.deltaTime;
                if (stopTimer <= 0)
                {
                    isStopped = false;
                    moveSpeed = 2f;
                }
            }
        }

        MoveTowardsTarget();
        HandleDirection(); // 시선 처리
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
            rb.AddForce(direction * moveSpeed);

            if (!followingPlayer && Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetNewRandomPosition();
            }
        }
    }

    private void HandleDirection()
    {
        Vector2 target;

        if (followingPlayer && !IsPlayerObstructed())
        {
            // 플레이어를 바라봄
            target = Player.transform.position;

            // 시선 처리: 타겟이 오른쪽에 있으면 시선을 오른쪽으로, 왼쪽에 있으면 왼쪽으로
            if (target.x > transform.position.x)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (target.x < transform.position.x)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }

            // 총알 발사 위치 회전: 타겟을 향해 회전
            Vector2 direction = target - (Vector2)slashPoint.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            slashPoint.eulerAngles = new Vector3(0, 0, targetAngle);
        }
        else
        {
            // 플레이어가 장애물 뒤에 있을 때는 적이 현재 바라보고 있는 방향을 유지
            // 기존 시선 유지 (총알 발사 위치도 함께 유지)
            Vector2 currentDirection = rb.velocity.normalized;

            if (currentDirection.x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (currentDirection.x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }

            // 총알 발사 위치를 현재 적의 바라보는 방향으로 유지
            slashPoint.eulerAngles = new Vector3(0, 0, slashPoint.eulerAngles.z);
        }
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
    
    
    
    





