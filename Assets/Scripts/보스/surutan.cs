using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class surutan : MonoBehaviour
{
    private GameObject Player;
    private Player player1;
    public float timer = 0.7f;
    public float range = 2f; // ���� ����
    public GameObject explosionEffect; // ���� ȿ��


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
            // �÷��̾ ���� ���� ���� ������ �������� ����
            if (distance <= range)
            {
                player1.GetDamage();
            }
            // ���� ȿ�� ����
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
