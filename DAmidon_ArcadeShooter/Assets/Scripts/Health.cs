using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public Transform canvas;

    public Image healthBar;

    public void Start()
    {
        currentHealth = maxHealth;
        TakeDamage(0);
    }

    private void Update()
    {
        canvas.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void TakeDamage(int value)
    {
        currentHealth -= value;
        healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
    }
}
