using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySfx(AudioSource audioToPlay, bool randomPitch = false, bool detachAndAutoDestroy = false)
    {
        if (audioToPlay.clip == null)
        {
            Debug.Log("Could not play " + audioToPlay.gameObject.name + ". There is no audio Clip assigned!");
            return;
        }

        if (audioToPlay.isPlaying)
            audioToPlay.Stop();

        audioToPlay.pitch = randomPitch ? Random.Range(.9f, 1.1f) : 1;

        if (detachAndAutoDestroy)
        {
            // Detach from the maker so it survives destruction
            audioToPlay.transform.parent = null;
            audioToPlay.Play();

            // Destroy the AudioSource GameObject after the clip finishes
            Destroy(audioToPlay.gameObject, audioToPlay.clip.length);
        }
        else
        {
            audioToPlay.Play();
        }
    }
}
