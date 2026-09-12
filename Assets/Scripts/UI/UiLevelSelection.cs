using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiLevelSelection : MonoBehaviour
{
    private LevelManager levelManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private TextMeshProUGUI buildingName;
    [SerializeField] private Image buildingImage;
    [SerializeField] private Button survivalBtn;

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

        buildingName.text = buildingManager.currentBuilding.ToString();
        buildingImage.sprite = buildingManager.GetBuildingSprite(buildingManager.currentBuilding);

        if (levelManager.IsAllLevelsCompeleted())
            survivalBtn.IsActive();
    }
}
