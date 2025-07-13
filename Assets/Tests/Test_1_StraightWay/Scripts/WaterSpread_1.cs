using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaterSpread_1 : MonoBehaviour
{
    public static WaterSpread_1 singleton;
    public SpreadMode m_SpreadMode;
    public float spreadPerTime;
    public GameObject waterDrop_Prefab;
    public float dropStoppingDistance;
    public float dropsGravity = 9.81f;
    public Vector3 dropScale = Vector3.one;
    public Transform startPosition;
    public Transform destination;

    public List<Path> paths = new List<Path> ();
    private float spreadedTime;
    public int spawnedDrops;

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
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
        var dropObj = Instantiate(waterDrop_Prefab, startPosition.position, transform.rotation);
        dropObj.transform.localScale = dropScale;
        Drop_1 drop = dropObj.GetComponent<Drop_1>();
        drop.gravity = dropsGravity;
        drop.stoppingDistance = dropStoppingDistance;
        drop.target = destination;
        drop.dropNumber = spawnedDrops++;
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
            Debug.DrawLine(dropTrail.GetPosition(i), dropTrail.GetPosition(i + 1), Color.red);
    }
}
