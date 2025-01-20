using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunScript : MonoBehaviour
{
    [SerializeField] Transform target;

    [Header("Firing")]
    public GameObject rocket;
    public float rotSpeed;
    public float firingAngle;
    public Transform muzzle;
    public float chargeTime;
    public float shotsPerSecond;
    float lastShot;
    public ParticleSystem chargePart;
    public ParticleSystem firePart;
    bool firing;

    [Header("Misc")]
    Health health;
    public int scoreWorth;
    public ParticleSystem explosion;
    public Color color;

    private void Start()
    {
        chargePart.Stop();
        firePart.Stop();
        firing = false;
        target = FindObjectOfType<PlayerManager>().transform;
        lastShot = 1 / shotsPerSecond;
        health = GetComponent<Health>();
    }

    private void Update()
    {
        //Kill
        if (health.currentHealth <= 0)
            Die();

        //Rotate to Target
        transform.rotation = Quaternion.Lerp(transform.rotation, GetRotation(), rotSpeed * Time.deltaTime);

        //Shooting Logic
        lastShot += Time.deltaTime;
        if (CheckIfCanFire() && lastShot > (1 / shotsPerSecond) && !firing)
        {
            lastShot = 0;
            StartCoroutine(StartFire());
        }
    }

    IEnumerator StartFire()
    {
        //Play Charge for a bit then fire
        firing = true;
        chargePart.Play();
        yield return new WaitForSeconds(chargeTime);
        chargePart.Stop();
        Fire();
    }

    void Fire()
    {
        //Fire Rocket
        firePart.Play();
        FindFirstObjectByType<SoundManager>().PlaySound("Shoot");
        Instantiate(rocket, muzzle.position, muzzle.rotation);
        firing = false;
    }

    bool CheckIfCanFire()
    {
        //See if angle is within fire range
        float zRot = transform.rotation.eulerAngles.z;
        float desZRot = GetRotation().eulerAngles.z;

        if (zRot > desZRot - firingAngle && zRot < desZRot + firingAngle)
            return true;
        else return false;
    }

    Quaternion GetRotation()
    {
        //Trig!
        float x = target.position.x - transform.position.x;
        float y = target.position.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        Quaternion desiredRot = Quaternion.Euler(0, 0, angle - 180);
        return desiredRot;
    }

    void Die()
    {
        //Play Sound
        FindFirstObjectByType<SoundManager>().PlaySound("Explosion");

        //Make sure isnt dead
        if (!FindFirstObjectByType<GameManager>().dead)
            FindFirstObjectByType<GameManager>().UpdateScore(scoreWorth);

        //Particle
        ParticleSystem explosion_ = Instantiate(explosion, transform.position, transform.rotation);
        explosion_.transform.localScale = transform.localScale;
        explosion_.startColor = color;
        Destroy(explosion_, 2f);
        Destroy(this.gameObject);
    }
}
