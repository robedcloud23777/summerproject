using UnityEngine;

public class bosssword : MonoBehaviour
{
    public Player player1;
    public float moveSpeed = 2f; // 이동 속도
    public float changeDirectionTime = 6f; // 방향 변경 주기
    public float wanderRange = 6f; // 랜덤 이동 범위
    public float rotationSpeed = 360f; // 회전 속도 (도 단위)
    public float detectionRadius = 10f; // 플레이어 탐지 반경
    public float shootDistance = 3f; // 총알 발사 거리
    public float slashRate = 1.5f; //  속도 (초 단위)
    public Transform player; // 플레이어의 Transform
    public float hp = 100f;
    
    public Transform slashPoint; // 총알 발사 위치
    public float attackrange = 1f;
    public float attackAngle = 110f;

    private Vector2 targetPosition;
    private float timer;
    private bool followingPlayer;
    private bool isslashing;
    private float lastslashTime;

    private void Start()
    {
        timer = changeDirectionTime;
        lastslashTime = -slashRate; // 처음 발사 시간을 초기화하여 첫 발사가 가능하도록 설정
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
        isslashing = distanceToPlayer <= shootDistance;

        if (isslashing)
        {
            // 검 휘두르기
            TryslashAtPlayer();
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
        
        Vector2 directionToEnemy = (player.transform.position - slashPoint.position).normalized;

        // 공격 방향 (플레이어가 향하고 있는 방향)
        Vector2 attackDirection = transform.right; // 캐릭터의 오른쪽 방향 (앞 방향)

        // 공격 방향과 적 방향 사이의 각도 계산
        float angleToEnemy = Vector2.Angle(attackDirection, directionToEnemy);

        // 적이 부채꼴 범위 내에 있는지 확인
        if (angleToEnemy <= attackAngle / 2)
        {
            player1 = GameObject.FindWithTag("Player").GetComponent<Player>();
            player1.hp -= 5f;

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
    
    
    
    





