using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip dragItemStart;
    [SerializeField] private AudioClip dropItem;
    [SerializeField] private AudioClip completePuzzle;
    [SerializeField] private AudioClip wrongAnswer;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayButtonClick() => audioSource.PlayOneShot(buttonClick);
    public void PlayDragStart() => audioSource.PlayOneShot(dragItemStart);
    public void PlayDropItem() => audioSource.PlayOneShot(dropItem);
    public void PlayCompletePuzzle() => audioSource.PlayOneShot(completePuzzle);
    public void PlayWrongAnswer() => audioSource.PlayOneShot(wrongAnswer);

    public void SetMusicVolume()
    {
        float value = musicSlider.value;
        value = Mathf.Clamp(value, 0.0001f, 1f);
         audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume()
    {
        float value = sfxSlider.value;
        value = Mathf.Clamp(value, 0.0001f, 1f);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
}

