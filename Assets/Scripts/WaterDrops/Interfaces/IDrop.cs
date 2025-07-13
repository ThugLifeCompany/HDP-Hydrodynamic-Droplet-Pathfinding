using UnityEngine;

internal interface IDrop
{
    public void Init(float dropsGravity, float dropStoppingDistance, Transform destination);
}