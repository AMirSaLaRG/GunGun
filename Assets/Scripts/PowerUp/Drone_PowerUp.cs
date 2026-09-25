using DG.Tweening;
using System;
using UnityEngine;

public class Drone_PowerUp : MonoBehaviour
{

    [SerializeField] private float moveDuration = 2f;      // time to fly between waypoints
    [SerializeField] private float waitDuration = 1f;    // time to stand at each waypoint
    [SerializeField] private float lookRotationDuration = 0.3f;
    [SerializeField] private ESkillType power;
    [SerializeField] private MeshRenderer myMeshRenderer;
    PlayerController player;

    private Transform[] myWayPoints;
    private Material hullMaterial;
    private Color myColor = Color.white;
    private Transform mainCamera;

    private Sequence moveSequence;

    private void Awake()
    {
        Material mainHullMaterial = myMeshRenderer.materials[0];
        hullMaterial = new Material(mainHullMaterial);

        Material[] mats = myMeshRenderer.materials;   // copy
        mats[0] = hullMaterial;
        myMeshRenderer.materials = mats;
    }


    private void Start()
    {
        StartWaypointJourney();
    }

    public void SetUp(PlayerController player, ESkillType power, Transform[] waypoints, Color color)
    {
        this.player = player;
        this.power = power;
        myWayPoints = waypoints;
        myColor = color;
        this.mainCamera = player.mainCamera.transform;

        // hullMaterial already exists (created in Awake, which runs during Instantiate)
        hullMaterial.color = myColor;
    }

    public void Activate()
    {
        if (player != null) 
            player.skills.ActivateSkill(power);
    }

    public void StartWaypointJourney()
    {
        if (myWayPoints == null || myWayPoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned for the drone.");
            return;
        }

        // Kill any existing tween first (safe to call even if none exist)
        moveSequence?.Kill();

        // Waypoint 0 = spawn point. Place the drone there instantly,
        // do NOT wait or rotate there — just start flying to waypoint 1.
        transform.position = myWayPoints[0].position;

        moveSequence = DOTween.Sequence();

        // Start at index 1 so we skip any wait/stop at the spawn point
        for (int i = 1; i < myWayPoints.Length; i++)
        {
            Vector3 waypointPos = myWayPoints[i].position;

            // 1) Move toward the waypoint
            moveSequence.Append(
                transform.DOMove(waypointPos, moveDuration).SetEase(Ease.Linear)
            );

            // 2) Rotate to face the waypoint while moving (Y-axis only)
            moveSequence.Join(
                transform.DOLookAt(waypointPos, moveDuration, AxisConstraint.Y)
            );

            // 3) On arrival, turn to look at the camera
            if (mainCamera != null)
            {
                moveSequence.Append(
                    transform.DODynamicLookAt(player.mainCamera.transform.position, lookRotationDuration, AxisConstraint.Y)
                );
            }

            // 4) Wait at the waypoint while facing the camera
            moveSequence.AppendInterval(waitDuration);
        }

        // Destroy when the whole journey is done
        moveSequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    private Ease GetRandomEase()
    {
        Array values = Enum.GetValues(typeof(Ease));
        return (Ease)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    private void OnDestroy()
    {
        // Clean up tween if object is destroyed mid-flight
        moveSequence?.Kill();
    }
}
