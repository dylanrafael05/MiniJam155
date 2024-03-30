using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkCycle : MonoBehaviour
{

    private const int iMax = 120;

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
        while (true)
        {   
            for (int i = 0; i < iMax; i++)
            {
                transform.localEulerAngles = new Vector3(0, Mathf.Lerp(-45, 45, (float)i / iMax), 0);
                if (i < iMax / 2) transform.localPosition = new Vector3(0, 0, Mathf.Lerp(0, 1f, (float)i / (iMax / 2)));
                else transform.localPosition = new Vector3(0, 0, Mathf.Lerp(1f, 0, (float)(i - iMax / 2) / (iMax / 2)));
                yield return null;
            }
            for (int i = 0; i < iMax; i++)
            {
                transform.localEulerAngles = new Vector3(0, Mathf.Lerp(45, -45, (float)i / iMax), 0);
                if (i < iMax / 2) transform.localPosition = new Vector3(0, 0, Mathf.Lerp(0, 1f, (float)i / (iMax / 2)));
                else transform.localPosition = new Vector3(0, 0, Mathf.Lerp(1f, 0, (float)(i - iMax / 2) / (iMax / 2)));
                yield return null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
