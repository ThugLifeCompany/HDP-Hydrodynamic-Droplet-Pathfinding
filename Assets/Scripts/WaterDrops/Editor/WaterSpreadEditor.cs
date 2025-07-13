using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaterSpread))]
public class AIPlayer_ShooterEditor : Editor
{
    private void OnSceneGUI()
    {
        WaterSpread spread = (WaterSpread)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(spread.startPosition.position, Vector3.forward, Vector3.right, 360, spread.radius);

        Vector3 viewAngle01 = DirectionFromAngle(-spread.startPosition.eulerAngles.z, -spread.spreadAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(-spread.startPosition.eulerAngles.z, spread.spreadAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(spread.startPosition.position, spread.startPosition.position + viewAngle01 * spread.radius);
        Handles.DrawLine(spread.startPosition.position, spread.startPosition.position + viewAngle02 * spread.radius);
    }

    private Vector3 DirectionFromAngle(float eulerZ, float angleInDegrees)
    {
        angleInDegrees += eulerZ;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}
