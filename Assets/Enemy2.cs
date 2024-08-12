using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy2 : MonoBehaviour
{
    public Player player;
    public float stopRange = 1f;
    public float speed = 6f; // 적의 이동 속도
    public Transform playertransform; // 플레이어의 위치
    public float hp = 10f;

    // Update is called once per frame
    void Update()
    {
        
        
        
        MoveTowardsPlayer();
        Rotate();
        float distance = Vector2.Distance(transform.position, playertransform.position);

        if (distance <= stopRange)
        {
            Explode();
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
    
    
    void MoveTowardsPlayer()
    {


        // 플레이어를 향한 방향 벡터 계산
        Vector3 direction = (playertransform.position - transform.position).normalized;

        // 적의 위치를 플레이어를 향해 업데이트
        transform.position += direction * speed * Time.deltaTime;

    }
    
    
    void OnCollisionEnter2D(Collision2D collision)
    {

       

        if (collision.gameObject.CompareTag("bullet"))
        {
            hp -= 5;
        }
    }
    
    
    void Explode()
    {

        
        
        
        player.hp -= 5;
        

        // 자폭 후 적 오브젝트를 파괴
        Destroy(gameObject);
    }
}
