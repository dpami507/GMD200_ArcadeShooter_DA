using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base script for Enemies, includes common variables and functions for basic AI and other stuff
public class EnemyBaseScript : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target;
    public float rotSpeed;

    [Header("Gun")]
    public Transform muzzle;

    [Header("Score")]
    public int scoreWorth;
    public GameObject scoreTxt;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Health health;

    public bool CheckIfCanFire(float firingAngle)
    {
        //See if angle is within fire range
        float zRot = transform.rotation.eulerAngles.z;
        float desZRot = GetRotation(90).eulerAngles.z;

        if (zRot > desZRot - firingAngle && zRot < desZRot + firingAngle)
            return true;
        else return false;
    }

    public Quaternion GetRotation(float offSet)
    {
        //Trig!
        float x = target.position.x - transform.position.x;
        float y = target.position.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        Quaternion desiredRot = Quaternion.Euler(0, 0, angle - offSet);
        return desiredRot;
    }

    public void Die()
    {
        //Make sure isnt dead then add points
        if (!FindFirstObjectByType<GameManager>().dead)
            FindFirstObjectByType<GameManager>().UpdateScore(scoreWorth);

        GameObject scoreTxt_ = Instantiate(scoreTxt, transform.position, Quaternion.identity);
        scoreTxt_.GetComponentInChildren<TMPro.TMP_Text>().text = scoreWorth.ToString();
        Destroy(scoreTxt_, 1f);

        health.Die("Explosion");
    }
}
