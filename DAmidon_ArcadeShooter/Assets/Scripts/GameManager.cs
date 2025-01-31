using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

//Deals with lots of variables and info multiple objects rely on like player states and menus
public class GameManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] TMP_Text scoreTxt;
    [SerializeField] int score;

    [Header("Boss")]
    [SerializeField] GameObject bossAsset;
    [SerializeField] GameObject bossWarn;
    [SerializeField] float neededBossScore;
    float currentBossScore;

    [Header("Game State Bools")]
    public bool dead;
    public bool gameOver;
    public bool gameStarted;

    [Header("Canvas UI Stuff")]
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject H2PScreen;

    [SerializeField] GameObject deathScreen;
    [SerializeField] TMP_Text deathScore;
    [SerializeField] string[] deathMsgs;
    [SerializeField] TMP_Text deathMsgTxt;

    Spawner spawner;
    SoundManager soundManager;

    private void Start()
    {
        gameOver = false;
        dead = false;
        UpdateScore(0); //Set score text

        startScreen.SetActive(true);
        deathScreen.SetActive(false);
        H2PScreen.SetActive(false);

        soundManager = FindFirstObjectByType<SoundManager>();
        spawner = FindFirstObjectByType<Spawner>();
        currentBossScore = neededBossScore;
    }

    private void Update()
    {
        //If player is dead set gameOver and do the stuff
        if (dead && !gameOver)
        {
            gameOver = true;
            deathScreen.SetActive(true);
            deathMsgTxt.text = deathMsgs[Random.Range(0, deathMsgs.Length)];
            deathScore.text = "Score: " + score.ToString();
        }

        //Boss Spawning
        bossWarn.SetActive(FindFirstObjectByType<BossScript>());
        if(score > currentBossScore)
        {
            currentBossScore += neededBossScore;
            Instantiate(bossAsset, spawner.GetSpawnPos(), Quaternion.identity);
            FindFirstObjectByType<CameraFollowScript>().Shake(1);
            soundManager.PlaySound("Explosion");
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