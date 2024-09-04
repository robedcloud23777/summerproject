using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    private GameObject Player;
    private Player player1;
    public float moveSpeed = 2f; // 이동 속도
    public float changeDirectionTime = 5f; // 방향 변경 주기
    public float wanderRange = 10f; // 랜덤 이동 범위
    public float detectionRadius = 10f; // 플레이어 탐지 반경
    public float shootDistance = 3f; // 총알 발사 거리
    public float fireRate = 1f; // 연사 속도 (초 단위)
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform shootPoint; // 총알 발사 위치
    public float raycastDistance = 20f; // Raycast 거리 (충돌 감지 거리)
    public LayerMask obstacleLayer; // 장애물 레이어
    public int hp = 10;
    public int drop = 0;
    public GameObject tnfbxks;

    public float stopTime = 1f; // 멈춤 시간
    public float stopChance = 0.5f; // 멈춤 확률 (0.0 ~ 1.0 사이)

    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isShooting;
    private float lastFireTime;
    private float stopTimer;
    private bool isStopped;
<<<<<<< HEAD
<<<<<<< HEAD
    private Animator animator;
=======
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674

>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
    private Rigidbody2D rb;

    private void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        timer = changeDirectionTime;
        lastFireTime = -fireRate; // 처음 발사 시간을 초기화하여 첫 발사가 가능하도록 설정
        stopTimer = stopTime; // 멈춤 타이머 초기화
<<<<<<< HEAD
<<<<<<< HEAD
        animator = gameObject.GetComponent<Animator>();
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
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
            drop = Random.Range(1, 10);
            if (drop == 1)
            {
                Instantiate(tnfbxks, transform.position, shootPoint.transform.rotation * Quaternion.Euler(0, 0, 90));
                tnfbxks.GetComponent<Rigidbody2D>().AddForce(tnfbxks.transform.up * -2, ForceMode2D.Impulse);
            }
            Destroy(gameObject);
        }

        // 플레이어와의 거리 체크
        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);
        followingPlayer = distanceToPlayer <= detectionRadius;
        isShooting = distanceToPlayer <= shootDistance;

        if (isShooting)
        {
            // 총알 발사
            TryShootAtPlayer();
            // 멈춤
            moveSpeed = 0f;
        }
        else if (followingPlayer && !IsPlayerObstructed())
        {
            // 플레이어를 향해 이동
            targetPosition = Player.transform.position;
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
<<<<<<< HEAD
<<<<<<< HEAD
                    
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
                    if (Random.value < stopChance)
                    {
                        isStopped = true;
                        stopTimer = stopTime;
                        moveSpeed = 0f;
<<<<<<< HEAD
<<<<<<< HEAD
                        
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
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
<<<<<<< HEAD
<<<<<<< HEAD
                
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
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
<<<<<<< HEAD
<<<<<<< HEAD
        
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
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
<<<<<<< HEAD
<<<<<<< HEAD
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
            Vector2 direction = target - (Vector2)shootPoint.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            shootPoint.eulerAngles = new Vector3(0, 0, targetAngle);
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
            shootPoint.eulerAngles = new Vector3(0, 0, shootPoint.eulerAngles.z);
        }
=======
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
        Vector2 target = followingPlayer ? Player.transform.position : targetPosition;

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
        Vector2 direction = target - (Vector2)shootPoint.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shootPoint.eulerAngles = new Vector3(0, 0, targetAngle);
<<<<<<< HEAD
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
    }

    private void TryShootAtPlayer()
    {
<<<<<<< HEAD
<<<<<<< HEAD
        
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
=======
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
        if (Time.time - lastFireTime >= fireRate)
        {
            ShootAtPlayer();
            lastFireTime = Time.time;
        }
    }

    private void ShootAtPlayer()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(shootPoint.right * 200);
        }
    }

    private bool IsPlayerObstructed()
    {
        Vector2 directionToPlayer = (Player.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, raycastDistance, obstacleLayer);

        return hit.collider != null && hit.collider.gameObject != Player;
    }

    void OnTriggerEnter2D(Collider2D collision)
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
<<<<<<< HEAD
<<<<<<< HEAD
}
=======
}
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
=======
}
>>>>>>> 21ba89bea4921a48af6ccd1250ccf813eda72674
