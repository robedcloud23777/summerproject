using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossswing : MonoBehaviour
{
    public bool Swinging = false; // 외부에서 접근 가능한 bool 변수
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Swinging == true)
        {  
            Debug.Log("hello");
            animator.SetTrigger("swing");
            Swinging = false;
        }
        
    }
}