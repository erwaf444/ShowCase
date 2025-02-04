using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Transform bulletPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            var bullet = Instantiate(bulletPrefab, bulletPoint.position, bulletPoint.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = bulletPoint.up * bulletSpeed;
        }
    }
}
