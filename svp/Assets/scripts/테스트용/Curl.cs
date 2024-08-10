using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Curl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }
    void Rotate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float dx = mousePos.x - transform.position.x;
        float dy = mousePos.y - transform.position.y;

        float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 movePosition = Vector3.zero;

        if ((angle > 90) || (angle < -90))
        {
            movePosition = Vector3.left;
            GetComponent<SpriteRenderer>().flipY = true;}
                
            
            // GetComponent<SpriteRenderer>().flipY = true;
        else if ((angle < 90) || (angle > -90))
        {
            movePosition = Vector3.right;
            GetComponent<SpriteRenderer>().flipY = false;
        }
    }
}


