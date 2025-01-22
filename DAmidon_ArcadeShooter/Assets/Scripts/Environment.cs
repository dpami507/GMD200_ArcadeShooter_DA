using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    [Header("Starting Platform")]
    [SerializeField] int waitTime;
    [SerializeField] int platformHeight;
    [SerializeField] float lowerSpeed;
    [SerializeField] Transform platform;

    [Header("Lava")]
    [SerializeField] Transform lava;
    Transform player;
    Vector2 lavaStartPos;
    Vector3 endPos;

    [Header("Grapple Points")]
    [SerializeField] GameObject platformSpawn;
    [SerializeField] GameObject health;
    [SerializeField] GameObject point;
    [SerializeField] int amount;
    [SerializeField] int height;
    [SerializeField] int width;

    //Other
    GameManager manager;
    bool isStarted;

    private void Start()
    {
        isStarted = false;
        lavaStartPos = lava.position;
        player = FindFirstObjectByType<PlayerManager>().transform;
        manager = FindFirstObjectByType<GameManager>();
        endPos = platform.position;
        SpawnPoints();
    }

    private void Update()
    {
        //Start
        if(!isStarted && manager.gameStarted)
        {
            isStarted = true;
            StartCoroutine(LowerPlatform());
        }

        //Lava follow player X
        lava.position = new Vector2(lavaStartPos.x + player.position.x, lavaStartPos.y);

        //Lower Platform
        if(manager.gameStarted)
            platform.position = Vector2.Lerp(platform.position, endPos, lowerSpeed * Time.deltaTime);
    }

    public void SpawnPoints()
    {
        for (int i = 0; i < amount; i++)
        {
            float random = Random.Range(0, 100);

            if(random < 90)
            {
                //Spawn Point
                Vector2 randPos = new Vector2(Random.Range(-width, width), Random.Range(0, height));
                Instantiate(point, randPos, Quaternion.identity, this.transform);
            }
            else if(random < 98)
            {
                //Spawn Health
                Vector2 randPos = new Vector2(Random.Range(-width, width), Random.Range(0, height));
                Instantiate(health, randPos, Quaternion.identity, this.transform);
            }
            else
            {
                //Spawn Platform
                Vector2 randPos = new Vector2(Random.Range(-width, width), Random.Range(0, height));
                Instantiate(platformSpawn, randPos, Quaternion.identity, this.transform);
            }
        }    
    }

    IEnumerator LowerPlatform()
    {
        //Wait then set pos
        yield return new WaitForSeconds(waitTime);
        endPos = new Vector2(platform.position.x, platform.position.y - platformHeight);
    }
}
