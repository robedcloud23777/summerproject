using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class surutan : MonoBehaviour
{
    private GameObject Player;
    private Player player1;
    public float timer = 0.7f;
    public float range = 2f; // 폭발 범위
    public GameObject explosionEffect; // 폭발 효과


    void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
    }

    private void Update()
    {
        Explode();
    }


    public void Explode()
    {
        timer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, Player.transform.position);
        if (timer <= 0)
        {
            // 플레이어가 일정 범위 내에 있으면 데미지를 입힘
            if (distance <= range)
            {
                player1.GetDamage();
            }
            // 폭발 효과 생성
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
