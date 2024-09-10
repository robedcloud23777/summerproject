using UnityEngine;
using System.Collections;

public class Elitetang : MonoBehaviour
{
    private GameObject Player;
    private Player player1;
    public float moveSpeed = 1.5f; // 이동 속도
    public float changeDirectionTime = 6f; // 방향 변경 주기
    public float wanderRange = 6f; // 랜덤 이동 범위
    public float rotationSpeed = 360f; // 회전 속도 (도 단위)
    public float detectionRadius = 10f; // 플레이어 탐지 반경
    public float shootDistance = 3f; // 총알 발사 거리
    public float fireRate = 2.2f; // 연사 속도 (초 단위)
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform shootPoint; // 총알 발사 위치
    public float raycastDistance = 20f; // Raycast 거리 (충돌 감지 거리)
    public LayerMask obstacleLayer; // 장애물 레이어
    public int hp = 50;
    public int drop = 0;
    public GameObject weapon;
    private Rigidbody2D rb;
    private Animator animator;
    public float maxSpeed = 10f;


    public float stopTime = 1f; // 멈춤 시간
    public float stopChance = 0.5f; // 멈춤 확률 (0.0 ~ 1.0 사이)
    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isShooting;
    private float lastFireTime;
    private float stopTimer;
    private bool isStopped;
    private Renderer _renderer;
    private Color _originalColor;
    public Color damageColor = Color.red; // 데미지 색상

    private void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        timer = changeDirectionTime;
        lastFireTime = -fireRate; // 처음 발사 시간을 초기화하여 첫 발사가 가능하도록 설정
        SetNewRandomPosition();
        stopTimer = stopTime; // 멈춤 타이머 초기화
        animator = gameObject.GetComponent<Animator>();
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
    }
    void FixedUpdate()
    {
        // 현재 속도를 확인
        if (rb.velocity.magnitude > maxSpeed)
        {
            // 최대 속력으로 제한
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
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
            if (drop >= 5)
            {
                Instantiate(weapon, transform.position, shootPoint.transform.rotation * Quaternion.Euler(0, 0, 90));
                weapon.GetComponent<Rigidbody2D>().AddForce(weapon.transform.up * -2, ForceMode2D.Impulse);
            }
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
            // 이동 속도 재설정
            moveSpeed = 2f;
        }
        else
        {
            // 랜덤 이동
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
        Vector2 target = followingPlayer ? Player.transform.position : targetPosition;

        // isShooting 상태에 따라 시선 처리
        if (isShooting)
        {
            // 총을 쏘는 방향으로 시선을 맞춤
            Vector2 shootDirection = target - (Vector2)transform.position;
            if (shootDirection.x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false; // 오른쪽을 바라봄
            }
            else if (shootDirection.x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true; // 왼쪽을 바라봄
            }

            // 총알 발사 위치 회전: 타겟을 향해 회전
            Vector2 direction = target - (Vector2)shootPoint.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            shootPoint.eulerAngles = new Vector3(0, 0, targetAngle);
            if (targetAngle > 90 || targetAngle < -90)
            {
                Debug.Log(targetAngle);
                shootPoint.GetComponent<SpriteRenderer>().flipY = true;
            }
            else
            {   
                
                shootPoint.GetComponent<SpriteRenderer>().flipY = false;
            }
        }
        else
        {
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
        float DFP = 0.1f;
        Vector2 rks = (target - (Vector2)transform.position).normalized;
        shootPoint.transform.position = (Vector2)transform.position + rks * DFP;
        
    }

    private void TryShootAtPlayer()
    {
        // 총알 발사 시간을 체크하여 일정 시간 간격으로 총알 발사
        if (Time.time - lastFireTime >= fireRate)
        {
            ShootAtPlayer();
            lastFireTime = Time.time;
        }
    }

    private void ShootAtPlayer()
    {
        isStopped = true;
    
        // 샷건 효과: 10발의 총알을 발사
        int numberOfBullets = 5;
        float spreadAngle = 30f; // 총알 퍼지는 각도 (각도 범위 내에서 퍼짐)
        float angleStep = spreadAngle / (numberOfBullets - 1); // 각 총알 사이의 각도 차이
        float startAngle = -spreadAngle / 2; // 첫 총알이 시작하는 각도

        for (int i = 0; i < numberOfBullets; i++)
        {
            // 각 총알의 각도를 계산
            float angle = startAngle + (angleStep * i);
            Quaternion rotation = shootPoint.rotation * Quaternion.Euler(0, 0, angle);
        
            // 총알을 생성하고 발사
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(rotation * Vector2.right * 200);
            }
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
        if (collision.CompareTag("bullet"))
        {
            hp -= 1;
            StartCoroutine(FlashDamageColor());
        }
    }

    private IEnumerator FlashDamageColor()
    {
        _renderer.material.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        _renderer.material.color = _originalColor;
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
    public void bombEx(int damage)
    {
        hp -= damage;
        StartCoroutine(FlashDamageColor());
    }
}
