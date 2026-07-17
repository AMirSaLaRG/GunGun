using DG.Tweening;
using UnityEngine;

public class ElevatorMainMenu : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform Door;
    [SerializeField] private Transform button;

    private Vector3 startingPos;
    private void Start()
    {
        startingPos = transform.position;
    }

    [ContextMenu("Test look at level change")]
    private void ChangeLevelAnimation()
    {
        transform.DORotate(new Vector3(0, 90, 0), 1);
    }
    [ContextMenu("Test look at level")]
    private void LevelView()
    {
        transform.DORotate(new Vector3(0, 0, 0), 1);
    }
    [ContextMenu("Test EnterScene")]
    private void OpenScene()
    {
        transform.DORotate(new Vector3(0, 180, 0), 1).OnComplete(() => {

            Door.DOScaleX(20, 1).OnComplete(() => { transform.DOMoveZ(-2, 1); });
               
        });
    }
    [ContextMenu("Test LeaveScene")]
    private void LeaveScene()
    {
        transform.DOMoveZ(startingPos.z, 1).OnComplete(() => {

            Door.DOScaleX(100, 1).OnComplete(() => { transform.DORotate(new Vector3(0, 0, 0), 1); });
               
        });
    }
    [ContextMenu("Level Change scene change")]
    private void SceneChangeAnim()
    {
        transform.DOLocalMove(new Vector3(0, startingPos.y + .05f, 0), .3f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            transform.DOLocalMove(new Vector3(0, startingPos.y, 0), .3f).SetEase(Ease.InOutElastic);

        });

    }
}
