using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBehavior : MonoBehaviour
{

    public bool scaredOfTornado;
    public float scareDistance;
    public float groundOffset;
    public float movementSpeed;
    public float rotationSpeed;
    public float turnSwitchChance;
    private int direction;
    private WalkCycle walkCycle;
    private Rigidbody parentRb;
    private TornadoForce tornadoForce;
    [SerializeField] private bool dead = false;
    private bool touchedGround = false;

    // Start is called before the first frame update
    void Start()
    {
        walkCycle = GetComponent<WalkCycle>();
        parentRb = GetComponentInParent<Rigidbody>();
        tornadoForce = GetComponentInParent<TornadoForce>();
        walkCycle.StartWalkCycle();
        parentRb.freezeRotation = true;
        direction = Random.Range(0, 2) == 1 ? 1 : -1;
    }

    private void FixedUpdate()
    {
        if (tornadoForce.Grounded) touchedGround = true;
        if (!tornadoForce.Grounded && touchedGround)
        {
            walkCycle.EndWalkCycle();
            dead = true;
            parentRb.freezeRotation = false;
        }
        else if (!dead)
        {
            if (Random.Range(0f, 1f) <= turnSwitchChance) direction *= -1;
            if (!walkCycle.Walking) walkCycle.StartWalkCycle();
            transform.parent.position += -transform.parent.up.normalized * movementSpeed;
            transform.parent.eulerAngles += direction * rotationSpeed * Vector3.forward;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
