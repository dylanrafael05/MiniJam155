using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;

using SeededRandom = Unity.Mathematics.Random;

public class WorldManager : MonoBehaviour
{
    private PlayerController player;
    private SeededRandom random;
    private uint seed;

    [Header("Chunk Settings")]
    public float2 chunkSize;

    [Header("Towns")]
    public int minBuildings;
    public int maxBuildings;
    public float buildingSpacing;

    [Header("Others")]
    public int minObjects;
    public int maxObjects;

    [Header("Spawn Prefabs")]
    public GameObject ground;
    public GameObject[] animals;
    public GameObject[] trees;
    public GameObject[] buildings;

    public Dictionary<int2, GameObject> GeneratedChunks = new();
    int2 oldPlayerChunk;

    public IEnumerator GenerateChunk(int2 chunk)
    {
        // Setup container and random state
        var time = Time.realtimeSinceStartup;
        random = new(seed + (uint)chunk.x + (uint)chunk.y);

        var container = new GameObject($"chunk{chunk.x}.{chunk.y}");
        container.transform.SetParent(transform);

        // Mark chunk as generated
        GeneratedChunks[chunk] = container;

        var center = chunk * chunkSize;
        var center3d = math.float3(center.x, 0, center.y);

        void Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            var go = GameObject.Instantiate(prefab, pos, rot, container.transform);
        }

        bool ShouldBreak()
        {
            const float MAX_TIME = 1f / 1000;
            return time < Time.realtimeSinceStartup - MAX_TIME;
        }

        void UpdateBreak()
        {
            time = Time.realtimeSinceStartup;
        }

        Spawn(ground, center3d, ground.transform.rotation);

        // Generate town
        var buildingCount = random.NextInt(minBuildings, maxBuildings);
        var townRowSize = random.NextInt(2, minBuildings);
        var townColSize = buildingCount / townRowSize;

        var townSize = math.float2(townRowSize, townColSize) * buildingSpacing;

        var townPos = random.NextFloat2(center - chunkSize / 2 + townSize, center + chunkSize / 2 - townSize);

        for(int i = 0; i < buildings.Length; i++)
        {
            var x = i % townRowSize;
            var y = i / townRowSize;

            var buildingPos = townPos + math.float2(
                x * buildingSpacing - townSize.x / 2,
                y * buildingSpacing - townSize.y / 2
            );

            var idx = random.NextInt(buildings.Length);

            var randOffset = random.NextFloat2(-buildingSpacing / 4, buildingSpacing / 4);
            var randangle = Quaternion.AngleAxis(random.NextFloat(math.PI * 2), Vector3.up);

            buildingPos += randOffset;

            Spawn(
                buildings[idx], 
                new(buildingPos.x, 0, buildingPos.y), 
                buildings[idx].transform.rotation * randangle
            );
        }

        if(ShouldBreak())
        {
            yield return null;
            UpdateBreak();
        }

        // Generate trees and animals
        int objectTarget = random.NextInt(minObjects, maxObjects);
        int objectCount = 0;
        int attempts = 0;

        var noiseOffset = random.NextFloat2();

        while(objectCount < objectTarget && attempts++ < 100_000)
        {
            // Occasionally check if we need to break
            if (attempts % 20 == 0 && ShouldBreak())
            {
                yield return null;
                UpdateBreak();
            }

            var pos = random.NextFloat2(center - chunkSize / 2, center + chunkSize / 2);

            if (math.distance(pos, townPos) < math.cmax(townSize))
                continue;

            var chance = noise.cnoise(pos + noiseOffset);
            var roll = random.NextFloat();

            GameObject[] choices;

            if (chance < 0 && roll < math.abs(chance))
            {
                choices = trees;
            }
            else if (chance > 0 && roll < math.abs(chance))
            {
                choices = animals;
            }
            else continue;

            var idx = random.NextInt(choices.Length);
            var randangle = Quaternion.AngleAxis(random.NextFloat(math.PI * 2), Vector3.up);

            Spawn(
                choices[idx],
                new(pos.x, 0, pos.y),
                choices[idx].transform.rotation * randangle
            );

            objectCount++;
        }
    }

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        if (Application.isEditor)
        {
            SetSeed((uint)Environment.TickCount);
        }

        UpdateChunks();
    }

    private void Update()
    {
        UpdateChunks();
    }

    public void UpdateChunks()
    {
        // TODO: fix this being incorrect!
        var playerChunk = (int2)(((float3)player.transform.position).xz / chunkSize + 0.5f);

        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                // Enable or generate chunks within view
                var chunk = playerChunk + math.int2(i, j);

                if (!GeneratedChunks.ContainsKey(chunk))
                {
                    StartCoroutine(GenerateChunk(chunk));
                }
                else if (!GeneratedChunks[chunk].activeSelf)
                {
                    GeneratedChunks[chunk].SetActive(true);
                }

                // Disable chunks out of view
                var oldChunk = oldPlayerChunk + math.int2(i, j);

                if(math.cmax(math.abs(oldChunk - playerChunk)) > 1)
                {
                    GeneratedChunks[oldChunk].SetActive(false);
                }
            }
        }

        oldPlayerChunk = playerChunk;
    }

    public void SetSeed(uint seed)
    {
        this.seed = seed;
    }
}
