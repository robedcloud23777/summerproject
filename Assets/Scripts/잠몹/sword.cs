using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword : MonoBehaviour
{
    public bool swing = false; // 외부에서 접근 가능한 bool 변수
    public Animator animator;
    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (swing == true)
        {  
            animator.SetTrigger("swing");
            swing = false;
        }
        
    }
}
