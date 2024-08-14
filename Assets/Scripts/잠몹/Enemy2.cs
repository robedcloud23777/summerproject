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
                Instantiate(tnfbxks, transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 90));
                tnfbxks.GetComponent<Rigidbody2D>().AddForce(tnfbxks.transform.up * -2, ForceMode2D.Impulse);
            }
            Destroy(gameObject);
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SetNewRandomPosition();
            timer = changeDirectionTime;
        }

        Rotate();
        if (followingPlayer)
        {
            MoveTowardsPlayer();
        }

        if (distanceToPlayer <= stopRange)
        {
            StartCoroutine(ExplodeAfterDelay(1f)); // 1초 후 폭발
        }
    }

    void Rotate()
    {
        // 플레이어를 향한 방향 벡터 계산
        Vector3 direction = Player.transform.position - transform.position;

        // 방향 벡터를 각도로 변환
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // z축을 기준으로 회전 설정
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void SetNewRandomPosition()
    {
        float randomX = Random.Range(-wanderRange, wanderRange);
        float randomY = Random.Range(-wanderRange, wanderRange);
        targetPosition = new Vector2(transform.position.x + randomX, transform.position.y + randomY);
    }

    void MoveTowardsPlayer()
    {
        // 플레이어를 향한 방향 벡터 계산
        Vector3 direction = (Player.transform.position - transform.position).normalized;

        // Rigidbody2D를 사용하여 힘을 가해 적이 플레이어를 향해 이동하도록 함
        rb.AddForce(direction * moveSpeed, ForceMode2D.Force);
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        isExploding = true; // 폭발 대기 상태 활성화 
        yield return new WaitForSeconds(delay); // 1초 대기
        Explode();
    }

    void Explode()
    {
        player1.hp -= 1;

        // 자폭 후 적 오브젝트를 파괴
        Destroy(gameObject);
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
