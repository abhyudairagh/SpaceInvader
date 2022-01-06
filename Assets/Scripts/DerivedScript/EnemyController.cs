using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum EnemyMoveDirection
{
    Left,
    Right
}

public class EnemyController : MonoBehaviour, IEnemyController
{
    const float delayConst = 1.5f;

    [Tooltip("Number of enemies that needs to be in a row")]
    [SerializeField]
    int enemiesPerRow;

    [Tooltip("Height offset from player's y position")]
    [SerializeField]
    float YPositionOffset;
    float XPositionOffset;

    [Tooltip("Distance from player to identify if enemy crossed the player boundary")]
    [SerializeField]
    float invadingRegionyOffset;

    

    [Tooltip("Horizontal to vertical spacing between each enemy")]
    [SerializeField]
    Vector2 SpacingOffset;

    [Tooltip("Size of enemy in world space")]
    [SerializeField]
    Vector2 enemySize;

    [SerializeField]
    GameObject enemyPrefab;

    List<IEnemy> currentEnemies = new List<IEnemy>();

    EnemyMoveDirection moveDirection = EnemyMoveDirection.Left;

    WaitForSeconds wait;
    float moveDelay;
    public int RemainingEnemies
    {
        get
        {
            return currentEnemies.FindAll(x => !x.IsDead).Count;
        }
    }

    void Start()
    {
        moveDelay = delayConst;
        wait = new WaitForSeconds(1f);
        Initialize();
        
        
    }
    /// <summary>
    /// Finds a random enemy to fire 
    /// </summary>
    public void EnemyFire()
    {
        List<IEnemy> enemies = currentEnemies.FindAll(x => !x.IsDead);
        int random = Random.Range(0, enemies.Count);
        if (enemies.Count > 0)
        {
            BulletPool.Instance.FireEnemyBullet(enemies[random].Position);
        }
    }

    public void EnemiesReset()
    {
        foreach (IEnemy enemy in currentEnemies)
        {
            enemy.Respawn();
        }
        moveDelay = delayConst;
        StopAllCoroutines();
        StartCoroutine(MoveEnemies()) ;
        UpdateMoveDelay();
    }
    /// <summary>
    /// Initialize all the enemies at the start
    /// </summary>
    public void Initialize()
    {
        DemoSpaceInvaders game = DemoSpaceInvaders.Instance as DemoSpaceInvaders;
        game.SetEnemyController(this);
        Vector3 playerPos = game.playerPosition;

        int enemyCount = game.NoOfEnemies;

        enemiesPerRow = enemyCount < enemiesPerRow ? enemyCount : enemiesPerRow;
        int enemiesPerColumn = (int)(enemyCount / (int)enemiesPerRow);


        float yPos = playerPos.y + YPositionOffset + (((enemiesPerColumn - 1) * enemySize.y) + ((enemiesPerColumn - 1) * SpacingOffset.y));


        XPositionOffset = playerPos.x - ((((enemiesPerRow - 1) * enemySize.x) + ((enemiesPerRow - 1) * SpacingOffset.x)) * 0.5f);

        float xPos = XPositionOffset;


        while (enemyCount > 0)
        {
            for (int i = 0; i < enemiesPerRow; i++)
            {
                GameObject gameObject = Instantiate(enemyPrefab, new Vector3(xPos, yPos, -1f), enemyPrefab.transform.rotation);
                xPos += (enemySize.x + SpacingOffset.x);
                enemyCount--;
                IEnemy enemyScript = gameObject.GetComponent<IEnemy>();
                enemyScript.InvadingRegion = playerPos.y + invadingRegionyOffset;
                currentEnemies.Add(enemyScript);
                if (enemyCount < 1)
                {
                    break;
                }
            }
            xPos = XPositionOffset;
            yPos -= (enemySize.y + SpacingOffset.y);
        }
        UpdateMoveDelay();
        StartCoroutine(MoveEnemies());
    }


    /// <summary>
    /// Removes a single enemy or mark the enemy as dead
    /// </summary>
    public void RemoveEnemy(IEnemy enemy)
    {
        if (currentEnemies.Contains(enemy))
        {
            enemy.Kill();

            UpdateMoveDelay();


        }

        
    }
    /// <summary>
    /// Method used to check and update the movement speed of the enemies
    /// </summary>
    void UpdateMoveDelay()
    {
        if (RemainingEnemies < 10)
        {
            moveDelay = delayConst * (RemainingEnemies / 10f);
        }

    }

    IEnumerator MoveEnemies()
    {
        yield return wait;
        DemoSpaceInvaders game = DemoSpaceInvaders.Instance as DemoSpaceInvaders;
        while (game.IsGameStarted)
        {
            yield return new WaitForSeconds(moveDelay);

            foreach(IEnemy enemy in currentEnemies)
            {
                if (!enemy.IsDead)
                {
                    enemy.MoveTo(moveDirection);
                }
            }
        }
    }
    
    /// <summary>
    /// Change the direction of movement for enemies
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <returns></returns>
    public bool ChangeMoveDirection(EnemyMoveDirection moveDirection)
    {
        if (this.moveDirection != moveDirection)
        {
            this.moveDirection = moveDirection;
        }
        foreach(IEnemy enemy in currentEnemies)
        {
            if (!enemy.IsDead)
            {
                enemy.StepDownPosition();

                if (enemy.HasInvaded)
                    return true;
            }
        }
        return false;
    }
}
public interface IEnemyController
{
    int RemainingEnemies { get; }
    void Initialize();

    void RemoveEnemy(IEnemy enemy);

    void EnemyFire();

    bool ChangeMoveDirection(EnemyMoveDirection moveDirection);
    void EnemiesReset();

}