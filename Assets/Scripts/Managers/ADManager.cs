using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class ADManager : MonoBehaviour,
    IUnityAdsInitializationListener,
    IUnityAdsLoadListener,
    IUnityAdsShowListener
{
    public static ADManager instance;

    [Header("Unity Ads")]
    [SerializeField] private string androidGameId;
    [SerializeField] private string iosGameId;
    [SerializeField] private string androidRewardedAdUnitId = "Rewarded_Android";
    [SerializeField] private string iosRewardedAdUnitId = "Rewarded_iOS";
    [SerializeField] private bool testMode = true;

    [Header("Reward")]
    [SerializeField] private int rewardedCoinAmount = 1;
    [SerializeField] private float cooldownHours = 3f;

    private string gameId;
    private string rewardedAdUnitId;

    private bool isInitialized;
    private bool isRewardedAdLoaded;
    private bool isShowingRewardedAd;

    public bool IsRewardedAdLoaded => isRewardedAdLoaded;
    public bool IsShowingRewardedAd => isShowingRewardedAd;

    public bool CanWatchRewardedAd
    {
        get
        {
            if (SaveManager.instance == null ||
                SaveManager.instance.data == null)
            {
                return false;
            }

            return !isShowingRewardedAd &&
                   IsCooldownFinished &&
                   isRewardedAdLoaded;
        }
    }

    public bool IsCooldownFinished
    {
        get
        {
            if (SaveManager.instance == null ||
                SaveManager.instance.data == null)
            {
                return false;
            }

            return DateTime.UtcNow.Ticks >=
                   SaveManager.instance.data.rewardedAdNextAvailableUtcTicks;
        }
    }

    public TimeSpan RemainingCooldown
    {
        get
        {
            if (SaveManager.instance == null ||
                SaveManager.instance.data == null)
            {
                return TimeSpan.Zero;
            }

            long remainingTicks =
                SaveManager.instance.data.rewardedAdNextAvailableUtcTicks -
                DateTime.UtcNow.Ticks;

            if (remainingTicks <= 0)
                return TimeSpan.Zero;

            return new TimeSpan(remainingTicks);
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SelectPlatformIds();
        InitializeAds();
    }

    private void SelectPlatformIds()
    {
#if UNITY_ANDROID
        gameId = androidGameId;
        rewardedAdUnitId = androidRewardedAdUnitId;
#elif UNITY_IOS
        gameId = iosGameId;
        rewardedAdUnitId = iosRewardedAdUnitId;
#else
        gameId = androidGameId;
        rewardedAdUnitId = androidRewardedAdUnitId;
#endif
    }

    private void InitializeAds()
    {
        if (string.IsNullOrWhiteSpace(gameId))
        {
            Debug.LogError(
                $"Unity Ads Game ID is missing for the current platform. " +
                $"Selected platform ID: '{gameId}'");
            return;
        }

        Advertisement.Initialize(gameId, testMode, this);
    }

    public void ShowRewardedAd()
    {
        if (SaveManager.instance == null ||
            SaveManager.instance.data == null)
        {
            Debug.LogError("Cannot show rewarded ad because SaveManager is missing.");
            return;
        }

        if (isShowingRewardedAd)
            return;

        if (!IsCooldownFinished)
        {
            Debug.Log(
                $"Rewarded ad cooldown active. Remaining time: {RemainingCooldown}");
            return;
        }

        if (!isInitialized)
        {
            Debug.Log("Rewarded ad is not initialized yet.");
            return;
        }

        if (!isRewardedAdLoaded)
        {
            Debug.Log("Rewarded ad is not loaded. Loading now...");
            Advertisement.Load(rewardedAdUnitId, this);
            return;
        }

        isShowingRewardedAd = true;
        Advertisement.Show(rewardedAdUnitId, this);
    }

    private void GiveReward()
    {
        if (SaveManager.instance == null ||
            SaveManager.instance.data == null)
        {
            return;
        }

        SaveManager.instance.AddSoldirCoins(rewardedCoinAmount);


        DateTime nextAvailableTime =
            DateTime.UtcNow.AddHours(cooldownHours);

        SaveManager.instance.data.rewardedAdNextAvailableUtcTicks =
            nextAvailableTime.Ticks;

        SaveManager.instance.Save();

        Debug.Log(
            $"Rewarded ad completed. Added {rewardedCoinAmount} soldir coin.");

        if (GameManager.instance != null &&
            GameManager.instance.uiManager != null)
        {
            GameManager.instance.uiManager.UpdatePoints();
        }
    }

    private void LoadRewardedAd()
    {
        if (!isInitialized || string.IsNullOrWhiteSpace(rewardedAdUnitId))
            return;

        isRewardedAdLoaded = false;
        Advertisement.Load(rewardedAdUnitId, this);
    }

    public void OnInitializationComplete()
    {
        isInitialized = true;
        Debug.Log("Unity Ads initialization completed.");

        LoadRewardedAd();
    }

    public void OnInitializationFailed(
        UnityAdsInitializationError error,
        string message)
    {
        isInitialized = false;
        Debug.LogError($"Unity Ads initialization failed: {error} - {message}");
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId != rewardedAdUnitId)
            return;

        isRewardedAdLoaded = true;
        Debug.Log("Rewarded ad loaded.");
    }

    public void OnUnityAdsFailedToLoad(
        string adUnitId,
        UnityAdsLoadError error,
        string message)
    {
        if (adUnitId != rewardedAdUnitId)
            return;

        isRewardedAdLoaded = false;
        Debug.LogError($"Rewarded ad failed to load: {error} - {message}");
    }

    public void OnUnityAdsShowComplete(
        string adUnitId,
        UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId != rewardedAdUnitId)
            return;

        isShowingRewardedAd = false;
        isRewardedAdLoaded = false;

        if (showCompletionState ==
            UnityAdsShowCompletionState.COMPLETED)
        {
            GiveReward();
        }

        LoadRewardedAd();
    }

    public void OnUnityAdsShowFailure(
        string adUnitId,
        UnityAdsShowError error,
        string message)
    {
        if (adUnitId != rewardedAdUnitId)
            return;

        isShowingRewardedAd = false;
        isRewardedAdLoaded = false;

        Debug.LogError($"Rewarded ad failed to show: {error} - {message}");

        LoadRewardedAd();
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("Rewarded ad started.");
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log("Rewarded ad clicked.");
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}