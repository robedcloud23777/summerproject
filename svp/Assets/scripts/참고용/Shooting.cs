using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform shotPoint;
    public float shootingCooltime;
    bool allowShooting = true;

    public float reloadCooltime;
    public int reloadBulletCount;
    public int BulletCount;
    bool allowReloading = true;

    // Update is called once per frame
    void Update()
    {
        Shoot();
        Reload();
    }

    void Shoot()
    {
        if (allowShooting && Input.GetMouseButton(0) && BulletCount > 0)
        {
            GameObject newBullet = Instantiate(bullet, shotPoint.position, transform.rotation * Quaternion.Euler(0, 0, -90));
            newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.up * 200); // ����� �Ѿ� �ӵ�
            allowShooting = false;
            BulletCount--;
            Invoke("EnableShooting", shootingCooltime);
        }
    }

    void EnableShooting()
    {
        allowShooting = true;
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && BulletCount < reloadBulletCount && allowReloading)
        {
            allowShooting = false;
            allowReloading = false;
            Invoke("EnableReloading", reloadCooltime);
            Debug.Log("���� ��...");
        }
    }

    void EnableReloading()
    {
        allowShooting = true;
        allowReloading = true;
        BulletCount += reloadBulletCount - BulletCount;
        Debug.Log("������ �Ϸ�Ǿ����ϴ�!");
    }


}
