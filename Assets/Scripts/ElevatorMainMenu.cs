using DG.Tweening;
using UnityEngine;

public class ElevatorMainMenu : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform Door;
    [SerializeField] private Transform button;
    public float actionTime = 1;

    private Vector3 startingPos;
    private void Start()
    {
        startingPos = transform.position;
    }

    [ContextMenu("Test look at level change")]
    public void ChangeLevelAnimation()
    {
        transform.DORotate(new Vector3(0, 90, 0), actionTime);
    }
    [ContextMenu("Test look at level")]
    public void LevelView()
    {
        transform.DORotate(new Vector3(0, 0, 0), actionTime);
    }
    [ContextMenu("Test EnterScene")]
    public void EnterTheLevel()
    {
        transform.DORotate(new Vector3(0, 180, 0), actionTime).OnComplete(() => {

            Door.DOScaleX(20, actionTime).OnComplete(() => { transform.DOMoveZ(-2, actionTime); });
               
        });
    }
    [ContextMenu("Test LeaveScene")]
    public void LeaveScene()
    {
        transform.DOMoveZ(startingPos.z, actionTime).OnComplete(() => {

            Door.DOScaleX(100, actionTime).OnComplete(() => { transform.DORotate(new Vector3(0, 0, 0), actionTime); });
               
        });
    }
    [ContextMenu("Level Change scene change")]
    public void SceneChangeAnim()
    {
        transform.DOLocalMove(new Vector3(0, startingPos.y + .05f, 0), .3f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            transform.DOLocalMove(new Vector3(0, startingPos.y, 0), .3f).SetEase(Ease.InOutElastic);

        });

    }
}
