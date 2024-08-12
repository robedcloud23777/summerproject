using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class SimpleEnemyFollow : MonoBehaviour
{
    public float hp = 10f;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public Transform player; // 플레이어의 위치
    public float speed = 2f; // 적의 이동 속도
    public float stopRange = 5f;
    public float waitTimer = 0f;
    public bool isStopped = false;
    public float waitTime = 0f;
    public float bulletForce = 700f; // 총알 발사 힘
    public float playerrange = 20f;
    
    public float movementRange = 0.1f;  // 움직임 범위
    public float speeds = 1f;
    private Rigidbody2D rb;

    
    void Start()
    {
        // Rigidbody2D 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {

        Rotate();
        // 플레이어와의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);

        if (playerrange <= distance)
        {
            float randomX = Random.Range(-movementRange, movementRange);
            float randomY = Random.Range(-movementRange, movementRange);

            // 새로운 위치 계산
            Vector2 randomMovement = (new Vector2(randomX, randomY) * speeds * Time.deltaTime).normalized;

            // Rigidbody2D를 사용하여 오브젝트 이동
            rb.MovePosition(rb.position + randomMovement);
        }


        if (distance <= stopRange && !isStopped)
        {


            isStopped = true;
            FireBullet();
            waitTime = Random.Range(0.5f, 1f);
            waitTimer = 0f;


        }

        if (isStopped)
        {
            waitTimer += Time.deltaTime;
            
            if (waitTimer >= waitTime)
            {
                isStopped = false;

            }
        }

        if (distance <= playerrange && !isStopped)
        {
            MoveTowardsPlayer();
        }


    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("bullet"))
        {


            Debug.Log("hello");

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("bullet"))
        {
            hp -= 5;
        }
    }


    void MoveTowardsPlayer()
    {


        // 플레이어를 향한 방향 벡터 계산
        Vector3 direction = (player.position - transform.position).normalized;

        // 적의 위치를 플레이어를 향해 업데이트
        transform.position += direction * speed * Time.deltaTime;

    }



    void FireBullet()
    {
        // 총알 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation * Quaternion.Euler(0,0,-90));
        
        // 총알의 방향 설정 (플레이어를 향함)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            
            rb.AddForce(rb.transform.up * 200);
        }
    }

    void Rotate()
    {
        // 플레이어를 향한 방향 벡터 계산
        Vector3 direction = player.position - transform.position;
        
        // 방향 벡터를 각도로 변환
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // z축을 기준으로 회전 설정
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
