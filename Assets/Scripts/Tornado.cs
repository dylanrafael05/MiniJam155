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

    // Start is called before the first frame update
    void Start()
    {

        strength = 1.0f;

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
}
