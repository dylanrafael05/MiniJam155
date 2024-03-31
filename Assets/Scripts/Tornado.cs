using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    [Serializable]
    public struct StrengthBarLevel
    {
        public float maximumValue;
        public Color barColor;
    }

    [Header("Visuals")]
    public TornadoRing ringPrefab;
    public Explosion explosionPrefab;
    public int ringCount;
    public UIBar strengthBar;
    public StrengthBarLevel[] barLevels;

    [Header("Tornado Values")]
    public float strength;
    public float strengthLoss;
    public float groundCheckTime;
    public float timeBeforeExplosion;

    [Header("Force Constants")]
    public float strengthToForceRatio;
    public float maxPullDistance;
    public float maxPullForce;
    public float perpendicularForceMultiplier;
    public float distanceFalloffPower;
    public float perpendicularDistanceFalloffPower;
    public float influenceRadius;

    private float targetStrength;
    private Transform child;

    public Rigidbody Rigidbody { get; private set; }

    public float BaseStrength { get; private set; }
    public float PenaltyContributions { get; set; }
    public float ObjectContributions { get; set; }

    public float EffectiveStrength => Mathf.Log(1 + strength) / Mathf.Log(2);

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
        targetStrength = BaseStrength + ObjectContributions - PenaltyContributions;
        targetStrength = Mathf.Max(0.5f, targetStrength);

        strength = Mathf.Lerp(strength, targetStrength, 2 * Time.deltaTime);

        child.position = new(
            child.position.x,
            EffectiveStrength * 5,
            child.position.z
        );

        child.localScale = new(
            EffectiveStrength,
            EffectiveStrength,
            EffectiveStrength
        );

        PenaltyContributions += Time.deltaTime * strengthLoss;

        // Update strength UI
        for(int i = 0; i < barLevels.Length; i++)
        {
            var currentLevel = barLevels[i];
            if(currentLevel.maximumValue > strength)
            {
                strengthBar.FilledColor = currentLevel.barColor;
                strengthBar.UnfilledColor = currentLevel.barColor * 0.5f;

                var previousMax = i > 0 ? barLevels[i - 1].maximumValue : 0;

                strengthBar.Fill = Mathf.InverseLerp(
                    previousMax,
                    currentLevel.maximumValue,
                    strength
                );

                break;
            }
        }
    }
}
