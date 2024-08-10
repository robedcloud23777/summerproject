using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public int hp;
    bool damaged=false;
    public Image dashPannel;
    bool dashable=true;
    public float dashCooltime=5f;
    public float moveSpeed=5f;
    public float dashSpeed=30f;
    bool dashing=false;
    public GameObject gun;
    private GameObject Fpannel;
    Gun weapon;    int maxAmmo;
    bool onW= false;
    float shootingCooltime;
    float reloadCooltime;
    int ammo;
    int damage;
    GameObject W;
    void Start()
    {    
        dashPannel.color= new Color(255,255,255,0f);
        Fpannel = GameObject.Find("F");
        weapon =  GameObject.Find("Gun").GetComponent<Gun>();

    }

    // Update is called once per frame
    void Update()
    {
         Move();  
         Flip();
         Dash();
         changeW();
    }
    void changeW(){
        if(!Input.GetKey(KeyCode.F)||!onW)return;
        Destroy(W);
        weapon.maxAmmo = maxAmmo;
        weapon.ammo = ammo;
        weapon.shootingCooltime = shootingCooltime;
        weapon.reloadCooltime = reloadCooltime;
        weapon.damage = damage;
    }
    void Move() // 이동
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float vt = Input.GetAxisRaw("Vertical");

        Vector3 moveVector = new Vector3(hor, vt).normalized;
        transform.position += moveVector*Time.deltaTime*(dashing?dashSpeed:moveSpeed);
        dashPannel.transform.position = new Vector3(transform.position.x+0.7f,transform.position.y+0.7f);

    }
    void Flip()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float dx = mousePos.x - transform.position.x;
        float dy = mousePos.y - transform.position.y;

        float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        if ((angle > 90) || (angle < -90))
        {
            GetComponent<SpriteRenderer>().flipX = true;
        } 
        else if ((angle < 90) || (angle > -90))
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    void Dash(){
        if(!Input.GetKeyDown(KeyCode.Space)||!dashable)return;
        dashing=true;
        dashable=false;
        dashPannel.color= new Color(255,255,255,0.6f);
        dashPannel.GetComponent<Animator>().SetTrigger("reload");
        dashPannel.GetComponent<Animator>().speed=1/dashCooltime;
        Invoke("dashFinish",0.2f);
        Invoke("dashCool",dashCooltime);
        gameObject.GetComponent<SpriteRenderer>().color=new Color(255,255,255,0.6f);
        gameObject.layer=6;
    }
    void dashFinish(){
        dashing=false;
        gameObject.GetComponent<SpriteRenderer>().color=new Color(255,255,255,1f);
        gameObject.layer=3;
    }void dashCool(){
        dashPannel.color = new Color(255,255,255,0f);
        dashPannel.GetComponent<Animator>().speed=100;
        dashable=true;
    }
    public void getDamage(int damage) // 데미지 받기
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
    //      hp  바 표시부분        
        Invoke("Dmgd",0.5f);  // 무적시간
    }
    void Dmgd(){
        gameObject.GetComponent<SpriteRenderer>().color=new Color(255,255,255,1f);
        gameObject.layer=3;
        damaged=false;
    }
    void OnCollisionEnter2D(Collision2D collision){ // 적 충돌 감지하고 데미지 함수 실행
        if(collision.gameObject.tag=="damageObject"){
            // getDamage(collision.gameObject.GetComponent<Dobject>().damage);
        }
    }
    void OnTriggerEnter2D(Collider2D collision){
        W=collision.gameObject;
        if(collision.gameObject.tag=="gun"){
            if(collision.gameObject.name=="w1") setW(7,7,3,0.7f,2.5f);
            if(collision.gameObject.name=="w2") setW(1,1,10,0,3f);
            Fpannel.SetActive(true);
            Fpannel.transform.position = new Vector2(collision.gameObject.transform.position.x,collision.gameObject.transform.position.y+1);
        }
    }
    void setW(int maxAmmo_, int ammo_, int damage_, float shootingCooltime_, float reloadCooltime_)
    {
        maxAmmo = maxAmmo_;
        ammo = ammo_;
        damage = damage_;
        shootingCooltime = shootingCooltime_;
        reloadCooltime = reloadCooltime_;
        onW=true;

    }
    void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.tag=="gun"){
            onW=false;
            Fpannel.SetActive(false);  
        }
    }
}
