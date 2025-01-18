using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Environment : MonoBehaviour
{
    [Header("Starting Platform")]
    public int waitTime;
    public int platformHeight;
    public float lowerSpeed;
    public Transform platform;

    [Header("Lava")]
    public Transform lava;
    Transform player;
    Vector2 lavaStartPos;
    Vector3 endPos;

    [Header("Grapple Points")]
    public GameObject point;
    public int amount;
    public int height;
    public int width;

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
        if(!isStarted && manager.gameStarted)
        {
            Debug.Log("0");
            isStarted = true;
            StartCoroutine(LowerPlatform());
        }

        lava.position = new Vector2(lavaStartPos.x + player.position.x, lavaStartPos.y);

        if(manager.gameStarted)
        {
            Debug.Log("2");
            platform.position = Vector2.Lerp(platform.position, endPos, lowerSpeed * Time.deltaTime);
        }
    }

    public void SpawnPoints()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 randPos = new Vector2(Random.Range(-width, width), Random.Range(0, height));
            Instantiate(point, randPos, Quaternion.identity);
        }    
    }

    IEnumerator LowerPlatform()
    {
        yield return new WaitForSeconds(waitTime);
        endPos = new Vector2(platform.position.x, platform.position.y - platformHeight);
        Debug.Log("1");
    }
}
