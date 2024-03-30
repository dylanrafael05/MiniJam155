using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private PlayerController player;

    [Header("Spawn Settings")]
    public float minWait;
    public float maxWait;
    public float minRadius;
    public float maxRadius;

    [Header("Spawn Prefabs")]
    public GameObject cow;
    public GameObject honse;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));

            var choices = new List<GameObject>
            { 
                cow, 
                honse 
            };

            var rad = Random.Range(minRadius, maxRadius);

            var dir = Random.Range(0, 360);
            var pos = player.transform.position + Quaternion.AngleAxis(dir, Vector3.up) * Vector3.forward * rad;

            var choice = choices[Random.Range(0, choices.Count)];

            GameObject.Instantiate(choice, pos, choice.transform.rotation, transform);
        }
    }
}
