using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

//Deals with lots of variables and info multiple objects rely on like player states and menus
public class GameManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] TMP_Text scoreTxt;
    [SerializeField] int score;

    [Header("Multiplier")]
    [SerializeField] TMP_Text scoreMultiplyerTxt;
    [SerializeField] float multiplierTxtLerp;
    [SerializeField] float scoreMultiplier;
    [SerializeField] float multiplierTime;
    float lastMultiplierTime;

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
    public static GameManager instance;

    private void Start()
    {
        instance = this;
        gameOver = false;
        dead = false;
        UpdateScore(0); //Set score text

        startScreen.SetActive(true);
        deathScreen.SetActive(false);
        H2PScreen.SetActive(false);

        spawner = FindFirstObjectByType<Spawner>();
        currentBossScore = neededBossScore;
    }

    private void Update()
    {
        //Turn on and off multiplier after a set time
        lastMultiplierTime += Time.deltaTime;
        if(lastMultiplierTime > multiplierTime)
            scoreMultiplier = 1;

        if (scoreMultiplier > 1)
            scoreMultiplyerTxt.gameObject.SetActive(true);
        else scoreMultiplyerTxt.gameObject.SetActive(false);

        scoreMultiplyerTxt.transform.localScale = Vector3.Lerp(scoreMultiplyerTxt.transform.localScale, Vector3.one, multiplierTxtLerp * Time.deltaTime);

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
            SoundManager.instance.PlaySound("Explosion");
        }
    }

    public void StartGame()
    {
        SoundManager.instance.PlaySound("Select");
        startScreen.SetActive(false);
        gameStarted = true;
    }

    public void Restart()
    {
        SoundManager.instance.PlaySound("Select");
        SceneManager.LoadScene("Game");
    }

    public void ShowHowToPlay()
    {
        SoundManager.instance.PlaySound("Select");
        H2PScreen.SetActive(!H2PScreen.activeInHierarchy);
    }

    public void Quit()
    {
        SoundManager.instance.PlaySound("Select");
        Application.Quit();
    }

    public float UpdateScore(int value)
    {
        //Add value then update text
        int addedScore = Mathf.RoundToInt(value * scoreMultiplier);
        score += addedScore;
        scoreTxt.text = "SCORE: " + score.ToString();

        lastMultiplierTime = 0;

        if (value > 0 && scoreMultiplier < 3)
        {
            UpdateMultiplier();
        }

        scoreMultiplyerTxt.text = "x" + scoreMultiplier.ToString();
        return addedScore;
    }

    void UpdateMultiplier()
    {
        scoreMultiplier += .1f;
        scoreMultiplier = RoundToTenth(scoreMultiplier);

        scoreMultiplyerTxt.transform.localScale = scoreMultiplyerTxt.transform.localScale * 1.5f;
    }

    float RoundToTenth(float value)
    {
        value = Mathf.Round(value * 10);
        return value / 10;
    }
}