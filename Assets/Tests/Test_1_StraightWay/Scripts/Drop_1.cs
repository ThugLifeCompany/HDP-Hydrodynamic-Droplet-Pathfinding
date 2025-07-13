using UnityEngine;

public class Drop_1 : MonoBehaviour
{
    public Transform target;
    public float gravity = 9.81f;
    public float distanceLength;
    public float stoppingDistance;
    public int dropNumber = 1;

    [Header("Gizmos")]
    private bool isFinished;

    private void Update()
    {
        if (isFinished)
            return;

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
        WaterSpread_1.singleton.AddNewPath(new Path { drop = gameObject, pathLength = distanceLength });
        isFinished = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<CircleCollider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().enabled = false;
        gameObject.SetActive(false);
    }
}
