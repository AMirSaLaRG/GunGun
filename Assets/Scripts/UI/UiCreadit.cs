using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiCreadit : MonoBehaviour
{

    [Header("Social Media Links")]
    [SerializeField] private string instagramUrl = "https://www.instagram.com/yourusername/";
    [SerializeField] private string telegramUrl = "https://t.me/yourusername";
    [SerializeField] private string githubUrl = "https://github.com/yourusername";

    [Header("Social Media Buttons")]
    [SerializeField] private Button instagramButton;
    [SerializeField] private Button telegramButton;
    [SerializeField] private Button githubButton;

    [Header("Optional - Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    void Start()
    {


        // Setup social buttons
        if (instagramButton != null)
            instagramButton.onClick.AddListener(() => OpenURL(instagramUrl));

        if (telegramButton != null)
            telegramButton.onClick.AddListener(() => OpenURL(telegramUrl));

        if (githubButton != null)
            githubButton.onClick.AddListener(() => OpenURL(githubUrl));


    }

    // Open URL in browser (works in both editor and build)
    private void OpenURL(string url)
    {
#if UNITY_WEBGL
        Application.OpenURL(url);
#elif UNITY_ANDROID || UNITY_IOS
        Application.OpenURL(url);  // Works on mobile too!
#else
        // Windows, Mac, Linux
        System.Diagnostics.Process.Start(url);
#endif
    }
}
