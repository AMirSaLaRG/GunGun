using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;

public class RespawnBox : MonoBehaviour
{
    private RespawnManager respawnManager;

    public bool showGizmos = true;

    [Header("Respawn Setup")]
    [SerializeField] private bool isScaling;

    [SerializeField] Transform waypointsParent;
    [SerializeField] Transform secondaryWayPointsParent;

    private Transform[] waypoints;
    private Transform[] secondaryWayPoints;

    [Header("Activate Setup")]
    [SerializeField] private EntityActivator myActivateItem;

    [Header("DummySetup")]
    [SerializeField] private GameObject enemyDummy;
    [SerializeField] private Material DummyMaterial;

    private Transform myLastSide;  
    private List<Transform> respawnPoints = new List<Transform>();
    private PlayerController player;
    public bool isSingleSummon;
    public bool isActive { private set; get; } = false;

    private void Awake()
    {
        if (secondaryWayPoints == null || secondaryWayPoints.Length == 0)
            isSingleSummon = true;

        if (myActivateItem != null)
            myActivateItem.AddBox(this);
    }

    private void Start()
    {
        HideGuides();

    }

    private void HideGuides()
    {
        foreach (var point in waypoints)
            if (point.TryGetComponent<Renderer>(out Renderer r))
                r.enabled = false;
        foreach (var point in secondaryWayPoints)
            if (point.TryGetComponent<Renderer>(out Renderer r))
                r.enabled = false;

        showGizmos = false;
    }

    public void RespawnLeftTheBox(Target boxUser)
    {
        respawnManager.OnRespawnLeaveTheBox(this, boxUser);   
    }

    public Target RespawnTarget(GameObject respawnObject, bool isMainWayPoints = true)
    {
        if (respawnObject == null) return null;

        GameObject newTarget = Instantiate(respawnObject, waypoints[0].position, Quaternion.identity);

        Target targetData = newTarget.GetComponent<Target>();

        Transform[] targetWaypoints = waypoints;
        if (isMainWayPoints == false)
            targetWaypoints = secondaryWayPoints;
        
        targetData.SetTargetWayPoints(targetWaypoints);
        targetData.SetScalingAtRespawn(isScaling);

        return targetData;
    }

    public Target[] DoubleRespawn(GameObject respawnTargetPrefab1, GameObject RespawnTargetPrefab2)
    {
        if (isSingleSummon == true)
        {
            Debug.Log("This is single summon tring to summon two");
            return null;
        }

        Target[] targets = new Target[2];

        targets[1] = RespawnTarget(respawnTargetPrefab1);
        targets[2] = RespawnTarget(RespawnTargetPrefab2);

        return targets;
    }

    [ContextMenu("testActive")]
    public void ActiveThisBox(out float activateTime, float forceActivateTime)
    {
        isActive = true;
        myActivateItem.SetActive(out activateTime, forceActivateTime);
    }
    [ContextMenu("testDeActive")]

    public void DeActivateThisBox(bool isImediate = false)
    {
        isActive = false;
        myActivateItem.CheckCloseRequsts(isImediate);
    }

    public void ImediateDeActivate()
    {
        isActive = false;
        myActivateItem.CheckCloseRequsts();
    }

    public void SetPlayer(PlayerController player) => this.player = player;

    public void SetManager(RespawnManager manager) => respawnManager = manager;

    public Transform[] GetWayPoints() => waypoints;
    public Transform[] GetSecondaryWayPoints() => secondaryWayPoints;

    public GameObject GetActivator() => myActivateItem.gameObject;

    private void OnValidate()
    {
        // Clear waypoints if parent is null
        if (waypointsParent == null)
        {
            waypoints = new Transform[0]; // Empty array
        }
        else
        {
            // Get all transforms including parent
            Transform[] allTransforms = waypointsParent.GetComponentsInChildren<Transform>();

            // Skip the first one (parent) and take the rest
            waypoints = new Transform[allTransforms.Length - 1];
            for (int i = 1; i < allTransforms.Length; i++)
            {
                waypoints[i - 1] = allTransforms[i];
            }
        }

        // Clear secondary waypoints if parent is null
        if (secondaryWayPointsParent == null)
        {
            secondaryWayPoints = new Transform[0]; // Empty array
        }
        else
        {
            Transform[] allSecondary = secondaryWayPointsParent.GetComponentsInChildren<Transform>();
            secondaryWayPoints = new Transform[allSecondary.Length - 1];
            for (int i = 1; i < allSecondary.Length; i++)
            {
                secondaryWayPoints[i - 1] = allSecondary[i];
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return;

        if (waypoints != null && waypoints.Length > 1)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (i == 0)
                    continue;

                Gizmos.DrawLine(waypoints[i].position, waypoints[i - 1].position);
            }
        }
        

        if (secondaryWayPoints != null && secondaryWayPoints.Length > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < secondaryWayPoints.Length; i++)
            {
                if (i == 0)
                    continue;

                Gizmos.DrawLine(secondaryWayPoints[i].position, secondaryWayPoints[i - 1].position);
            }
        }


    }
}
