using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSpaceInvaders : BaseSpaceInvaders
{
    [Tooltip("Enable this to have a continous wave of enemies")]
    [SerializeField]
    bool endlessGame;

    [Tooltip("Total player's life")]
    [SerializeField]
    int playerTotalLife;

    [Tooltip("Speed of player ship movement")]
    [SerializeField]
    private float moveSpeed;

    [Tooltip("Total number of enemies to spawn in the game")]
    [SerializeField]
    private int noOfEnemies;

    [Tooltip("Interval duration of enemy's firing")]
    [SerializeField]
    private int firingInterval ;

    [Tooltip("Damage Inflicted on shields")]
    [SerializeField]
    private int shieldDamage;

    [SerializeField]
    Transform player;

    [Tooltip("Add the shield objects in the scene")]
    [SerializeField]
    private Shield[] Shields;

    float sideBounds;

    int totalScore = 0;
    int highScore = 0;

    public bool IsGameStarted { get; private set; } 


    IEnemyController _enemyController;
    IUIManager _uIManager;
    IPlayer _player;

    public Vector3 playerPosition
    {
        get
        {
            return player.position;
        }
    }

    public int NoOfEnemies
    {
        get
        {
            return noOfEnemies;
        }
    }
    private void Start()
    {
        Initialize();
        UpdateHighScore();
        _uIManager?.ResetUI(playerTotalLife, highScore);
    }
    void Initialize()
    {
        IsGameStarted = true;
        _player = player.GetComponent<IPlayer>();
        _player.Life = playerTotalLife;
        StartCoroutine(FireEnemyBullet());
    }

    public void SetEnemyController(IEnemyController enemyController)
    {
        _enemyController = enemyController;

    }
    public void SetUIManager(IUIManager uiManager)
    {
        _uIManager = uiManager;


    }

    /// <summary>
    /// To check Remaining enemies left in the game and update game status
    /// </summary>
    void CheckForRemainingEnemies()
    {
        _uIManager.UpdateScore(totalScore);
        if (_enemyController.RemainingEnemies < 1)
        {
            if (endlessGame)
            {
                _enemyController.EnemiesReset();
            }
            else
            {

                GameOver(true);
            }
        }
    }
    /// <summary>
    /// To check and identify if player's life is over
    /// </summary>
    void CheckForPlayerDeath()
    {
        
        if (_player.Life < 1)
        {
            GameOver(false);
        }
        _uIManager.UpdateLife(_player.Life);
    }

   

    /// <summary>
    /// Handles all the collider trigger events and identify the next execution
    /// </summary>
    /// <param name="object1"></param>
    /// <param name="object2"></param>
    public override void HandleHit(GameObject object1, GameObject object2)
     {
       if((object1.tag == "Bullet") && (object2.tag =="Player"))
        {
            HandlePlayerDamage(object1);
    
        }
        else if ((object1.tag == "Bullet") && (object2.tag == "Enemy"))
        {
            HandleEnemyDamage(object1, object2);

        }
        else if ((object1.tag == "Bullet") && (object2.tag == "Shield"))
        {
            HandleShieldDamage(object1,object2);
            
        }
        
        if ((object1.tag == "Bullet") && (object2.tag == "Bullet") ||
            (object1.tag == "Bullet") && (object2.tag == "Border"))
        {
            ResetBullet(object1);
        }
        
        if((object1.tag == "Player") && (object2.tag == "Border"))
        {
            if (sideBounds == 0f)
            {
                sideBounds = Mathf.Abs(player.position.x);
            }

        }
        else if ((object1.tag == "Enemy") && (object2.tag == "Border"))
        {
            if (sideBounds == 0)
            {
                sideBounds = Mathf.Abs(object1.transform.position.x);
            }
            ChangeEnemyMoveDirection(object1.transform);


        }
    }

    /// <summary>
    /// Update the enemies move direction
    /// </summary>
    /// <param name="enemyTransfrom"></param>
    void ChangeEnemyMoveDirection(Transform enemyTransfrom)
    {
        EnemyMoveDirection dir = EnemyMoveDirection.Left;
            if( enemyTransfrom.position.x < (-sideBounds + 20f) )
            {
                dir = EnemyMoveDirection.Right;
            }
            else if(enemyTransfrom.position.x > (sideBounds - 20f))
            {
                dir = EnemyMoveDirection.Left;
            }
       bool hasInvaded = _enemyController.ChangeMoveDirection(dir);
        if (hasInvaded)
        {
            GameOver(false);
        }
    }

    /// <summary>
    /// Reset the whole game
    /// </summary>
    public void ResetGame()
    {
        totalScore = 0;
        IsGameStarted = true;

        foreach (Shield shield in Shields)
        {
            shield.ResetShield();
        }

        _enemyController.EnemiesReset();
        _player.Life = playerTotalLife;
        BulletPool.Instance.ResetBullets();
        _uIManager.ResetUI(playerTotalLife,highScore);

        StopAllCoroutines();
        StartCoroutine(FireEnemyBullet());
    }

    private void ResetBullet(GameObject bullet)
    {
        bullet.SetActive(false);
    }
    /// <summary>
    /// Reduce shield life
    /// Destroy of less life than 1
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="shield"></param>
    private void HandleShieldDamage(GameObject bullet , GameObject shield)
    {
        ResetBullet(bullet);
        IShield shieldScript = shield.GetComponent<IShield>();
        shieldScript.Damage(shieldDamage);
       
    }

    /// <summary>
    /// Remove Enemy
    // Add player score
    // Check for remaining enemies
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="enemy"></param>
    private void HandleEnemyDamage(GameObject bullet , GameObject enemy)
    {
        Bullet script = bullet.GetComponent<Bullet>();
        if (script.Owner == Owner.Enemy)
            return;

        ResetBullet(bullet);
        IEnemy enemyScript = enemy.GetComponent<IEnemy>();
        _enemyController.RemoveEnemy(enemyScript);
        totalScore += 10;

        CheckForRemainingEnemies();

    }
    /// <summary>
    ///  // Reduce life
    /// check life less than 1
    /// game over if life is over
    /// </summary>
    /// <param name="bullet"></param>
    private void HandlePlayerDamage(GameObject bullet)
    {
        if (bullet.GetComponent<Bullet>().Owner == Owner.Player)
            return;

        ResetBullet(bullet);
        _player.Damage();
        CheckForPlayerDeath();
        // Reduce life
        // check life less than 1
        // game over if life is over
    }


   
    /// <summary>
    /// Update highscore and keep it persistant
    /// </summary>
    void UpdateHighScore()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
            if(totalScore > highScore)
            {
                highScore = totalScore;
                PlayerPrefs.SetInt("HighScore",totalScore);
            }
        }
        else
        {
            highScore = totalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }
  
    IEnumerator FireEnemyBullet()
    {
        while (IsGameStarted)
        {
            yield return new WaitForSeconds(firingInterval);
            _enemyController.EnemyFire();
        }
    }
    void GameOver(bool isWin)
    {
        IsGameStarted = false;
        UpdateHighScore();
        _uIManager.ShowGameOver(isWin);
    }


    /// <summary>
    /// Handles Input and movement of player
    /// </summary>
    private void Update()
    {
        if (!IsGameStarted)
            return;

        player.position += (Vector3.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime);

        if(sideBounds != 0)
        {
            Vector3 pos = player.position;
            pos.x = Mathf.Clamp(pos.x, -sideBounds, sideBounds);
            player.position = pos;

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BulletPool.Instance.FirePlayerBullet(player.position);
        }

    }

}
