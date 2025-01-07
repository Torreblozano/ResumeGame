using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Control SFX and music by addressables
/// </summary>
public class AudioManager : CustomSingleton<AudioManager>
{
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        }
        backgroundMusicSource.loop = true;
        backgroundMusicSource.playOnAwake = false;

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    public void PauseBackgroundMusic(bool pause)
    {
        if (pause && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Pause();
        }
        else if (!pause && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
    }

    public void StopSFX()
    {
        if (sfxSource.isPlaying)
            sfxSource.Stop();
    }

    /// <summary>
    /// Plays background music from an Addressable.
    /// </summary>
    /// <param name="address">The Addressable address of the AudioClip.</param>
    public void PlayBackgroundMusic(string address)
    {
        Addressables.LoadAssetAsync<AudioClip>(address).Completed += (operation) =>
        {
            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                AudioClip clip = operation.Result;
                if (backgroundMusicSource.clip == clip) return;

                backgroundMusicSource.clip = clip;
                backgroundMusicSource.loop = true;
                backgroundMusicSource.Play();
            }
            else
            {
                Debug.LogError($"Failed to load background music: {address}");
            }
        };
    }

    /// <summary>
    /// Plays a one-shot sound effect from an Addressable.
    /// </summary>
    /// <param name="address">The Addressable address of the AudioClip.</param>
    public void PlaySFX(string address)
    {
        Addressables.LoadAssetAsync<AudioClip>(address).Completed += (operation) =>
        {
            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                sfxSource.PlayOneShot(operation.Result);
            }
            else
            {
                Debug.LogError($"Failed to load SFX: {address}");
            }
        };
    }

    /// <summary>
    /// Releases Addressables after use (optional for memory management).
    /// </summary>
    /// <param name="address">The Addressable address to release.</param>
    public void ReleaseAddressable(string address)
    {
        Addressables.Release(address);
    }
}
