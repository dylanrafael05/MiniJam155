using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkCycle : MonoBehaviour
{
    private const float CycleTime = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        StartWalkCycle();
    }

    public void StartWalkCycle()
    {
        StartCoroutine(DoWalkCycle());
    }

    public void EndWalkCycle()
    {
        StopCoroutine(DoWalkCycle());
    }

    IEnumerator DoWalkCycle()
    {
        var dir = 1;
        while (true)
        {
            var start = Time.time;
            while (Time.time - CycleTime < start)
            {
                var p = Mathf.InverseLerp(start, start + CycleTime, Time.time);

                transform.localEulerAngles = new Vector3(0, dir * Mathf.Lerp(-45, 45, 0.5f - Mathf.Cos(Mathf.PI * p) / 2), 0);

                if (p < 0.5f) transform.localPosition = Vector3.forward * Mathf.Lerp(0, 1f, 4 * p - 4 * p * p);
                else transform.localPosition = Vector3.forward * Mathf.Lerp(1f, 0, 4 * p * p - 4 * p + 1);

                yield return null;
            }

            dir *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
