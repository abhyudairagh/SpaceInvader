using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,IEnemy
{
    [SerializeField]
    BoxCollider m_Collider;

     bool hasInvaded = false;

    Vector3 initPos;

    public float InvadingRegion { get; set; }
    public Vector2 Size
    {
        get
        {
            return m_Collider.bounds.size;
        }
    }
    public bool HasInvaded
    {
        get
        {
            return hasInvaded;
        }
    }

    public Vector3 Position 
    {
        get 
        {
            return transform.position; 
        }
    }

    public bool IsDead { get; private set; }


    public void Kill()
    {
        IsDead = true;
        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        IsDead = false;
        hasInvaded = false;
        transform.position = initPos;
        gameObject.SetActive(true);
    }

    IEnumerator StepDown()
    {
        yield return null;
        Vector3 postion = transform.position;
        postion.y -= 1f;
        transform.position = postion;

        if (transform.position.y < InvadingRegion)
        {
            hasInvaded = true;
        }
    }

    public void StepDownPosition()
    {
        StartCoroutine(StepDown());

    }
    public void MoveTo(EnemyMoveDirection direction)
    {
        Vector3 postion = transform.position;

        if(direction == EnemyMoveDirection.Left)
        {
            postion += (Vector3.left * Size.x * 0.5f);
        }
        else
        {
            postion += ( Vector3.right * Size.x * 0.5f);
        }
        transform.position = postion;
    }

    void Start()
    {
        m_Collider = GetComponent<BoxCollider>();
        initPos = transform.position;
    }
}

public interface IEnemy
{
    float InvadingRegion { get; set; }

    Vector2 Size { get; }
    Vector3 Position { get; }
    bool IsDead { get; }
    bool HasInvaded { get; }
    void Kill();
    void Respawn();

    void MoveTo(EnemyMoveDirection direction);

    void StepDownPosition();
}