using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoStartMenu : MonoBehaviour
{
    [Header("Visuals")]
    public TornadoRing ringPrefab;
    public int ringCount;

    [Header("Tornado Values")]
    public float strength;
    public float groundCheckTime;

    [Header("Force Constants")]
    public float strengthToForceRatio;
    public float maxPullDistance;
    public float maxPullForce;
    public float perpendicularForceMultiplier;
    public float distanceFalloffPower;
    public float perpendicularDistanceFalloffPower;
    public float influenceRadius;

    private Transform child;

    public Rigidbody Rigidbody { get; private set; }

    public float BaseStrength { get; private set; }
    public float PenaltyContributions { get; set; }
    public float ObjectContributions { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        BaseStrength = strength;
        child = transform.GetChild(0);
        Rigidbody = GetComponent<Rigidbody>();

        for (int i = 0; i < ringCount; i++)
        {
            var height = (i + 1) / (float)ringCount;

            var go = GameObject.Instantiate(
                ringPrefab,
                Vector3.up * (height * 10 - 5),
                ringPrefab.transform.rotation,
                child
            );

            go.transform.localScale *= height * 2;
        }
    }

    private void Update()
    {
        child.position = new(
            child.position.x,
            strength * 5,
            child.position.z
        );

        child.localScale = new(
            strength,
            strength,
            strength
        );
    }
}