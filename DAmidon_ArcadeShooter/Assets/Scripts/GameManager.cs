using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text scoreTxt;
    [SerializeField] int score;

    [SerializeField] GameObject bossAsset;
    [SerializeField] float neededBossScore;
    public bool bossSpawned;
    float currentBossScore;

    public bool dead;
    public bool gameOver;
    public bool gameStarted;

    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject H2PScreen;

    [SerializeField] GameObject deathScreen;
    [SerializeField] TMP_Text deathScore;
    [SerializeField] string[] deathMsgs;
    [SerializeField] TMP_Text deathMsg;

    Spawner spawner;
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
        spawner = FindFirstObjectByType<Spawner>();
        currentBossScore = neededBossScore;
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

        bossSpawned = FindFirstObjectByType<BossScript>();

        if(score > currentBossScore)
        {
            currentBossScore += neededBossScore;
            Instantiate(bossAsset, spawner.GetSpawnPos(), Quaternion.identity);
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
