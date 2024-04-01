using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Tornado))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public new Camera camera;

    [Header("Movement")]
    public float moveSpeed;
    public float maxMoveSpeed;
    public float moveDrag;

    [Header("Rotation")]
    public float rotationSpeed;
    public float rotationDrag;
    public float rotationDeadband = 0.1f;

    [Header("Zoom")]
    public float minZoom;
    public float maxZoom;
    public float zoomSpeed;
    public float zoomDrag;

    Rigidbody body;
    Tornado nado;

    Vector3 cameraOffset;
    Quaternion cameraRotation;
    float angle;

    float currentOmega;
    float currentZoomSpeed;

    public Rigidbody Rigidbody => body;

    private void Awake()
    {
        nado = GetComponent<Tornado>();
        body = GetComponent<Rigidbody>();

        cameraOffset = camera.transform.position - transform.position;
        cameraRotation = camera.transform.rotation;

        angle = 0;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update()
    {
        // Handle mouse movement
        var mouseDelta = Input.GetAxis("Mouse X");
        var newOmega = rotationSpeed * mouseDelta;

        // Change omega if
            // 1. the mouse is being scrolled
            // 2. the mouse scroll commands a movement of greater magnitude
            //    than the current movement, or in a different direction.

        if (Mathf.Abs(mouseDelta) > 0.1f * Time.deltaTime 
            && (Mathf.Abs(newOmega) > Mathf.Abs(currentOmega) 
            || Mathf.Sign(newOmega) != Mathf.Sign(currentOmega)))
        {
            currentOmega = newOmega;
        }
        else
        {
            currentOmega *= 1 - Time.deltaTime / rotationDrag;
        }

        angle += currentOmega * Time.deltaTime;

        // Handle zoom
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            currentZoomSpeed = -zoomSpeed * Input.mouseScrollDelta.y;
        }
        else
        {
            currentZoomSpeed *= 1 - Time.deltaTime / zoomDrag;
        }

        var originalZoom = cameraOffset.magnitude;

        var zoom = originalZoom;
        zoom += currentZoomSpeed * Time.deltaTime;

        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        cameraOffset /= originalZoom;
        cameraOffset *= zoom;

        // Handle WASD movement
        bool moving = false;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            var dir = Input.GetKey(KeyCode.S) ? -1 : 1;
            var axis = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;

            body.AddForce(dir * moveSpeed * nado.EffectiveStrength * axis);

            moving = true;
        }

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            var dir = Input.GetKey(KeyCode.A) ? -1 : 1;
            var axis = Quaternion.AngleAxis(angle + 90, Vector3.up) * Vector3.forward;

            body.AddForce(dir * moveSpeed * nado.EffectiveStrength * axis);

            moving = true;
        }

        if(!moving)
        {
            body.AddForce(-body.velocity * (1 - moveDrag));
        }

        // Clamp movement speed
        var currentSpeed = body.velocity.magnitude;
        if (currentSpeed > maxMoveSpeed * nado.EffectiveStrength)
        {
            body.velocity *= maxMoveSpeed * nado.EffectiveStrength / currentSpeed;
        }

        // Rotate camera
        var quat = Quaternion.AngleAxis(angle, Vector3.up);
        camera.transform.SetPositionAndRotation(
            transform.position + quat * cameraOffset * nado.EffectiveStrength, 
            quat * cameraRotation
        );
    }


}