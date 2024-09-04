using UnityEngine;

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
    public int hp = 50;
    public int drop = 0;
    public GameObject weapon;
    private Rigidbody2D rb;


    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isShooting;
    private float lastFireTime;

    private void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
        timer = changeDirectionTime;
        lastFireTime = -fireRate; // 처음 발사 시간을 초기화하여 첫 발사가 가능하도록 설정
        SetNewRandomPosition();
    }

    private void Update()
    {

        if (hp <= 0)
        {
            drop = Random.Range(1, 5);
            if (drop == 1)
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
        else if (followingPlayer)
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
        int bulletCount = 10; // 발사할 총알 개수
        float spreadAngle = 30f; // 총알 퍼짐 각도 (양옆으로 30도씩 퍼지도록 설정)

        for (int i = 0; i < bulletCount; i++)
        {
            // 총알의 퍼짐 각도 계산
            float angleStep = spreadAngle / (bulletCount - 1);
            float currentAngle = -spreadAngle / 2 + angleStep * i;

            // 총알 프리팹 인스턴스화
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position,
                transform.rotation * Quaternion.Euler(0, 0, currentAngle - 90));

            // 총알의 방향 설정 (플레이어를 향함)
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(rb.transform.up * 200);
            }
        }
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
