using UnityEngine;

public class Eliterunning1 : MonoBehaviour
{
    public float moveSpeed = 4f; // 이동 속도
    public float changeDirectionTime = 6f; // 방향 변경 주기
    public float wanderRange = 6f; // 랜덤 이동 범위
    public float rotationSpeed = 360f; // 회전 속도 (도 단위)
    public float detectionRadius = 10f; // 플레이어 탐지 반경
    public float shootDistance = 3f; // 총알 발사 거리
    public float fireRate = 1.5f; // 연사 속도 (초 단위)
    public Transform player; // 플레이어의 Transform
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform shootPoint; // 총알 발사 위치
    public float hp = 30f;
    public int drop = 0;
    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isShooting;
    private float lastFireTime;

    private void Start()
    {
        timer = changeDirectionTime;
        lastFireTime = -fireRate; // 처음 발사 시간을 초기화하여 첫 발사가 가능하도록 설정
        SetNewRandomPosition();
    }

    private void Update()
    {
        
        if (hp <= 0)
        {
            Destroy(gameObject);
            drop = Random.Range(1, 5);
            if (drop == 1)
            {
                Instantiate(weapon, transform.position, shootPoint.transform.rotation * Quaternion.Euler(0, 0, 90);
                weapon.GetComponent<Rigidbody2D>().AddForce(weapon.transform.up * -2,ForceMode2D.Impulse);
            }


        }
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
            return;
        }

        // 플레이어와의 거리 체크
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        followingPlayer = distanceToPlayer <= detectionRadius;
        isShooting = distanceToPlayer <= shootDistance;

        if (isShooting)
        {
            // 총알 발사
            TryShootAtPlayer();
            // 멈춤
            moveSpeed = 0f;
        }
        else if (followingPlayer)
        {
            // 플레이어를 향해 이동
            targetPosition = player.position;
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
        float step = moveSpeed * Time.deltaTime;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);

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

    public void TryShootAtPlayer()
    {
        // 총알 발사 시간을 체크하여 일정 시간 간격으로 총알 발사
        if (Time.time - lastFireTime >= fireRate)
        {
            ShootAtPlayer();
            lastFireTime = Time.time;
        }
    }

    public  void ShootAtPlayer()
    {
        // 총알 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position,
            transform.rotation * Quaternion.Euler(0, 0, -90));

        // 총알의 방향 설정 (플레이어를 향함)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {

            rb.AddForce(rb.transform.up * 200);
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트가 적인지 확인
        if (collision.CompareTag("bullet"))
        {
            hp -= 5f;
        }
        
    }
}
