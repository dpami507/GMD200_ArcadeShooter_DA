using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;

    [SerializeField] Transform playerCam;

    [SerializeField] int spawnedEnemies;
    [SerializeField] int spawned;

    [SerializeField] float waveTimer;
    float lastSpawned;

    GameManager manager;

    private void Start()
    {
        manager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if(!manager.gameStarted) { return; }

        spawned = FindObjectsOfType<EnemyScript>().Length + FindObjectsOfType<RailgunScript>().Length;

        //Spawn logic
        lastSpawned += Time.deltaTime;
        if(lastSpawned >= waveTimer && !manager.dead)
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
            if (spawned <= 7)
            {
                Vector2 spawnPos = GetSpawnPos();

                Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
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
}
