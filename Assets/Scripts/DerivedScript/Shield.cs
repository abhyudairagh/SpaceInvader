using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour,IShield
{

    [SerializeField]
    private Image lifeBar;
    private const int totalLife = 100;
    private int life;

    public int Life
    {
        get
        {
            return life;
        }
    }

    private void Start()
    {
        life = totalLife;
    }

    public void Damage(int damage)
    {
        life -= damage;
        lifeBar.fillAmount = (float)life /totalLife ;
        if (life < 1)
        {
            gameObject.SetActive(false);
        }
    }
    public void ResetShield()
    {
        life = totalLife;
        lifeBar.fillAmount = 1f;
        gameObject.SetActive(true);
    }
}

public interface IShield
{
     int Life { get; }
    void Damage(int damage);

    void ResetShield();
}