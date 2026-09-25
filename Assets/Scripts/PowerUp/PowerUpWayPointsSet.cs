using UnityEngine;

public class PowerUpWayPointsSet : MonoBehaviour
{
    [SerializeField] private Transform[] startAndEndBoxes;
    [SerializeField] private Transform[] stopPoints;

    public Transform[] GetStartAndEndOfSet()
    {
        return startAndEndBoxes;
    }
    public Transform[] GetStopPointsOfSet()
    {
        return stopPoints;
    }
}
