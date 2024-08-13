using Unity.VisualScripting;
using UnityEngine;

public class bossthrow : MonoBehaviour
{
    public GameObject bombOb;
    
    public float moveSpeed = 2f; // 이동 속도
    public float changeDirectionTime = 5f; // 방향 변경 주기
    public float wanderRange = 10f; // 랜덤 이동 범위
    public float rotationSpeed = 360f; // 회전 속도 (도 단위)
    public float detectionRadius = 5f; // 플레이어 탐지 반경
    public float shootDistance = 3f; // 총알 발사 거리
    public float fireRate = 1f; // 연사 속도 (초 단위)
    public Transform player; // 플레이어의 Transform
    public Transform throwPoint; // 총알 발사 위치
    public float hp = 100f;
    public surutan surutan;

    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isthrowing;
    private float lastthrowTime;

    private void Start()
    {
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
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
            return;
        }

        // 플레이어와의 거리 체크
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        followingPlayer = distanceToPlayer <= detectionRadius;
        isthrowing = distanceToPlayer <= shootDistance;

        if (isthrowing)
        {
            // 총알 발사
            TrythrowAtPlayer();
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
        
        GameObject newBomb = Instantiate(bombOb, transform.position, throwPoint.transform.rotation*Quaternion.Euler(0,0,90));
        newBomb.GetComponent<Rigidbody2D>().AddForce(newBomb.transform.up * -5,ForceMode2D.Impulse);
        surutan.isthrowed = true;
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