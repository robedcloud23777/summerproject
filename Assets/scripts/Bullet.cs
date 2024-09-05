using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         Destroy(gameObject,5);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Map"))
        {
            Destroy(gameObject);
        }
    }
}
