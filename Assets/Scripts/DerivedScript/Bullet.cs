using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Owner
{
    Player,
    Enemy
}

public class Bullet : MonoBehaviour
{
    public Vector3 Velocity { get; set; }
    public Owner Owner { get; set; }


    // Update is called once per frame
    void Update()
    {
        transform.position += (Velocity * Time.deltaTime);
    }
}
