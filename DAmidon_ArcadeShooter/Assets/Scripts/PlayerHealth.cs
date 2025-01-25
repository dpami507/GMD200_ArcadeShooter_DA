using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Health script but with more stuff for the player like shaking and increase sliders
public class PlayerHealth : Health
{
    [Header("Just Player")]
    [SerializeField] Image currentHealthBar;
    Transform healthBarTrans;
    [SerializeField] float healthBarTransSpeed;
    [SerializeField] float rotAmount;
    float scale;

    private void Start()
    {
        //Set bar
        currentHealth = maxHealth;
        displayedPercent = currentHealth;

        //SetHealthBar
        if (healthBar != null)
            healthBarTrans = healthBar.transform.parent;
    }

    private void Update()
    {
        healthBarTrans.localScale = Vector3.Lerp(healthBarTrans.localScale, Vector3.one, healthBarTransSpeed * Time.deltaTime);
        healthBarTrans.rotation = Quaternion.Lerp(healthBarTrans.rotation, Quaternion.identity, healthBarTransSpeed * Time.deltaTime);

        //Update Bar
        healthBar.fillAmount = displayedPercent / maxHealth;
        currentHealthBar.fillAmount = (float)currentHealth / maxHealth;

        //stop greater than max >:(
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        //Lerp for smoooooothness
        displayedPercent = Mathf.Lerp(displayedPercent, currentHealth, healthBarSpeed * Time.deltaTime);

        if (canvas == null || healthBar == null) { return; }

        //Set rotation so when enemies rotate the health bar does not
        canvas.rotation = Quaternion.Euler(0, 0, 0);
    }

    public override void TakeDamage(int value)
    {
        currentHealth -= value;

        scale *= 1.05f;
        scale = Mathf.Clamp(scale, 1, 1.25f);
        healthBarTrans.localScale = Vector3.one * scale;

        float angle = Random.Range(-rotAmount, rotAmount);
        healthBarTrans.rotation = Quaternion.Euler(0, 0, angle);

        FindFirstObjectByType<CameraFollowScript>().Shake((float)value / 10);
    }
}
