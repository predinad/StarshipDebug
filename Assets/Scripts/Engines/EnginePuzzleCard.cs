using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnginePuzzleCard : MonoBehaviour
{
    [Header("Puzzle Details")]
    [SerializeField] private Sprite[] images; // Array of images to cycle through
    [SerializeField] private string[] imageDescriptions; // Corresponding descriptions for images
    [SerializeField] private int correctAnswerIndex = 0;   // Serialized correct index
    [SerializeField] private int questionNumber = 0;     // Serialized index for EngineTracker

    [Header("Engine Tracker")]
    [SerializeField] private EngineTracker engineTracker; // Reference to the EngineTracker

    [Header("Answer Delay")]
    [SerializeField] private float answerDelay = 1f; // Minimum time before answering is allowed
    private float timeLaunched;
    private bool canAnswer = false;

    [Header("Disable On Answer")]
    [SerializeField] private GameObject[] objectsToDisableOnAnswer; // Object to disable after answering

    [Header("Player Interaction")]
    [SerializeField] private GameObject playerObject; // Explicitly assign the player object

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // AudioSource component
    [SerializeField] private AudioClip audioClip; // Audio clip to play

    [Header("Visual Feedback")]
    [SerializeField] private GameObject greenCheck; // Serialized object to activate

    private int currentIndex = 0; // Tracks the current index
    private float inputDelay = 0.2f; // Delay to prevent rapid cycling
    private float nextInputTime = 0f; // Tracks when the next input is allowed
    private bool hasAnswered = false; // Track if this card has been answered

    private SpriteRenderer displayImage; // The image component to update
    private TextMeshProUGUI displayText; // The text object to update

    void Awake()
    {
        // Find child objects
        Transform imageTransform = transform.Find("EnginePuzzleImage");
        Transform textTransform = transform.Find("EnginePuzzleAlgTitle");

        if (imageTransform == null || textTransform == null)
        {
            Debug.LogError("Required child objects 'EnginePuzzleImage' or 'EnginePuzzleAlgTitle' not found!");
            enabled = false;
            return;
        }

        // Get components
        displayImage = imageTransform.GetComponent<SpriteRenderer>();
        displayText = textTransform.GetComponent<TMPro.TextMeshProUGUI>();

        if (displayImage == null || displayText == null)
        {
            Debug.LogError("Required components (Image or TMP_Text) not found on child objects!");
            enabled = false;
            return;
        }

        // Ensure arrays are aligned
        if (images.Length != imageDescriptions.Length)
        {
            Debug.LogError("Images and descriptions arrays must have the same length.");
            enabled = false;
            return;
        }

        // Initialize the display
        UpdateDisplay();

        // Ensure EngineTracker is assigned
        if (engineTracker == null)
        {
            Debug.LogError("EngineTracker is not assigned! Please assign it in the Inspector.");
            enabled = false;
        }

        // Ensure Player Object is assigned
        if (playerObject == null)
        {
            Debug.LogError("Player Object is not assigned! Please assign it in the Inspector.");
            enabled = false;
        }

        // Ensure AudioSource is assigned
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned! Please assign it in the Inspector.");
            enabled = false;
        }

        // Ensure GreenCheck is assigned
        if (greenCheck == null)
        {
            Debug.LogError("GreenCheck is not assigned! Please assign it in the Inspector.");
            enabled = false;
        }

        // Record the launch time
        timeLaunched = Time.time;
        canAnswer = false; // Initially cannot answer
    }

    void OnEnable()
    {
        resetCard();
    }

    void Update()
    {
        // Check if the answer delay has passed
        if (!canAnswer && Time.time >= timeLaunched + answerDelay)
        {
            canAnswer = true;
            //Debug.Log($"Puzzle Card {questionNumber}: Answering enabled.");
        }

        // Handle movement input with delay and only if not already answered
        if (!hasAnswered && Time.time >= nextInputTime)
        {
            if (Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.MoveLeft)))
            {
                CycleLeft();
                nextInputTime = Time.time + inputDelay;
            }
            else if (Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.MoveRight)))
            {
                CycleRight();
                nextInputTime = Time.time + inputDelay;
            }
        }

        // Handle interact input using InputManager for the Interact action and only if not already answered and can answer
        if (!hasAnswered && canAnswer && Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.Interact)))
        {
            CheckAnswer();
        }
    }

    private void CycleLeft()
    {
        currentIndex = (currentIndex - 1 + images.Length) % images.Length; // Wrap around
        UpdateDisplay();
    }

    private void CycleRight()
    {
        currentIndex = (currentIndex + 1) % images.Length; // Wrap around
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        // Update the image and text
        displayImage.sprite = images[currentIndex];
        displayText.text = imageDescriptions[currentIndex];
    }

    private void resetCard()
    {
        currentIndex = 0;
        hasAnswered = false;
        canAnswer = false;
        timeLaunched = Time.time;
        UpdateDisplay();
    }

    private void CheckAnswer()
{
    if (hasAnswered || !canAnswer) return; // Prevent answering multiple times or before the delay

    hasAnswered = true; // Mark this card as answered
    bool isCorrect = (currentIndex == correctAnswerIndex);

    // Log the result
    //Debug.Log($"Question {questionNumber}: Answer is {(isCorrect ? "Correct" : "Incorrect")}");

    // Call the RegisterAnswer method on the EngineTracker
    if (engineTracker != null)
    {
        engineTracker.RegisterAnswer(questionNumber, isCorrect);
    }
    else
    {
        Debug.LogWarning("EngineTracker is not assigned in the inspector.");
    }

    // Disable objects with delay
    if (objectsToDisableOnAnswer != null)
    {
        StartCoroutine(DelayedActions());
    }



    // Play the audio clip
    PlayAudioResource();

    // Activate green check
    ActivateGreenCheck();


}


    private IEnumerator DelayedActions()
    {
        yield return new WaitForSeconds(0.75f);
        // Reactivate player movement
        if (playerObject != null)
        {
            PlayerCharacterController playerMovement = playerObject.GetComponent<PlayerCharacterController>();
            if (playerMovement != null)
            {
                playerMovement.EnableMovement();
                //Debug.Log("Player movement enabled.");
            }
            else
            {
                Debug.LogWarning("Player object assigned, but no PlayerCharacterController script attached.");
            }
        }
        else
        {
            Debug.LogWarning("Player Object is not assigned in the inspector.");
        }
        foreach (var obj in objectsToDisableOnAnswer)
        {
            obj.SetActive(false);
        }
    }

    private void PlayAudioResource()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
            //Debug.Log("Audio clip played.");
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is not assigned.");
        }
    }

    private void ActivateGreenCheck()
    {
        if (greenCheck != null)
        {
            greenCheck.SetActive(true);
            //Debug.Log("Green check activated.");
        }
        else
        {
            Debug.LogWarning("GreenCheck is not assigned.");
        }
    }
}
