using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
        
        public Player player;
        public Text txt;
        public GameObject bullet;
        public Transform shotPoint;
        public Slider slider;
        public string Wn="Glock";
        public int maxAmmo = 15;
        public float shootingCooltime;
        public float reloadCooltime;
        bool allowShooting = true;
        public int ammo = 30;
        bool reloading=false;
        public int damage=1;
        public Vector2 fireSet;
        public int gun_sprite=0;

    // Start is called before the first frame update
    void Start()
    {
        slider.transform.Find("Fill Area").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Move_Rotate();
        ammoSystem();
        Shoot();
    }
    void Move_Rotate()
{
    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    float dx = mousePos.x - player.transform.position.x;
    float dy = mousePos.y - player.transform.position.y;

    float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

    transform.rotation = Quaternion.Euler(0, 0, angle);

    // 총의 위치를 플레이어와 마우스 사이의 일정 거리만큼 이동시키기
    float distanceFromPlayer = 0.6f;  // 총이 플레이어에서 떨어진 거리 (조절 가능)
    Vector2 direction = (mousePos - (Vector2)player.transform.position).normalized;
    transform.position = (Vector2)player.transform.position + direction * distanceFromPlayer;
    if ((angle >= 90) || (angle <= -90))
    {
        // if (fireSet.x > 0) fireSet.x *= -1;
        transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = true;
    }
    else if ((angle < 90) || (angle > -90))
    {
        // if (fireSet.x <= 0) fireSet.x *= -1;
        transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = false;
    }
}


    void ammoSystem(){
        txt.text =ammo+" / "+maxAmmo;
        if(Input.GetKey(KeyCode.R)&&ammo!=maxAmmo&&!reloading){
            reloading=true;
            slider.transform.Find("Fill Area").gameObject.SetActive(true);
            slider.GetComponent<Animator>().speed=1/reloadCooltime;
            slider.GetComponent<Animator>().SetTrigger("reload");
            Invoke("Reload", reloadCooltime);

        }
    }
    Vector2 Rotate90Clockwise(Vector2 v)
    {
        // 시계 방향 90도 회전
        return new Vector2(v.y, -v.x);
    }

    Vector2 Rotate90CounterClockwise(Vector2 v)
    {
        // 반시계 방향 90도 회전
        return new Vector2(-v.y, v.x);
    }
    void Shoot()

    {
        if (allowShooting&&Input.GetMouseButton(0)&&!reloading&&ammo>0)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            Vector2 direction = (mousePos - (Vector2)player.transform.position).normalized;
            Debug.Log(direction);
            Vector2 sp = new Vector2(0,fireSet.y);//(Vector2)transform.GetChild(0).transform.position+direction*distanceFromPlayer+sp
            GameObject newBullet = Instantiate(bullet, transform.position+(Vector3)direction*fireSet.x+(Vector3)(direction.x>0?Rotate90CounterClockwise(direction):Rotate90Clockwise(direction))*fireSet.y, transform.rotation*Quaternion.Euler(0,0,90));
            newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.up * -2000);
            ammo--;
            allowShooting = false;
            Invoke("EnableShooting", shootingCooltime); }
    }
    void EnableShooting()
    {
        allowShooting=true;
    }
    void Reload(){
        slider.transform.Find("Fill Area").gameObject.SetActive(false);
        slider.GetComponent<Animator>().speed=100;

        ammo=maxAmmo;
        reloading=false;
    }
}
