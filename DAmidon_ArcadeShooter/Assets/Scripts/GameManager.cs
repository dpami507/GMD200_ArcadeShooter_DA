using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreTxt;
    public int score;
    public bool dead;
    public bool gameOver;
    public bool gameStarted;

    public GameObject startScreen;
    public GameObject H2PScreen;

    public GameObject deathScreen;
    public TMP_Text deathScore;
    public string[] deathMsgs;
    public TMP_Text deathMsg;

    SoundManager soundManager;

    private void Start()
    {
        gameOver = false;
        dead = false;
        UpdateScore(0);

        startScreen.SetActive(true);
        deathScreen.SetActive(false);
        H2PScreen.SetActive(false);

        soundManager = FindFirstObjectByType<SoundManager>();
    }

    private void Update()
    {
        //If player is dead set gameOver and do the stuff
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
        soundManager.PlaySound("Select");
        startScreen.SetActive(false);
        gameStarted = true;
    }

    public void Restart()
    {
        soundManager.PlaySound("Select");
        SceneManager.LoadScene("Game");
    }

    public void ShowHowToPlay()
    {
        soundManager.PlaySound("Select");
        H2PScreen.SetActive(!H2PScreen.activeInHierarchy);
    }

    public void Quit()
    {
        soundManager.PlaySound("Select");
        Application.Quit();
    }

    public void UpdateScore(int value)
    {
        //Add value then update text
        score += value;
        scoreTxt.text = "SCORE: " + score.ToString();
    }
}
