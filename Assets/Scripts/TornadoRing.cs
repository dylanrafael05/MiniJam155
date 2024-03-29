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
        var rotationSign = Random.Range(0, 2) * 2 - 1;
        rotation = rotationSign * Random.Range(minRotation, maxRotation);
    }

    private void Update()
    {
        transform.eulerAngles += Vector3.forward * rotation * Time.deltaTime;
    }
}
