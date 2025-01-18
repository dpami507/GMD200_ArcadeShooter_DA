using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] bool gameStarted;

    private void Start()
    {
        gameStarted = false;
        lavaStartPos = lava.position;
        player = FindFirstObjectByType<PlayerMovement>().transform;
        endPos = new Vector2(platform.position.x, platform.position.y - platformHeight);

        StartCoroutine(LowerPlatform());
    }

    private void Update()
    {
        lava.position = new Vector2(lavaStartPos.x + player.position.x, lavaStartPos.y);

        if(gameStarted)
        {
            platform.position = Vector2.Lerp(platform.position, endPos, lowerSpeed * Time.deltaTime);
        }
    }

    IEnumerator LowerPlatform()
    {
        yield return new WaitForSeconds(waitTime);
        gameStarted = true;
    }
}
