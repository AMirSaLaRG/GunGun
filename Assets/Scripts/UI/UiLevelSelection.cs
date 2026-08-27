using UnityEngine;

public class UiLevelSelection : MonoBehaviour
{
    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
    }


    private void OnEnable()
    {
        if (levelManager == null)
            levelManager = FindFirstObjectByType<LevelManager>();

        if (levelManager != null)
            levelManager.PopulateLevels();
    }
}
