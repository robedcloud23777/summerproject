using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class surutan : MonoBehaviour
{
    public Player player1;
    public Transform player;
    public bool isnotbumb = true;
    public float timer = 0.7f;
    public float range = 2f;
    public bool isthrowed = false;
    
    void Update()
    {


        if (isthrowed)
        {
            timer -= Time.deltaTime;
        }
        float distance = Vector2.Distance(transform.position, player.position);
        if (!isnotbumb && timer <= 0)
        {
            if (distance <= range)
            {
                player1.hp -= 5f;
            }
            
        }
    }
    
    
}
