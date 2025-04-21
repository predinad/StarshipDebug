using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance; // Singleton instance
    private AudioSource audioSource; // AudioSource to play music

    [SerializeField] private string[] musicFiles; // Array of music file names (without extension)
    private int currentTrackIndex = 0; // Index of the currently playing track

    private void Awake()
    {
        // Singleton setup
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        // Set up the AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true; // Loop by default

        // Play the first track
        PlayTrack(currentTrackIndex);
    }

    /// <summary>
    /// Plays the specified track by index.
    /// </summary>
    /// <param name="trackIndex">Index of the track to play.</param>
    public void PlayTrack(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= musicFiles.Length)
        {
            Debug.LogError($"Track index {trackIndex} is out of bounds!");
            return;
        }

        currentTrackIndex = trackIndex;

        // Load the music file
        AudioClip musicClip = Resources.Load<AudioClip>(musicFiles[trackIndex]);
        if (musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError($"Music file '{musicFiles[trackIndex]}' not found in Resources folder!");
        }
    }

    /// <summary>
    /// Plays the next track in the playlist.
    /// </summary>
    public void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicFiles.Length;
        PlayTrack(currentTrackIndex);
    }

    /// <summary>
    /// Stops the music playback.
    /// </summary>
    public void StopMusic()
    {
        audioSource.Stop();
    }
}