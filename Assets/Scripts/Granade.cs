using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Bomb : MonoBehaviour
{
    public float delay = 1.5f;
    public float radius = 2f; // 폭발 범위
    public int exdamage = 25;   // 폭발 피해량
    public GameObject explosionEffect; // 폭발 효과

    private bool exploded = false;

    void Start()
    {
        Invoke("Explode", delay);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="bullet"){
            Explode();
        }
    }

    public void Explode()
    {
        if (exploded) return; 

        exploded = true;

        // 폭발 효과 생성
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        // 폭발 범위 내의 오브젝트 감지
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D nearbyObject in colliders)
        {
            // if (nearbyObject.tag == "Enemy")
            // {
            //     try{
            //         nearbyObject.GetComponent<Enemy1>().bombEx(exdamage);
            //     }catch{
            //         try{
            //             nearbyObject.GetComponent<Enemy2>().bombEx(exdamage);
            //         }catch{
            //             try{
            //                 nearbyObject.GetComponent<Enemy3>().bombEx(exdamage);
            //             }catch{
            //                 try{
            //                     nearbyObject.GetComponent<Eliterunning1>().bombEx(exdamage);
            //                 }catch{
            //                     try{
            //                         nearbyObject.GetComponent<Elitetang>().bombEx(exdamage);
            //                     }catch{
            //                         try{
            //                             nearbyObject.GetComponent<bossgun>().bombEx(exdamage);
            //                         }catch{
            //                             try{
            //                                 nearbyObject.GetComponent<bossthrow>().bombEx(exdamage);
            //                             }catch{
            //                                 try{
            //                                     nearbyObject.GetComponent<bosssword>().bombEx(exdamage);
            //                                 }finally{
            //                                     Debug.Log("오류");
            //                                 }
            //                             }
            //                         }
            //                     }
            //                 }
            //             }
            //         }
            //     }
            // }
        }

        Destroy(gameObject);
    }
}
