using UnityEngine;

public class TornadoRing : MonoBehaviour
{
    [Header("Visuals")]
    public float minRotation;
    public float maxRotation;

    float rotation;

    public const string ColorPropName = "_Color";

    public void Start()
    {
        rotation = Random.Range(minRotation, maxRotation);
    }

    private void Update()
    {
        transform.eulerAngles += Vector3.forward * rotation * Time.deltaTime;
    }
}
