using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCamera : MonoBehaviour
{

    private Vector3 targetPos = new Vector3(0, 17.5f, -29.5f);

    // Start is called before the first frame update
    void Start()
    {
        transform.position = targetPos + Vector3.up * 500;
        StartCoroutine(StartAnim());
    }

    IEnumerator StartAnim()
    {
        for (int i = 0; i < 50; i++)
        {
            transform.position -= Vector3.up * 10;
            yield return null;
        }
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
