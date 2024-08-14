using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class surutan : MonoBehaviour
{
    private GameObject Player;
    private Player player1;
    public float timer = 0.7f;
    public float range = 2f;

    private void Start()
    {
        Player = GameObject.Find("player");
        player1 = Player.GetComponent<Player>();
    }
    void Update()
    { 
        timer -= Time.deltaTime;
   
        float distance = Vector2.Distance(transform.position, Player.transform.position);
        if (timer <= 0)
        {
            if (distance <= range)
            {
                player1.hp -= 1;
            }
            Destroy(gameObject);
        }
    }
}
