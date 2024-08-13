using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy2 : MonoBehaviour
{
    public Player player1;
    public float stopRange = 1f;
    public float speed = 6f; // 적의 이동 속도
    public Transform player; // 플레이어의 위치
    public float hp = 10f;

    // Update is called once per frame
    void Update()
    {
        
        
        
        MoveTowardsPlayer();
        Rotate();
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stopRange)
        {
            
            Explode();
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
    
    
    void MoveTowardsPlayer()
    {


        // 플레이어를 향한 방향 벡터 계산
        Vector3 direction = (player.position - transform.position).normalized;

        // 적의 위치를 플레이어를 향해 업데이트
        transform.position += direction * speed * Time.deltaTime;

    }
    
    
    
    
    
    void Explode()
    {

        
        
        
        player1.hp -= 5;
        

        // 자폭 후 적 오브젝트를 파괴
        Destroy(gameObject);
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
