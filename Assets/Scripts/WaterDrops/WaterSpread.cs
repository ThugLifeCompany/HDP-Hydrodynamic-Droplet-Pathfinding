using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaterSpread : MonoBehaviour
{
    public static WaterSpread singleton;
    public SpreadMode m_SpreadMode;
    public float spreadPerTime;
    public GameObject waterDrop_Prefab;
    public float dropStoppingDistance;
    public float dropsPerSpread;
    public LayerMask spreadMask;
    public float spreadForce;
    [Range(0, 360)] public float spreadAngle;
    public float dropsGravity = 9.81f;
    public Vector3 dropScale = Vector3.one;
    public Transform startPosition;
    public Transform destination;

    public List<Path> paths = new List<Path> ();
    private float spreadedTime;
    public bool canSpread;

    [Header("Gizmos")]
    public float radius;
    public float timeScale = 1;

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
        Time.timeScale = timeScale;
        canSpread = !Physics2D.OverlapCircle(startPosition.position, radius, spreadMask);

        switch (m_SpreadMode)
        {
            case SpreadMode.Key:
                if (Input.GetKeyDown(KeyCode.Space))
                    SpreadDrops();
                break;
            case SpreadMode.Loop:
                if (Time.time >= spreadedTime + spreadPerTime)
                {
                    SpreadDrops();
                    spreadedTime = Time.time;
                }
                break;
        }

        DrawShortestPath();
    }

    private void SpreadDrops()
    {
        if (!canSpread)
            return;

        for (int i = 0; i < dropsPerSpread; i++)
        {
            var dropObj = Instantiate(waterDrop_Prefab, startPosition.position, transform.rotation);
            dropObj.transform.localScale = dropScale;
            var randomDirection = -startPosition.localEulerAngles.z + (Random.Range(-spreadAngle, spreadAngle) / 2);
            dropObj.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(Mathf.Sin(randomDirection * Mathf.Deg2Rad) * spreadForce, Mathf.Cos(randomDirection * Mathf.Deg2Rad) * spreadForce, 0);
            dropObj.GetComponent<IDrop>().Init(dropsGravity, dropStoppingDistance, destination);
        }
    }

    public void AddNewPath(Path _path)
    {
        paths.Add(_path);
        paths = paths.OrderBy(p => p.pathLength).ToList();
        Debug.Log("added");
    }

    private void DrawShortestPath()
    {
        if (paths.Count <= 0)
            return;

        var dropTrail = paths[0].drop.GetComponent<TrailRenderer>();
        for (int i = 0; i < dropTrail.positionCount - 1; i++)
        {
            Debug.DrawLine(dropTrail.GetPosition(i), dropTrail.GetPosition(i + 1), Color.red);
        }
        //dropTrail.gameObject.SetActive(true);
    }
}
