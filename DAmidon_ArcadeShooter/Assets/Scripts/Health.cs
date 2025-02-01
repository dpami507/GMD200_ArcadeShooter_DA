using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Deals with damage and helth for all entities
public class Health : MonoBehaviour
{
    [Header("Logic")]
    public int maxHealth;
    public int currentHealth;
    public float displayedPercent;

    [Header("Canvas Stuff")]
    public Transform canvas;
    public Image healthBar;
    public float healthBarSpeed;

    [Header("Death")]
    [SerializeField] ParticleSystem explosion;
    [SerializeField] Color color;

    void Start()
    {
        //Set bar
        currentHealth = maxHealth;
        displayedPercent = currentHealth;
    }

    void Update()
    {
        //stop greater than max >:(
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (canvas == null || healthBar == null) { return; }

        //Set rotation so when enemies rotate the health bar does not
        canvas.rotation = Quaternion.Euler(0, 0, 0);

        //Lerp for smoooooothness
        displayedPercent = Mathf.Lerp(displayedPercent, currentHealth, healthBarSpeed * Time.deltaTime);

        //Update Bar
        healthBar.fillAmount = displayedPercent / maxHealth;
    }

    //owchie
    public virtual void TakeDamage(int value)
    {
        currentHealth -= value;
    }

    public void Die(string sound)
    {
        SoundManager.instance.PlaySound(sound);

        ParticleSystem explosion_ = Instantiate(explosion, transform.position, transform.rotation);
        explosion_.transform.localScale = transform.localScale;

        ParticleSystem.MainModule main = explosion_.main;
        main.startColor = color;

        Destroy(explosion_.gameObject, 2f);
        Destroy(this.gameObject);
    }
}
