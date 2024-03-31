using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1);
    }

    public void Setup(Vector3 pos)
    {
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
