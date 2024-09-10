using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject Player;
    private Player player1;

    void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player1.GetDamage();
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Map"))
        {
            Destroy(gameObject);
        }
    }
}
