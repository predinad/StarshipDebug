using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

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
}

