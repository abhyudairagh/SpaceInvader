using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoSingleton<BulletPool>
{
    [SerializeField]
    GameObject bulletPrefab;

    [Tooltip("Total no of bullets instantiated")]
    [SerializeField]
    private int noOfBulletsPerSession;

    [Tooltip("Speed of bullet")]
    [SerializeField]
    private float bulletSpeed;


    List<Bullet> bulletsPool = new List<Bullet>();
    Bullet playerBullet;


    void Start()
    {
        InitializeBulletPool();    
    }

    public void ResetBullets()
    {
        foreach (Bullet bullet in bulletsPool)
        {
            bullet.gameObject.SetActive(false);
            playerBullet.gameObject.SetActive(false);
        }
     }
    private void InitializeBulletPool()
    {
      
      for(var i = 0; i < noOfBulletsPerSession; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullet.transform.Rotate(Vector3.up , 180f);
            Bullet script = bullet.GetComponent<Bullet>();
            script.Owner = Owner.Enemy;
            bulletsPool.Add(script);
            
        }

        GameObject p_Bullet = Instantiate(bulletPrefab);
        p_Bullet.SetActive(false);
        playerBullet = p_Bullet.GetComponent<Bullet>();
        playerBullet.Owner = Owner.Player;
    }

    public void FireEnemyBullet(Vector3 enemyPosition)
    {
        foreach(Bullet bullet in bulletsPool)
        {
            if (bullet == null)
                continue;

            if (!bullet.gameObject.activeInHierarchy)
            {
                bullet.Velocity = Vector3.down * bulletSpeed;
                bullet.transform.position = enemyPosition;
                bullet.gameObject.SetActive(true);
                break;
            }
        }
    }
    public void FirePlayerBullet(Vector3 playerPos)
    {
        if (!playerBullet.gameObject.activeInHierarchy)
            {
                playerBullet.Velocity = Vector3.up * bulletSpeed;
                playerBullet.transform.position = playerPos;
                playerBullet.gameObject.SetActive(true);
            }
    }
}
