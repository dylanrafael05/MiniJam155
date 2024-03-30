using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TornadoForce : MonoBehaviour
{
    public float strengthContribution = 1;
    public string itemName = "No Name";

    [Header("Force Constants")]
    private float strengthToForceRatio;
    private float maxPullDistance;
    private float maxPullForce;
    private float perpendicularForceMultiplier;
    private float distanceFalloffPower;
    private float perpendicularDistanceFalloffPower;
    private float groundCheckTime;

    [Header("Tornado")]
    private Tornado tornado;

    [Header("Componenets")]
    private Rigidbody rb;
    private SpringJoint spring;
    
    private bool grounded;
    float timeHitGround;
    TrackedCondition lifted;
    Transform originalParent;

    public bool Grounded => grounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalParent = transform.parent;

        tornado = FindAnyObjectByType<Tornado>();
        strengthToForceRatio = tornado.strengthToForceRatio;
        maxPullDistance = tornado.maxPullDistance;
        maxPullForce = tornado.maxPullForce;
        perpendicularForceMultiplier = tornado.perpendicularForceMultiplier;
        distanceFalloffPower = tornado.distanceFalloffPower;
        perpendicularDistanceFalloffPower = tornado.distanceFalloffPower;
        groundCheckTime = tornado.groundCheckTime;

        timeHitGround = Time.time - groundCheckTime;

        lifted.Value = false;
    }

    private void FixedUpdate()
    {
        Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 tornadoPosition2D = new Vector2(tornado.transform.position.x, tornado.transform.position.z);
        if (Vector2.Distance(position2D, tornadoPosition2D) <= maxPullDistance * tornado.strength) 
        {
            float distance = Vector3.Distance(transform.position, tornado.transform.position);
            Vector3 direction = tornado.transform.position - transform.position;
            direction.y += 3 * tornado.strength;
            direction = direction.normalized;
            float magnitude = (tornado.strength * strengthToForceRatio) / Mathf.Pow(distance, distanceFalloffPower);
            if (magnitude > maxPullForce) magnitude = maxPullForce;
            rb.AddForce(direction * magnitude);
            // print(direction * magnitude);

            Vector2 perpendicularDirection2D = Vector2.Perpendicular((tornadoPosition2D - position2D).normalized);
            Vector3 perpendicularDirection = new Vector3(perpendicularDirection2D.x, 0, perpendicularDirection2D.y);
            perpendicularDirection *= -1;
            float perpendicularMagnitude = ((tornado.strength * strengthToForceRatio) / Mathf.Pow(distance, perpendicularDistanceFalloffPower)) * perpendicularForceMultiplier;
            if (perpendicularMagnitude > maxPullForce) perpendicularMagnitude = maxPullForce;
            rb.AddForce(perpendicularDirection * perpendicularMagnitude);
        }

        // Handle lifting
        var delta = transform.position - tornado.transform.position;
        delta -= Vector3.up * delta.y;

        lifted.Value = (timeHitGround > Time.time - groundCheckTime || !grounded) 
                    && delta.magnitude < tornado.influenceRadius * tornado.strength;

        if(lifted.Rising)
        {
            tornado.ObjectContributions += strengthContribution;
            transform.SetParent(tornado.transform);

            spring = gameObject.AddComponent<SpringJoint>();
            spring.connectedBody = tornado.Rigidbody;
            spring.connectedMassScale = 0;
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedAnchor = -Vector3.forward * (tornado.strength * 10 + 5);
            spring.minDistance = 0;
            spring.maxDistance = 4;
            spring.damper = 0.5f;
        }
        else if(lifted.Falling)
        {
            tornado.ObjectContributions -= strengthContribution;
            transform.SetParent(originalParent);
            Destroy(spring);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.Ground)
        {
            timeHitGround = Time.time;
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == Layers.Ground)
        {
            grounded = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
