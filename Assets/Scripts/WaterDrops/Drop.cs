using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour, IDrop
{
    public Transform target;
    public float gravity = 9.81f;
    public float distanceLength;
    public float stoppingDistance;
    public LayerMask dropletsLayer;
    public int dropNumber = 1;

    [Header("Cohension and Viscosity")]
    public float CVEfficencyFactor = 3;
    public bool useCohesion;
    public float cohesionPower;
    public bool useViscosity;
    public float viscosityPower;

    [Header("Turbulent")]
    public bool useTurbulent;
    private float sinRepeater;

    [Header("Gizmos")]
    public bool isDrawingGizmo;
    private bool isFinished;

    public void Init(float dropsGravity, float dropStoppingDistance, Transform destination)
    {
        gravity = dropsGravity;
        stoppingDistance = dropStoppingDistance;
        target = destination;
        //useCohesion = useViscosity = Random.value >= 0.5f;
    }

    private void Update()
    {
        if (isFinished)
            return;

        if (useViscosity)
            Viscosity();

        if (!useViscosity && useCohesion)
            Cohesion();

        sinRepeater = Mathf.Repeat(sinRepeater + Time.deltaTime, 1f);

        if (target != null)
            Movement();
        else
            return;

        if (Vector2.Distance(transform.position, target.position) < stoppingDistance)
            IntroduceAsAPath();
    }

    private void Movement()
    {
        var heading = (target.position - transform.position).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector3(rb.linearVelocity.x + heading.x * gravity * Time.deltaTime, rb.linearVelocity.y + heading.y * gravity * Time.deltaTime, 0);
        //GetComponent<Rigidbody2D>().velocity = heading * gravity * Time.deltaTime;
        distanceLength += rb.linearVelocity.magnitude * Time.deltaTime;
    }

    private void IntroduceAsAPath()
    {
        WaterSpread.singleton.AddNewPath(new Path { drop = gameObject, pathLength = distanceLength });
        isFinished = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<CircleCollider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().enabled = false;
        gameObject.SetActive(false);
    }

    private void Viscosity()
    {
        var drops = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x * CVEfficencyFactor, dropletsLayer);
        Debug.Log(drops.Length);
        if (drops.Length <= 1)
        {
            if (useViscosity)
                Cohesion();
            return;
        }

        for (int i = 0; i < drops.Length; i++)
        {
            if (drops[i].gameObject == gameObject)
                continue;
            Vector2 heading = (drops[i].transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, drops[i].transform.position);
            var max = (transform.localScale.x * (CVEfficencyFactor));
            float turbulent = useTurbulent ? Mathf.Sin(sinRepeater) : 1;
            Vector2 _force = heading * (1f - Mathf.Clamp(distance / max, 0, max)) * cohesionPower * turbulent; 
            drops[i].GetComponent<Rigidbody2D>().AddForce(_force);
        }
    }

    private void Cohesion()
    {
        var drops = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x * (CVEfficencyFactor * 1.5f), dropletsLayer);
        Debug.Log(drops.Length);
        for (int i = 0; i < drops.Length; i++)
        {
            if (drops[i].gameObject == gameObject)
                continue;
            float distance = Vector2.Distance(transform.position, drops[i].transform.position);
            if (distance <= (transform.localScale.x * CVEfficencyFactor))
                continue;
            Vector2 heading = (drops[i].transform.position - transform.position).normalized;
            var max = (transform.localScale.x * (CVEfficencyFactor * 1.5f));
            float turbulent = useTurbulent ? Mathf.Sin(sinRepeater) : 1;
            Vector2 _force = -heading * Mathf.Clamp(distance / max, 0, max) * viscosityPower * turbulent;
            drops[i].GetComponent<Rigidbody2D>().AddForce(_force);
        }
    }

    private void OnDrawGizmos()
    {
        if (!isDrawingGizmo)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * CVEfficencyFactor);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * CVEfficencyFactor * 1.5f);
    }
}
