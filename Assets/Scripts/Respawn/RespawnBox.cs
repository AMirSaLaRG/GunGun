using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;

public class RespawnBox : MonoBehaviour
{
    private RespawnManager respawnManager;

    public bool showGizmos = true;

    [Header("Respawn Setup")]
    [SerializeField] private bool isScaling;

    [Header("Setup")]
    [SerializeField] private Transform rightRespawnPoint;
    [SerializeField] private Transform leftRespawnPoint;
    [SerializeField] private Transform centerFrontRespawnPoint;
    [SerializeField] private Transform centerBackRespawnPoint;
    [SerializeField] private Transform hostageStandPoint;
    [SerializeField] private Transform ViewTracker;

    [Header("DummySetup")]
    [SerializeField] private GameObject enemyDummy;
    [SerializeField] private Material DummyMaterial;

    [Header("Activate Setup")]
    [SerializeField] private EntityActivator myActivateItem;

    private Transform myLastSide;
   
    private List<Transform> respawnPoints = new List<Transform>();


    private PlayerController player;



    public bool isSingleSummon;
    public bool isActive { private set; get; } = false;

    private void Awake()
    { 
        SignUpRespawnPoints();

        if (hostageStandPoint == null)
            isSingleSummon = true;

        if (myActivateItem != null)
            myActivateItem.AddBox(this);
    }

    public void LeavingTheBox(Target boxUser)
    {

        respawnManager.OnLeavingTheBox(this, boxUser);
        
    }

    public Target RespawnTargetOn(Transform onTransform, GameObject respawnObject)
    {
        Vector3 onPos = onTransform.position;

        if (respawnObject == null) return null;

        GameObject newTarget = Instantiate(respawnObject, onPos, Quaternion.identity);

        Target targetData = newTarget.GetComponent<Target>();

        targetData.SetTargetsFace(ViewTracker.position);
        targetData.SetScalingAtRespawn(isScaling);

        return targetData;
    }

    public Target RespawnRandomSide(GameObject respawnTargetPrefab, bool useLast = false)
    {
        Transform randomTransform = null;

        if (useLast == false || myLastSide == null)
        {

            randomTransform = GetRandomRespawnPoint();
            myLastSide = randomTransform;
        }
        else
        {
            randomTransform = myLastSide.transform;
        }


        if (randomTransform == null)
        {
            Debug.Log("there is no respawnPoint. plz sign them up");
            return null;
        }

        Target newTarget = RespawnTargetOn(randomTransform, respawnTargetPrefab);
        if (newTarget == null)
        {
            Debug.Log("Could Not Create new Target");
        }

        return newTarget;
    }
    [ContextMenu("RespawnTest")]
    public void RepawnTest()
    {
        RespawnRandomSide(enemyDummy);
    }

    public Target[] DoubleRespawnRandomSide(GameObject respawnTargetPrefab1, GameObject RespawnTargetPrefab2)
    {
        Target[] targets = new Target[2];
        GameObject[] prefabs = new GameObject[2];
        prefabs[0] = respawnTargetPrefab1;
        prefabs[1] = RespawnTargetPrefab2;

        Transform[] randomTransforms = GetDoubleRandomRespawnPoint();
        if (randomTransforms == null)
        {
            Debug.Log("there is no respawnPoint. plz sign them up" + gameObject.name);
            return null;
        }

        for (int i = 0; i < randomTransforms.Length; i++)
        {
            Target newTarget = RespawnTargetOn(randomTransforms[i], prefabs[i]);
            if (newTarget == null)
            {
                Debug.Log("Could Not Create new Target");
            }
            targets[i] = newTarget;
        }
        

        return targets;
    }


    
    private Transform GetRandomRespawnPoint()
    {
        int randomIndex = Random.Range(0, respawnPoints.Count);
        return respawnPoints[randomIndex];
    }
    private Transform[] GetDoubleRandomRespawnPoint()
    {
        Debug.Log(gameObject.name + "Getting double" + gameObject.name);

        if (respawnPoints.Count < 2)
            return null;

        Transform[] doubleRespawnPoint = new Transform[2];


        List<Transform> copyList = new List<Transform>();
        copyList.AddRange(respawnPoints);

        int randomIndex1 = Random.Range(0, respawnPoints.Count);
        copyList.RemoveAt(randomIndex1);

        int randomIndex2 = Random.Range(0, copyList.Count);


        doubleRespawnPoint[0] = respawnPoints[randomIndex1];
        doubleRespawnPoint[1] = copyList[randomIndex2];

        Debug.Log(gameObject.name + "respawn points count > 2");
        return doubleRespawnPoint;
    }


    private void SignUpRespawnPoints(bool showPoints = false)
    {
        if (rightRespawnPoint != null)
        {
            rightRespawnPoint.GetComponent<MeshRenderer>().enabled = showPoints;
            respawnPoints.Add(rightRespawnPoint);
        }
        if (leftRespawnPoint != null)
        {
            leftRespawnPoint.GetComponent<MeshRenderer>().enabled = showPoints;
            respawnPoints.Add(leftRespawnPoint);
        }
        if (centerFrontRespawnPoint != null)
        {
            centerFrontRespawnPoint.GetComponent<MeshRenderer>().enabled = showPoints;
            respawnPoints.Add(centerFrontRespawnPoint);
        }
        if (centerBackRespawnPoint != null)
        {
            centerBackRespawnPoint.GetComponent<MeshRenderer>().enabled = showPoints;
            respawnPoints.Add(centerBackRespawnPoint);
        }

        if (hostageStandPoint != null)
            hostageStandPoint.GetComponent<MeshRenderer>().enabled = showPoints;

        isSingleSummon = respawnPoints.Count <= 1;
    }

    [ContextMenu("testActive")]
    public void ActiveThisBox(out float activateTime)
    {
        isActive = true;
        myActivateItem.SetActive(out activateTime);
    }
    [ContextMenu("testDeActive")]

    public void DeActivateThisBox()
    {
        isActive = false;
        myActivateItem.CheckCloseRequsts();
    }

    public void SetPlayer(PlayerController player) => this.player = player;

    public void SetManager(RespawnManager manager) => respawnManager = manager;
    public Transform GetHostagePoint() => hostageStandPoint;

    public GameObject GetActivator() => myActivateItem.gameObject;

    private void OnValidate()
    {
        SignUpRespawnPoints(true);
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return;

        Gizmos.color = Color.red;
        foreach (var respawnPoint in respawnPoints)
        {
            if (respawnPoint != null)
                Gizmos.DrawLine(respawnPoint.transform.position, ViewTracker.transform.position);
        }

        if (hostageStandPoint != null)
        {
            Gizmos.color = Color.green;
            foreach (var respawnPoint in respawnPoints)
                Gizmos.DrawLine(respawnPoint.transform.position, hostageStandPoint.position);
        }


    }
}
