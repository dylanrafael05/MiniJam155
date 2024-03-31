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
    private Tornado tornado;
    [SerializeField] private bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        walkCycle = GetComponent<WalkCycle>();
        parentRb = GetComponentInParent<Rigidbody>();
        tornadoForce = GetComponentInParent<TornadoForce>();
        tornado = FindAnyObjectByType<Tornado>();
        walkCycle.StartWalkCycle();
        parentRb.freezeRotation = true;
        direction = Random.Range(0, 2) == 1 ? 1 : -1;
    }

    private void FixedUpdate()
    {
        if (tornadoForce.Lifted.Value)
        {
            walkCycle.EndWalkCycle();
            dead = true;
            parentRb.freezeRotation = false;
        }
        else if (!dead)
        {
            if (!walkCycle.Walking) walkCycle.StartWalkCycle();
            Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
            Vector2 tornadoPosition2D = new Vector2(tornado.transform.position.x, tornado.transform.position.z);
            if (Vector2.Distance(position2D, tornadoPosition2D) <= scareDistance * tornado.strength)
            {
                Vector3 direction = transform.position - tornado.transform.position;
                direction.y = 0;
                transform.parent.rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.parent.eulerAngles = new Vector3(-90, transform.parent.eulerAngles.y, transform.parent.eulerAngles.z);
            }
            else
            {  
                transform.parent.eulerAngles += direction * rotationSpeed * Vector3.forward;
            }
            transform.parent.position += -transform.parent.up.normalized * movementSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
