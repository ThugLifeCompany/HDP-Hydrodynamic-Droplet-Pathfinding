using UnityEngine;

public enum SpreadMode { Key, Loop }

[System.Serializable]
public class Path
{
    public GameObject drop;
    public Vector3[] pathPositions;
    public float pathLength;
}
