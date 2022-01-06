using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour,IUIManager
{
    [Tooltip("TMP text component to display score")]
    [SerializeField]
    TextMeshProUGUI scoreText;
    [Tooltip("TMP text component to display highScore")]
    [SerializeField]
    TextMeshProUGUI highScoreText;
    [Tooltip("TMP text component to display player's life")]
    [SerializeField]
    TextMeshProUGUI lifeText;

    [Tooltip("TMP text component to display Result ")]
    [SerializeField]
    TextMeshProUGUI gameOverText;

    [Tooltip("Reference for Gameover panel")]
    [SerializeField]
    GameObject gameOverScreen;

    public void ShowHighScore(int score)
    {
        highScoreText.text = score.ToString();
    }

    public void UpdateLife(int life)
    {
        lifeText.text = life.ToString();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
    public void ResetUI(int life,int highScore)
    {
        lifeText.text = life.ToString() ;
        scoreText.text = "0";
        highScoreText.text = highScore.ToString();
        gameOverScreen.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        DemoSpaceInvaders game = DemoSpaceInvaders.Instance as DemoSpaceInvaders;
        gameOverScreen.SetActive(false);
        game.SetUIManager(this);
    }
    public void ShowGameOver(bool win = false)
    {
        if (win)
        {
            gameOverText.text = "You Win";
        }
        else
        {
            gameOverText.text = "GameOver";
        }

        gameOverScreen.SetActive(true);

    }

}

public interface IUIManager
{
    void UpdateLife(int life);
    void UpdateScore(int score);
    void ShowHighScore(int score);
    void ResetUI(int life, int highScore);

    void ShowGameOver(bool win = false);

}