using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreTxt;
    public int score;

    private void Start()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int value)
    {
        score += value;
        scoreTxt.text = score.ToString();
    }
}
