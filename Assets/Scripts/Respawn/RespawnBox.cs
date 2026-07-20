using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class RespawnBox : MonoBehaviour
{
    [Header("Respawn Setup")]
    [SerializeField] private bool isScaling;

    [Header("Setup")]
    [SerializeField] private Transform rightRespawnPoint;
    [SerializeField] private Transform leftRespawnPoint;
    [SerializeField] private Transform centerFrontRespawnPoint;
    [SerializeField] private Transform centerBackRespawnPoint;
    [SerializeField] private Transform ViewTracker;

    [Header("DummySetup")]
    [SerializeField] private GameObject enemyDummy;
    [SerializeField] private Material DummyMaterial;

    [Header("Activate Setup")]
    [SerializeField] private EntityActivator myActivateItem;

   
    private List<Transform> respawnPoints = new List<Transform>();

    private GameObject dummyLeft;
    private GameObject dummyRight;
    private GameObject dummyTop;
    private GameObject dummyBottom;

    PlayerController player;
    public bool isActive {  get; private set; } = false;
    public bool isSingleSummon { private set; get; } = false;

    private void Awake()
    { 
        ClearDummy();
        SignUpRespawnPoints();

        
    }


    public Target RespawnTargetOn(Transform onTransform, GameObject respawnObject)
    {
        Vector3 onPos = onTransform.position;

        if (respawnObject == null) return null;

        GameObject newTarget = Instantiate(respawnObject, onPos, Quaternion.identity);

        Target targetData = newTarget.GetComponent<Target>();
        targetData.SetUpTarget(player, isScaling);

        targetData.RespawnTheTarget(ViewTracker.position);

        return targetData;
    }

    public Target RespawnRandomSide(GameObject respawnTargetPrefab)
    {

        Transform randomTransform = GetRandomRespawnPoint();
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


    private void SignUpRespawnPoints()
    {
        if (rightRespawnPoint != null)
        {
            rightRespawnPoint.GetComponent<MeshRenderer>().enabled = false;
            respawnPoints.Add(rightRespawnPoint);
        }
        if (leftRespawnPoint != null)
        {
            leftRespawnPoint.GetComponent<MeshRenderer>().enabled = false;
            respawnPoints.Add(leftRespawnPoint);
        }
        if (centerFrontRespawnPoint != null)
        {
            centerFrontRespawnPoint.GetComponent<MeshRenderer>().enabled = false;
            respawnPoints.Add(centerFrontRespawnPoint);
        }
        if (centerBackRespawnPoint != null)
        {
            centerBackRespawnPoint.GetComponent<MeshRenderer>().enabled = false;
            respawnPoints.Add(centerBackRespawnPoint);
        }

        isSingleSummon = respawnPoints.Count <= 1;
    }

    private void SetActive (bool isActive) => this.isActive = isActive;

    //TODO SomeTImes Activing some box take seconds i should implant time to make them active if needed
    [ContextMenu("testActive")]
    public void ActiveThisBox()
    {
        SetActive(true);
        myActivateItem.SetActive();
    }
    [ContextMenu("testDeActive")]

    public void DeActivateThisBox()
    {
        SetActive(false);
        myActivateItem.SetDeActive();

    }

    [ContextMenu ("DummyCheck")]
    public void ShowDummy()
    {
        dummyRight  = Instantiate(enemyDummy, rightRespawnPoint.position, Quaternion.identity);
        dummyLeft = Instantiate(enemyDummy, leftRespawnPoint.position, Quaternion.identity);

        dummyLeft.GetComponent<MeshRenderer>().material = DummyMaterial;
        dummyRight.GetComponent<MeshRenderer>().material = DummyMaterial;

        dummyLeft.transform.parent = leftRespawnPoint;
        dummyRight.transform.parent = rightRespawnPoint;
    }
    [ContextMenu("DummyClear")]


    public void ClearDummy()
    {
        if (dummyRight != null) 
            DestroyImmediate(dummyLeft);
        if (dummyLeft != null) 
            DestroyImmediate(dummyRight);
    }

    public void SetPlayer(PlayerController player) => this.player = player;
}
