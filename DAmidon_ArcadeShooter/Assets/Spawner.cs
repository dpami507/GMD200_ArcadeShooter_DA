using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemies;

    public Transform playerCam;

    public int spawnedEnemies;

    public float waveTimer;
    float lastSpawned;

    private void Update()
    {
        lastSpawned += Time.deltaTime;
        if(lastSpawned >= waveTimer)
        {
            lastSpawned = 0;
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        for(int i = 0; i < spawnedEnemies; i++)
        {
            Vector2 spawnPos = GetSpawnPos();

            Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }

        spawnedEnemies++;
    }

    Vector2 GetSpawnPos()
    {
        Camera cam = playerCam.GetComponent<Camera>();
        float camWidth = cam.orthographicSize * ((float)16 / 9) + 4;
        float camHeight = cam.orthographicSize + 4;
        Vector2 bounds = new Vector2(camWidth, camHeight);

        float spawnX = (Random.Range(-camWidth, camWidth) > 0) ? camWidth : -camWidth;
        float spawnY = Random.Range(-camHeight, camHeight);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0) + playerCam.transform.position;

        return spawnPos;
    }
}
