using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public int Life { get; set; }

    public void Damage()
    {
        Life--;
    }

}

public interface IPlayer
{
    int Life { get; set; }

    void Damage();

}