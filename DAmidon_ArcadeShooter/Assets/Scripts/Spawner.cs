using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spawns enemies and calculates weights and chance for certain enemeis
public class Spawner : MonoBehaviour
{
    [SerializeField] Enemy[] enemies;
    [SerializeField] int spawnedEnemies;
    [SerializeField] int maxSpawned;
    [SerializeField] int spawned;

    float totalWeight;

    [SerializeField] Transform playerCam;

    [SerializeField] float waveTimer;
    float lastSpawned;

    private void Start()
    {
        CalculateTotalWeight(); //Calculate total weight of all objects
    }

    private void Update()
    {
        if(!GameManager.instance.gameStarted) { return; }

        spawned = FindObjectsOfType<EnemyBaseScript>().Length;

        //Spawn logic
        lastSpawned += Time.deltaTime;
        if(lastSpawned >= waveTimer && !GameManager.instance.dead)
        {
            lastSpawned = 0;
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        for(int i = 0; i < spawnedEnemies; i++)
        {
            //Dont spawn if there are more than 7 enemies
            if (spawned <= maxSpawned)
            {
                Vector2 spawnPos = GetSpawnPos();

                //Spawn Chosen Enemy
                Instantiate(RandomEnemy(), spawnPos, Quaternion.identity);
            }
        }
    }

    public Vector2 GetSpawnPos()
    {
        //Get cam bounds and add offset as to hide enemy spawning
        Camera cam = playerCam.GetComponent<Camera>();
        float camWidth = cam.orthographicSize * ((float)16 / 9) + 4;
        float camHeight = cam.orthographicSize + 4;

        //Get random pos on either left or right of screen
        float spawnX = (Random.Range(-camWidth, camWidth) > 0) ? camWidth : -camWidth;
        float spawnY = Random.Range(-camHeight, camHeight);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0) + playerCam.transform.position;

        //Return pos
        return spawnPos;
    }

    //ChatGPT was a real one for givig me this
    void CalculateTotalWeight()
    {
        totalWeight = 0;
        foreach (Enemy enemy in enemies)
        {
            totalWeight += enemy.spawnChance;
        }
    }

    //ChatGPT was a real one for givig me this
    GameObject RandomEnemy()
    {
        float rand = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        foreach (Enemy enemy in enemies)
        {
            cumulativeWeight += enemy.spawnChance;
            if (rand <= cumulativeWeight)
            {
                return enemy.enemyObj;
            }
        }

        Debug.LogWarning("Ya, set it up wrong bucko :/");
        return null;
    }
}

[System.Serializable]
public class Enemy 
{
    public GameObject enemyObj;

    [Range(0, 100)]
    public int spawnChance;
}
