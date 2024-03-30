using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    [Header("Visuals")]
    public TornadoRing ringPrefab;
    public int ringCount;

    [Header("Tornado Values")]
    public float strength;
    public float strengthLoss;
    public float groundCheckTime;

    [Header("Force Constants")]
    public float strengthToForceRatio;
    public float maxPullDistance;
    public float maxPullForce;
    public float perpendicularForceMultiplier;
    public float distanceFalloffPower;
    public float perpendicularDistanceFalloffPower;

    private float targetStrength;

    public float BaseStrength { get; private set; }
    public float PenaltyContributions { get; set; }
    public float ObjectContributions { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        BaseStrength = strength;

        for(int i = 0; i < ringCount; i++)
        {
            var height = (i + 1) / (float)ringCount;

            var go = GameObject.Instantiate(
                ringPrefab,
                Vector3.up * height * transform.localScale.y * 10,
                ringPrefab.transform.rotation,
                transform
            );

            go.transform.localScale *= height * 2;
        }
    }

    private void Update()
    {
        targetStrength = BaseStrength + ObjectContributions - PenaltyContributions;
        targetStrength = Mathf.Max(0.5f, targetStrength);

        strength = Mathf.Lerp(strength, targetStrength, 2 * Time.deltaTime);

        transform.localPosition = new(
            transform.localPosition.x,
            strength * 5,
            transform.localPosition.z
        );

        transform.localScale = new(
            strength,
            strength,
            strength
        );

        PenaltyContributions += Time.deltaTime * strengthLoss;
    }
}
