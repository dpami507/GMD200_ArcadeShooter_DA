using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Logic")]
    public int maxHealth;
    public int currentHealth;
    float displayedPercent;

    [Header("Canvas Stuff")]
    [SerializeField] Transform canvas;
    [SerializeField] Image healthBar;
    [SerializeField] float healthBarSpeed;

    [Header("Death")]
    [SerializeField] ParticleSystem explosion;

    [Header("Just Player")]
    public bool isPlayer;
    Transform healthBarTrans;
    [SerializeField] float healthBarTransSpeed;
    [SerializeField] float rotAmount;
    float scale;

    void Start()
    {
        //Set bar
        currentHealth = maxHealth;
        displayedPercent = currentHealth;

        //isPlayer?
        isPlayer = GetComponent<PlayerManager>();

        //SetHealthBar
        if (healthBar != null)
            healthBarTrans = healthBar.transform.parent;
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

            FindFirstObjectByType<CameraFollowScript>().Shake((float)value / 10);
        }
    }

    public void Die(Color color, string sound)
    {
        FindFirstObjectByType<SoundManager>().PlaySound(sound);

        ParticleSystem explosion_ = Instantiate(explosion, transform.position, transform.rotation);
        explosion_.transform.localScale = transform.localScale;

        ParticleSystem.MainModule main = explosion_.main;
        main.startColor = color;

        Destroy(explosion_, 2f);
        Destroy(this.gameObject);
    }
}
