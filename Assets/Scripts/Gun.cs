using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
        public Player player;
        public Text txt;
        public GameObject bullet;
        public Transform shotPoint;
        public Slider slider;
        public int maxAmmo = 30;
        public float shootingCooltime;
        public float reloadCooltime;
        bool allowShooting = true;
        public int ammo = 30;
        bool reloading=false;
        public int damage=1;

    // Start is called before the first frame update
    void Start()
    {
        slider.transform.Find("Fill Area").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        ammoSystem();
        Shoot();
    }
    void Move(){
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y);
    }
    void Rotate()    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float dx = mousePos.x - transform.position.x;
        float dy = mousePos.y - transform.position.y;

        float angle = Mathf.Atan2(dy, dx)*Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
        
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
    void Shoot()

    {
        if (allowShooting&&Input.GetMouseButton(0)&&!reloading&&ammo>0)
        {GameObject newBullet = Instantiate(bullet, shotPoint.position, transform.rotation*Quaternion.Euler(0,0,90));
            newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.up * -1400);
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
