using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    // Public variables
    public int hp=3;
    public Image dashPannel;
    public float dashCooltime = 5f;
    public float moveSpeed = 5f;
    public float dashSpeed = 30f;
    public GameObject gun;
    public GameObject bombOb;
    public int bombremain = 1;

    // Private variables
    private bool damaged = false;
    private bool dashable = true;
    private bool dashing = false;
    private GameObject Fpannel;
    private Gun weapon;
    string Wn;
    bool onP=false;
    private int maxAmmo;
    private bool onW = false;
    private float shootingCooltime;
    private float reloadCooltime;
    private int ammo;
    private int damage;
    private int gun_sprite;
    private Vector2 fireSet;
    private GameObject W;
    private bool onG = false;
    public Image heart;
    public Image heart1;
    public Image heart2;
    public Sprite hert;
    public Sprite hert_;
    public GameObject pnl;
    public Sprite[] gunTexture;

    void Start()
    {
        pnl.SetActive(false);
        dashPannel.color = new Color(255, 255, 255, 0f);
        Fpannel = GameObject.Find("F");
        weapon = GameObject.Find("Gun").GetComponent<Gun>();
    }

    void FixedUpdate()
    {
        Move();
        Flip();
        Dash();
        ChangeWeapon();
        ThrowBomb();
        Heart();
        Pnl();
        gun.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite=gunTexture.ElementAt<Sprite>(weapon.gun_sprite);
    }
    void Pnl(){
        pnl.transform.GetChild(0).gameObject.transform.GetChild(3).gameObject.GetComponent<Text>().text="연사속도 : "+(math.floor((1/weapon.shootingCooltime)*100f)/100f).ToString()+"/s";
        pnl.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text="탄창 : "+weapon.maxAmmo;
        pnl.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.GetComponent<Text>().text="재장전시간 : "+weapon.reloadCooltime+"s";
        pnl.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.GetComponent<Text>().text="공격력 : "+weapon.damage;
        pnl.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text=weapon.Wn;
        pnl.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Image>().sprite=gunTexture.ElementAt<Sprite>(weapon.gun_sprite);
        
        pnl.transform.GetChild(1).gameObject.transform.GetChild(3).gameObject.GetComponent<Text>().text="연사속도 : "+(math.floor((1/shootingCooltime)*100f)/100f).ToString()+"/s";
        pnl.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text="탄창 : "+maxAmmo+"발";
        pnl.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.GetComponent<Text>().text="재장전시간 : "+reloadCooltime+"s";
        pnl.transform.GetChild(1).gameObject.transform.GetChild(4).gameObject.GetComponent<Text>().text="공격력 : "+damage;
        pnl.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text=Wn;
        pnl.transform.GetChild(1).GetChild(5).GetChild(0).GetComponent<Image>().sprite=gunTexture.ElementAt<Sprite>(gun_sprite);

    }
    void Heart(){
        if(hp>2){
            heart.sprite=hert;
            heart1.sprite=hert;
            heart2.sprite=hert;
            
        }else if(hp>1){
            heart.sprite=hert;
            heart1.sprite=hert;
            heart2.sprite=hert_;
        }else if(hp>0){
            heart.sprite=hert;
            heart1.sprite=hert_;
            heart2.sprite=hert_;
        }else{
            heart.sprite=hert_;
            heart1.sprite=hert_;
            heart2.sprite=hert_;
        }
    }
    void Move()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float vt = Input.GetAxisRaw("Vertical");

        Vector3 moveVector = new Vector3(hor, vt).normalized;
        if (moveVector != Vector3.zero) GetComponent<Animator>().SetBool("isWalking",true);
        else GetComponent<Animator>().SetBool("isWalking",false);;
        if (!onP) gameObject.GetComponent<Rigidbody2D>().AddForce(moveVector * (dashing ? dashSpeed : moveSpeed));
        // transform.position += moveVector * Time.deltaTime * (dashing ? dashSpeed : moveSpeed);
        dashPannel.transform.position = new Vector3(transform.position.x + 0.7f, transform.position.y + 0.7f);
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

    void Dash()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || !dashable) return;

        dashing = true;
        dashable = false;
        dashPannel.color = new Color(255, 255, 255, 0.6f);
        dashPannel.GetComponent<Animator>().SetTrigger("reload");
        dashPannel.GetComponent<Animator>().speed = 1 / dashCooltime;

        Invoke("DashFinish", 0.2f);
        Invoke("DashCooldown", dashCooltime);

        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.6f);
        gameObject.layer = 6;
    }

    void DashFinish()
    {
        dashing = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
        gameObject.layer = 3;
    }

    void DashCooldown()
    {
        dashPannel.color = new Color(255, 255, 255, 0f);
        dashPannel.GetComponent<Animator>().speed = 100;
        dashable = true;
    }

    void ChangeWeapon()
    {
        if (Input.GetKey(KeyCode.F))
        {
            if (onW&&!onP){
                onP=true;
                Fpannel.SetActive(false);
                pnl.SetActive(true);
            }else if(onG){
                Destroy(W);
                bombremain++;
            }
            
            
        }else if(Input.GetKey(KeyCode.V)){
            if (onP){
                onP=false;
                Fpannel.SetActive(true);
                pnl.SetActive(false);
                W.name=weapon.Wn;
                W.GetComponent<SpriteRenderer>().sprite=gunTexture.ElementAt<Sprite>(weapon.gun_sprite);
                weapon.gun_sprite=gun_sprite;
                weapon.fireSet=fireSet;
                weapon.Wn = Wn;
                weapon.maxAmmo = maxAmmo;
                weapon.ammo = ammo;
                weapon.shootingCooltime = shootingCooltime;
                weapon.reloadCooltime = reloadCooltime;
                weapon.damage = damage;
            }
        }else if(Input.GetKey(KeyCode.X)){
            if (onP){
                onP=false;
                Fpannel.SetActive(true);
                pnl.SetActive(false);
            }
        }
        
    }

    void ThrowBomb()
    {
        if (Input.GetKey(KeyCode.G) && bombremain != 0)
        {
            GameObject newBomb = Instantiate(bombOb, transform.position, gun.transform.rotation * Quaternion.Euler(0, 0, 90));
            newBomb.GetComponent<Rigidbody2D>().AddForce(newBomb.transform.up * -5, ForceMode2D.Impulse);
            bombremain--;
        }
    }

    public void GetDamage(int damage)
    {
        Debug.Log("작동");
        if (damaged) return;
        
        damaged = true;
        hp -= damage;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.6f);
        gameObject.layer = 6;

        if (hp <= 0)
        {
            Time.timeScale = 0f;
            CancelInvoke();
            Destroy(gameObject);
            Destroy(gun.gameObject);
        }

        Invoke("DamageCooldown", 0.5f);
    }

    void DamageCooldown()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
        gameObject.layer = 3;
        damaged = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "damageObject")
        {
            // GetDamage(collision.gameObject.GetComponent<Dobject>().damage);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!onP)
            W = collision.gameObject;

        if (collision.gameObject.tag == "gun")
        {
            if(!onP){
            if (collision.gameObject.name == "Glock")
                SetWeapon(15, 15, 1, 0.5f, 2.5f,"Glock",0,new Vector2(0,0.1f));
            else if (collision.gameObject.name == "P1911")
                SetWeapon(10, 10, 1, 0.5f, 2.5f,"P1911",1,new Vector2(0,0.1f));
            else if (collision.gameObject.name == "Desert Eagle")
                SetWeapon(7, 7, 3, 0.6f, 2.5f,"Desert Eagle",2,new Vector2(0,0.1f));
            else if (collision.gameObject.name == "AK47")
                SetWeapon(30, 30, 2, 0.2f, 2f,"AK47",3,new Vector2(0.3f ,0.1f));
            else if (collision.gameObject.name == "AUG")
                SetWeapon(30, 30, 2, 0.2f, 2f,"AUG",4,new Vector2(0.3f,0.085f));
            else if (collision.gameObject.name == "K2")
                SetWeapon(30, 30, 2, 0.2f, 2f,"K2",5,new Vector2(0.3f,0.065f));
            else if (collision.gameObject.name == "M4A1")
                SetWeapon(30, 30, 2, 0.2f, 2f,"M4A1",6,new Vector2(0.3f,0.1f));
            else if (collision.gameObject.name == "HK416")
                SetWeapon(30, 30, 2, 0.2f, 2f,"HK416",7,new Vector2(0.3f,0.11f));
            else if (collision.gameObject.name == "Kar98K")
                SetWeapon(5, 5, 10, 1.5f, 3f,"Kar98K",8,new Vector2(0.5f,0.11f));
            else if (collision.gameObject.name == "M107")
                SetWeapon(7, 7, 15, 0.7f, 3f,"M107",9,new Vector2(0.55f,0.11f));
            else if (collision.gameObject.name == "AWM")
                SetWeapon(7, 7, 3, 0.9f, 3f,"AWM",10,new Vector2(0.52f,0.15f));
            else if (collision.gameObject.name == "MG3")
                SetWeapon(100, 100, 1, 0.075f, 5f,"MG3",11,new Vector2(0.49f,0.1f));
            else if (collision.gameObject.name == "M249")
                SetWeapon(100, 100, 1, 0.075f, 5f,"M249",12,new Vector2(0.315f,0.102f));
            else if (collision.gameObject.name == "RPK74")
                SetWeapon(75, 75, 2, 0.2f, 3f,"RPK74",13,new Vector2(0.5f,0.115f));
            }
        // maxAmmo_, ammo, damage, sc, rc, wn,gs
            Fpannel.SetActive(true);
            Fpannel.transform.position = new Vector2(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y + 1);
        }
        else if (collision.gameObject.tag == "tnfbxks")
        {
            onG = true;
            Fpannel.SetActive(true);
            Fpannel.transform.position = new Vector2(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y + 1);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision != null){
        if (collision.gameObject.tag == "gun")
        {
            onW = false;
            Fpannel.SetActive(false);
        }
        if (collision.gameObject.tag == "tnfbxks")
        {
            onG = false;
            Fpannel.SetActive(false);
        }
        }
    }

    void SetWeapon(int maxAmmo_, int ammo_, int damage_, float shootingCooltime_, float reloadCooltime_, string Wn_,int gun_sprite_,Vector2 fireSet_)
    {
        fireSet=fireSet_;
        gun_sprite=gun_sprite_;
        Wn=Wn_;
        maxAmmo = maxAmmo_;
        ammo = ammo_;
        damage = damage_;
        shootingCooltime = shootingCooltime_;
        reloadCooltime = reloadCooltime_;
        onW = true;
    }
}
