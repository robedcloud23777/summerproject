using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public float hp;
    public float ox=25; //산소
    public float moveSpeed=5f;
    public GameObject gun;
    
    public Slider hpBar;
    public Slider oxbar;
    public GameObject tank;
    public Transform tankSpawnpoint;
    bool damaged=false;

    void Start()
    {    
        InvokeRepeating("Oxsystem",2,2);
        InvokeRepeating("tankSpawn",4,Random.Range(7f,10f));
    }

    // Update is called once per frame
    void Update()
    {
         Move();  
         Rotate()
;    }
    void Move() // 이동
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float vt = Input.GetAxisRaw("Vertical");

        Vector3 moveVector = new Vector3(hor, vt).normalized;
        transform.position += moveVector*Time.deltaTime*moveSpeed;
    }
    void Rotate()    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float dx = mousePos.x - transform.position.x;
        float dy = mousePos.y - transform.position.y;

        float angle = Mathf.Atan2(dy, dx)*Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
        
    }
    public void getDamage(float damage) // 데미지 받기
    { 
        Debug.Log("작동");
        if(damaged)return;
        damaged=true;
        hp-=damage;
        gameObject.GetComponent<SpriteRenderer>().color=new Color(255,255,255,0.6f);
        gameObject.layer=6;
        if(hp <= 0) {
            Time.timeScale = 0f;
            CancelInvoke();
            Destroy(gameObject);
            Destroy(gun.gameObject);
        }
        hpBar.value=hp;
        
        Invoke("Dmgd",0.5f);  // 무적시간
    }
    void Dmgd(){
        gameObject.GetComponent<SpriteRenderer>().color=new Color(255,255,255,1f);
        gameObject.layer=3;
        damaged=false;
    }
    void Oxsystem(){
     if(ox>0){
        ox-=5;
        oxbar.value=ox;
     }else{
        getDamage(2);
     } 
    }
    void tankSpawn(){
            Instantiate(tank, tankSpawnpoint.position, transform.rotation*Quaternion.Euler(0,0,0));
    }
    void OnCollisionEnter2D(Collision2D collision){ // 적 충돌 감지하고 데미지 함수 실행
        if(collision.gameObject.tag=="Enemy"){
            // Debug.Log("작동");
            getDamage(collision.gameObject.GetComponent<Enemy>().damage);
        }
    }
}
