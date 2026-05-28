using UnityEngine;

public class FishSwim : MonoBehaviour
{
    public float normalSpeed = 0.45f;
    public float burstSpeed = 1.2f;
    public float turnSpeed = 1.2f;
    public float burstTurnSpeed = 6f;
    public float burstDuration = 0.6f;

    [Header("Swim Area Around Starting Position")]
    public float areaWidth = 6f;
    public float areaHeight = 2f;
    public float areaDepth = 6f;
    public float changeDirectionTime = 5f;

    public LayerMask obstacleMask;
    public float detectionDistance = 2.5f;

    private Vector3 areaCenter;
    private Vector3 direction;
    private float timer;
    private float burstTimer;

    void Start()
    {
        // Wherever the fish is placed, that becomes its own swim area center.
        areaCenter = transform.position;
        PickDirection();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeDirectionTime)
        {
            PickDirection();
            timer = 0f;
        }

        AvoidObstacle();
        StayInArea();

        bool bursting = burstTimer > 0f;
        if (bursting) burstTimer -= Time.deltaTime;

        float currentSpeed = bursting ? burstSpeed : normalSpeed;
        float currentTurnSpeed = bursting ? burstTurnSpeed : turnSpeed;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            currentTurnSpeed * Time.deltaTime
        );

        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    void PickDirection()
    {
        direction = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.2f, 0.2f),
            Random.Range(-1f, 1f)
        ).normalized;
    }

    void AvoidObstacle()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, detectionDistance, obstacleMask))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
                return;

            direction = Vector3.Reflect(transform.forward, hit.normal).normalized;
            burstTimer = burstDuration;
            timer = 0f;
        }
    }

    void StayInArea()
    {
        Vector3 local = transform.position - areaCenter;

        if (Mathf.Abs(local.x) > areaWidth ||
            Mathf.Abs(local.y) > areaHeight ||
            Mathf.Abs(local.z) > areaDepth)
        {
            direction = (areaCenter - transform.position).normalized;
            burstTimer = burstDuration;
            timer = 0f;
        }
    }
}