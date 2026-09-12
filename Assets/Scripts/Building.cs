using UnityEngine;

public class Building : MonoBehaviour
{
    [Header ("Setup")]
    [SerializeField] public EBuilding building;
    [SerializeField] public int stage = 0;
    [SerializeField] public GameObject[] items;
    [SerializeField] public LevelItemData[] levels;
    [SerializeField] public bool isLock = true;

}
