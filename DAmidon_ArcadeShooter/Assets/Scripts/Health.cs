using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    float displayedPercent;

    public Transform canvas;

    public Image healthBar;

    [Header("Just Player")]
    public bool isPlayer;
    Transform healthBarTrans;
    public float healthBarSpeed;
    public float healthBarTransSpeed;
    public float rotAmount;
    float scale;

    public void Start()
    {
        //Set text
        currentHealth = maxHealth;
        displayedPercent = currentHealth;
        TakeDamage(0);

        //isPlayer?
        isPlayer = GetComponent<PlayerManager>();

        if (healthBar == null) { return; }

        //SetHealthBar
        healthBarTrans = healthBar.transform.parent;
    }

    private void Update()
    {
        //stop greater than max >:(
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        displayedPercent = Mathf.Lerp(displayedPercent, currentHealth, healthBarSpeed * Time.deltaTime);

        if (canvas == null || healthBar == null) { return; }

        //Set rotation so when enemies rotate the health bar does not
        canvas.rotation = Quaternion.Euler(0, 0, 0);

        //Lerp for smoooooothness
        if (isPlayer)
        {
            healthBarTrans.localScale = Vector3.Lerp(healthBarTrans.localScale, Vector3.one, healthBarTransSpeed * Time.deltaTime);
            healthBarTrans.rotation = Quaternion.Lerp(healthBarTrans.rotation, Quaternion.identity, healthBarTransSpeed * Time.deltaTime);
        }

        //Update Bar
        healthBar.fillAmount = displayedPercent / maxHealth;
    }

    //owchie
    public void TakeDamage(int value)
    {
        currentHealth -= value;

        if (isPlayer)
        {
            scale *= 1.05f;
            scale = Mathf.Clamp(scale, 1, 1.25f);
            healthBarTrans.localScale = Vector3.one * scale;

            float angle = Random.Range(-rotAmount, rotAmount);
            healthBarTrans.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
