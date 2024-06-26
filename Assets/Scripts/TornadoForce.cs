using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TornadoForce : MonoBehaviour
{
    public float strengthContribution = 1;
    public float strengthUnlock = 1;
    public string itemName = "No Name";

    [Header("Force Constants")]
    private float strengthToForceRatio;
    private float maxPullDistance;
    private float maxPullForce;
    private float perpendicularForceMultiplier;
    private float distanceFalloffPower;
    private float perpendicularDistanceFalloffPower;
    private float groundCheckTime;
    private float timeBeforeExplosion;

    [Header("Tornado")]
    private Tornado tornado;
    private Explosion explosion;

    [Header("Componenets")]
    private Rigidbody rb;
    private SpringJoint spring;
    
    private bool grounded;
    private bool settled = false;
    float timeHitGround;
    TrackedCondition lifted;
    Transform originalParent;

    public TrackedCondition Lifted => lifted;

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
        timeBeforeExplosion = tornado.timeBeforeExplosion;
        explosion = tornado.explosionPrefab;

        timeHitGround = Time.time - groundCheckTime * 2;

        lifted.Value = false;

        spring = gameObject.GetComponent<SpringJoint>();
    }

    void Update()
    {
        // Handle physics; this is in update instead of fixed update to reduce lag.
        var maxPull = maxPullDistance * tornado.EffectiveStrength;

        if (math.distance(tornado.transform.position.x, transform.position.x) > maxPull
         || math.distance(tornado.transform.position.z, transform.position.z) > maxPull)
        {
            return;
        }

        if (tornado.strength > strengthUnlock)
        {
            Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
            Vector2 tornadoPosition2D = new Vector2(tornado.transform.position.x, tornado.transform.position.z);

            if (Vector2.Distance(position2D, tornadoPosition2D) <= maxPull)
            {
                float distance = Vector3.Distance(transform.position, tornado.transform.position);
                Vector3 direction = tornado.transform.position - transform.position;
                direction.y += 3 * tornado.EffectiveStrength;
                direction = direction.normalized;
                float magnitude = (tornado.EffectiveStrength * strengthToForceRatio) / Mathf.Pow(distance, distanceFalloffPower);
                if (magnitude > maxPullForce) magnitude = maxPullForce;
                rb.AddForce(direction * magnitude * Time.deltaTime / Time.fixedDeltaTime);

                Vector2 perpendicularDirection2D = Vector2.Perpendicular((tornadoPosition2D - position2D).normalized);
                Vector3 perpendicularDirection = new Vector3(perpendicularDirection2D.x, 0, perpendicularDirection2D.y);
                perpendicularDirection *= -1;
                float perpendicularMagnitude = ((tornado.EffectiveStrength * strengthToForceRatio) / Mathf.Pow(distance, perpendicularDistanceFalloffPower)) * perpendicularForceMultiplier;
                if (perpendicularMagnitude > maxPullForce) perpendicularMagnitude = maxPullForce;
                rb.AddForce(perpendicularDirection * perpendicularMagnitude * Time.deltaTime / Time.fixedDeltaTime);
            }
        }

        // Handle lifting
        var delta = transform.position - tornado.transform.position;
        delta -= Vector3.up * delta.y;

        lifted.Value = settled 
                    && (timeHitGround > Time.time - groundCheckTime || !grounded)
                    && delta.magnitude < tornado.influenceRadius * tornado.EffectiveStrength
                    && tornado.strength > strengthUnlock;

        if (lifted.Rising)
        {
            tornado.ObjectContributions += strengthContribution;
            transform.SetParent(tornado.transform);

            spring.connectedBody = WorldManager.Instance.Rigidbody;
            spring.connectedMassScale = 0;
            spring.minDistance = 0;
            spring.maxDistance = 4;

            spring.spring = 3;
            StartCoroutine(Explode());
        }
        else if (lifted.Falling)
        {
            tornado.ObjectContributions -= strengthContribution;
            transform.SetParent(originalParent);

            spring.connectedBody = null;
            spring.spring = 0;
        }

        if (lifted.Value)
        {
            spring.connectedAnchor = tornado.transform.position + Vector3.up * (tornado.EffectiveStrength * 10);
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(timeBeforeExplosion);
        if (lifted.Value)
        {
            Instantiate(explosion).Setup(transform.position);
            tornado.ObjectContributions -= strengthContribution;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.Ground)
        {
            timeHitGround = Time.time;
            grounded = true;
            settled = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == Layers.Ground)
        {
            grounded = false;
        }
    }
}
