using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("BGM Details")]
    [SerializeField] private bool playBgm;
    [SerializeField] private AudioSource[] inGameBgm;
    [SerializeField] private AudioSource[] menuBgm;

    private int currentBgmIndex;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InvokeRepeating(nameof(PlayMusicIfNeeded), 0f, 2f);
    }

    public void PlayBGM(
        int bgmToPlay,
        EGameState requestedGameState = EGameState.None)
    {

        if (requestedGameState == EGameState.None)
        {
            if (GameManager.instance == null)
                return;

            requestedGameState = GameManager.instance.gameState;
        }

        AudioSource[] playlist =
            requestedGameState == EGameState.inMenu
                ? menuBgm
                : inGameBgm;

        if (playlist == null || playlist.Length == 0)
        {
            Debug.LogWarning(
                $"No music assigned for state {requestedGameState}.");

            return;
        }

        if (bgmToPlay < 0 || bgmToPlay >= playlist.Length)
            bgmToPlay = 0;

        if (playlist[bgmToPlay] == null)
        {
            Debug.LogWarning(
                $"Audio source at index {bgmToPlay} is not assigned.");

            return;
        }

        StopAllBGM();

        currentBgmIndex = bgmToPlay;
        playlist[currentBgmIndex].Play();
    }

    public void PlayRandomBGM(
        EGameState requestedGameState = EGameState.None)
    {
        if (requestedGameState == EGameState.None)
        {
            if (GameManager.instance == null)
                return;

            requestedGameState = GameManager.instance.gameState;
        }

        AudioSource[] playlist =
            requestedGameState == EGameState.inMenu
                ? menuBgm
                : inGameBgm;

        if (playlist == null || playlist.Length == 0)
        {
            Debug.LogWarning(
                $"No music assigned for state {requestedGameState}.");

            return;
        }

        int randomIndex = Random.Range(0, playlist.Length);
        PlayBGM(randomIndex, requestedGameState);
    }

    private void PlayMusicIfNeeded()
    {
        if (GameManager.instance == null)
        {
            Debug.LogWarning("AudioManager: GameManager.instance is null.");
            return;
        }

        EGameState state = GameManager.instance.gameState;



        if (!playBgm)
        {
            Debug.LogWarning("AudioManager: Play Bgm is disabled.");
            return;
        }

        if (state != EGameState.inMenu &&
            state != EGameState.InGame)
        {
            Debug.LogWarning($"AudioManager: unsupported state {state}.");
            return;
        }

        AudioSource[] playlist =
            state == EGameState.inMenu
                ? menuBgm
                : inGameBgm;

        if (playlist == null || playlist.Length == 0)
        {
            Debug.LogWarning(
                $"AudioManager: no AudioSources assigned for {state}.");

            return;
        }

        if (currentBgmIndex < 0 ||
            currentBgmIndex >= playlist.Length)
        {


            PlayRandomBGM(state);
            return;
        }

        AudioSource currentSource = playlist[currentBgmIndex];

        if (currentSource == null)
        {


            PlayRandomBGM(state);
            return;
        }


        if (!currentSource.isPlaying)
        {
            PlayRandomBGM(state);
        }
    }

    [ContextMenu("Stop All Music")]
    public void StopAllBGM()
    {
        if (menuBgm != null)
        {
            foreach (AudioSource source in menuBgm)
            {
                if (source != null)
                    source.Stop();
            }
        }

        if (inGameBgm != null)
        {
            foreach (AudioSource source in inGameBgm)
            {
                if (source != null)
                    source.Stop();
            }
        }
    }

    public void PlaySfx(
        AudioSource audioToPlay,
        bool randomPitch = false,
        bool detachAndAutoDestroy = false)
    {
        if (audioToPlay == null)
        {
            Debug.LogWarning("Cannot play SFX because the AudioSource is null.");
            return;
        }

        if (audioToPlay.clip == null)
        {
            Debug.LogWarning(
                $"Could not play {audioToPlay.gameObject.name}. " +
                "There is no audio clip assigned.");

            return;
        }

        if (audioToPlay.isPlaying)
            audioToPlay.Stop();

        audioToPlay.pitch =
            randomPitch ? Random.Range(0.9f, 1.1f) : 1f;

        if (detachAndAutoDestroy)
        {
            audioToPlay.transform.SetParent(null);
            audioToPlay.Play();

            Destroy(
                audioToPlay.gameObject,
                audioToPlay.clip.length);
        }
        else
        {
            audioToPlay.Play();
        }
    }

    public void PlaySfx(
        AudioSource audioToPlay,
        float targetDuration,
        bool detachAndAutoDestroy = false)
    {
        if (audioToPlay == null)
        {
            Debug.LogWarning("Cannot play SFX because the AudioSource is null.");
            return;
        }

        if (audioToPlay.clip == null)
        {
            Debug.LogWarning(
                $"Could not play {audioToPlay.gameObject.name}. " +
                "There is no audio clip assigned.");

            return;
        }

        if (targetDuration <= 0f)
        {
            Debug.LogWarning(
                "Target duration must be greater than zero.");

            return;
        }

        if (audioToPlay.isPlaying)
            audioToPlay.Stop();

        float calculatedPitch =
            audioToPlay.clip.length / targetDuration;

        audioToPlay.pitch = calculatedPitch;

        if (detachAndAutoDestroy)
        {
            audioToPlay.transform.SetParent(null);
            audioToPlay.Play();

            Destroy(
                audioToPlay.gameObject,
                targetDuration);
        }
        else
        {
            audioToPlay.Play();
        }
    }

    public void StopAllSfxInScene()
    {
        AudioSource[] allSources =
            FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource source in allSources)
        {
            if (source != null && source.isPlaying)
                source.Stop();
        }
    }

    public void PlayMenuMusic()
    {
        if (!playBgm)
        {
            Debug.LogWarning("AudioManager Play Bgm is disabled.");
            return;
        }

        if (menuBgm == null || menuBgm.Length == 0)
        {
            Debug.LogWarning("Menu Bgm array is empty.");
            return;
        }

        PlayRandomBGM(EGameState.inMenu);
    }
}
