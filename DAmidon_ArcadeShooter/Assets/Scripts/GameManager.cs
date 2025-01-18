using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreTxt;
    public int score;
    public bool dead;
    public bool gameOver;
    public bool gameStarted;

    public GameObject startScreen;

    public GameObject deathScreen;
    public TMP_Text deathScore;
    public string[] deathMsgs;
    public TMP_Text deathMsg;

    private void Start()
    {
        gameOver = false;
        dead = false;
        UpdateScore(0);

        startScreen.SetActive(true);
        deathScreen.SetActive(false);
    }

    private void Update()
    {
        if(dead && !gameOver)
        {
            gameOver = true;
            deathScreen.SetActive(true);
            deathMsg.text = deathMsgs[Random.Range(0, deathMsgs.Length)];
            deathScore.text = "Score: " + score.ToString();
        }
    }

    public void StartGame()
    {
        startScreen.SetActive(false);
        gameStarted = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateScore(int value)
    {
        score += value;
        scoreTxt.text = score.ToString();
    }
}
