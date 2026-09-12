using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class DynamicUiAdjuster : MonoBehaviour
{
    [Header("Mobile Margins (Percentage 0-50%)")]
    [Range(0f, 50f)] public float mobileTopPercent = 30f;
    [Range(0f, 50f)] public float mobileBottomPercent = 30f;
    [Range(0f, 50f)] public float mobileLeftPercent = 15f;
    [Range(0f, 50f)] public float mobileRightPercent = 15f;

    [Header("Tablet Margins (Percentage 0-50%)")]
    [Range(0f, 50f)] public float tabletTopPercent = 15f;
    [Range(0f, 50f)] public float tabletBottomPercent = 15f;
    [Range(0f, 50f)] public float tabletLeftPercent = 30f;
    [Range(0f, 50f)] public float tabletRightPercent = 30f;

    [Header("Threshold")]
    [Tooltip("Aspect ratios below this value (e.g., 1.6 for 16:10 or 4:3 in portrait) are treated as tablets.")]
    public float tabletAspectRatioThreshold = 1.6f;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        AdjustMargins();
    }

#if UNITY_EDITOR
    // Automatically updates UI margins in the Editor when values change in Inspector
    private void OnValidate()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        AdjustMargins();
    }
#endif

    public void AdjustMargins()
    {
        if (rectTransform == null) return;

        // Calculate aspect ratio (height / width for portrait mode)
        float aspectRatio = (float)Screen.height / (float)Screen.width;

        // Determine device category based on portrait aspect ratio
        bool isTablet = aspectRatio < tabletAspectRatioThreshold;

        // Choose appropriate target margins
        float topPercent = isTablet ? tabletTopPercent : mobileTopPercent;
        float bottomPercent = isTablet ? tabletBottomPercent : mobileBottomPercent;
        float leftPercent = isTablet ? tabletLeftPercent : mobileLeftPercent;
        float rightPercent = isTablet ? tabletRightPercent : mobileRightPercent;

        // Convert percentages (0-100) to normalized anchor coordinates (0.0 - 1.0)
        float anchorMinX = leftPercent / 100f;
        float anchorMaxX = 1f - (rightPercent / 100f);
        float anchorMinY = bottomPercent / 100f;
        float anchorMaxY = 1f - (topPercent / 100f);

        // Apply 4-way percentage anchors
        rectTransform.anchorMin = new Vector2(anchorMinX, anchorMinY);
        rectTransform.anchorMax = new Vector2(anchorMaxX, anchorMaxY);

        // Clear pixel offsets so pop-up fills anchor bounds precisely
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
