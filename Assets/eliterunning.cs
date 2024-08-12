using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class eliterunning : MonoBehaviour
{
    public float hp = 10f;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public Transform playertransform; // 플레이어의 위치
    public float speed = 3f; // 적의 이동 속도
    public float stopRange = 10f;
    public float waitTimer = 0f;
    public bool isStopped = false;
    public float waitTime;
    public float bulletForce = 700f; // 총알 발사 힘
    public float playerrange = 20f;

    private Rigidbody2D rb1;

    public float movementRange = 2f; // 움직임 범위
    public float speeds = 1f;
    // Update is called once per frame


    void Start()
    {
        rb1 = GetComponent<Rigidbody2D>();
        StartCoroutine(MoveRandomly());
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, playertransform.position);

        Rotate();
        if (distance <= stopRange && !isStopped)
        {


            isStopped = true;
            waitTime = Random.Range(2f, 3f);
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
            Rotate();
        }
    }





    void Rotate()
    {
        // 플레이어를 향한 방향 벡터 계산
        Vector3 direction = playertransform.position - transform.position;

        // 방향 벡터를 각도로 변환
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // z축을 기준으로 회전 설정
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }






    void OnCollisionEnter2D(Collision2D collision)
    {



        if (collision.gameObject.CompareTag("bullet"))
        {
            hp -= 5;
        }
    }

    void FireBullet()
    {
        // 총알 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position,
            transform.rotation * Quaternion.Euler(0, 0, -90));

        // 총알의 방향 설정 (플레이어를 향함)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {

            rb.AddForce(rb.transform.up * 200);
        }
    }

    IEnumerator MoveRandomly()
    {
        
        float distance = Vector2.Distance(transform.position, playertransform.position);
        if (playerrange <= distance)
        {
            
            isStopped = false;
            float randomX = Random.Range(-movementRange, movementRange);
            float randomY = Random.Range(-movementRange, movementRange);

            // 새로운 위치 계산
            Vector2 randomMovement = new Vector2(randomX, randomY) * speed;

            // Rigidbody2D를 사용하여 오브젝트 이동
            rb1.MovePosition(rb1.position + randomMovement * Time.fixedDeltaTime);

            // 1~2초 동안 대기 (쿨타임)
            float cooldownTime = Random.Range(1f, 2f);
            yield return new WaitForSeconds(cooldownTime);


        }

    }

    void MoveTowardsPlayer()
    {


        // 플레이어를 향한 방향 벡터 계산
        Vector3 direction = (playertransform.position - transform.position).normalized;

        // 적의 위치를 플레이어를 향해 업데이트
        transform.position += direction * speed * Time.deltaTime;

    }
}