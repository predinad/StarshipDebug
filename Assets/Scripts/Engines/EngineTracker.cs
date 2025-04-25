using UnityEngine;
using System.Collections;
using TMPro;

public class EngineTracker : MonoBehaviour
{
    [Header("attach puzzlearea")]
    [SerializeField] private PuzzleLauncher puzzleArea; // Explicitly assign the puzzle area
    [SerializeField] private int size = 6; // Serialized field for customization in Inspector

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI reviewText;
    [SerializeField] private GameObject solvedScreen;   
     
    [Header("Engine Tracker")]
    [SerializeField] private TaskManager taskManager; // Reference to the the global quest tracker
    [Header("Sound Effects")]
    [SerializeField] private AudioSource enginePuzzleAudioSource;
    private AudioClip successSound;
    private AudioClip failureSound;    
    private AudioClip puzzleCompleteSound;
    private AudioClip enginePowerUpSound;
    private bool[] answerArray;
    private bool[] correctArray;
    private int answeredCount = 0;
    private int correctCount = 0;
    private bool solved = false;

    private void Awake()
    {
        if (taskManager== null)
        {
            Debug.LogError("EventSystem is not assigned to EngineTracker! Please assign it in the Inspector.");
            enabled = false;
        }
        // Ensure EngineTracker is assigned
        if (puzzleArea == null)
        {
            Debug.LogError("PuzzleArea is not assigned to EngineTracker! Please assign it in the Inspector.");
            enabled = false;
        }
        if (statusText == null)
        {
            Debug.LogWarning("StatusText is not assigned to EngineTracker. Text updates will not be displayed.");
        }
        LoadAudioClips(); // Load audio clips in Awake
        answerArray = new bool[size];
        correctArray = new bool[size];
        Reset();

    }
        private void LoadAudioClips()
    {
        enginePowerUpSound = Resources.Load<AudioClip>("enginePowerUp"); // Load from Resources folder
        puzzleCompleteSound = Resources.Load<AudioClip>("transferOfDataIsComplete"); // Load from Resources folder
        successSound = Resources.Load<AudioClip>("engineFeedback2"); // Load from Resources folder
        failureSound = Resources.Load<AudioClip>("error"); // Load from Resources folder

        if (successSound == null)
        {
            Debug.LogError("Failed to load successSound from Resources folder.  Make sure the file is in a folder named Resources.");
        }
        if (failureSound == null)
        {
            Debug.LogError("Failed to load failureSound from Resources folder. Make sure the file is in a folder named Resources.");
        }
    }

    public void RegisterAnswer(int index, bool isCorrect)
    {
        Debug.LogError($"Attempt made on puzzle index number: {index}.");
        if (index < 0 || index >= answerArray.Length)
        {
            Debug.LogError($"Invalid index {index}. Must be between 0 and {answerArray.Length - 1}.");
            return;
        }

        if (!answerArray[index]) // Only register if not already answered
        {
            answerArray[index] = true;
            correctArray[index] = isCorrect;
            answeredCount++;
            UpdateStatusText();
            if (answeredCount == answerArray.Length)
            {
                EvaluateAnswers();
            }

        }
    }

    private void EvaluateAnswers()
    {
        Debug.Log("EvaluateAnwers called.");
        correctCount = 0;

        foreach (bool isCorrect in correctArray)
        {
            if (isCorrect)
            {
                correctCount++;
            }
        }

        solved = (correctCount == correctArray.Length);
        if (solved)
        {
            OnPuzzleSolved();
        }
    }

    public void Reset()
    {
        for (int i = 0; i < answerArray.Length; i++)
        {
            answerArray[i] = false;
            correctArray[i] = false;
        }

        answeredCount = 0;
        correctCount = 0;
        UpdateStatusText();
        solved = false;
    }

    private void UpdateStatusText()
    {
            statusText.text = $"{answeredCount} / {answerArray.Length}";
    }

    public bool IsPuzzleSolved()
    {
        return solved;
    }

    private void OnPuzzleSolved()
    {
                    Debug.Log("Onpuzzlesolved called.");
        // Disable the status text
        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
        }

        if (taskManager != null)
        {
            taskManager.CompleteTask("Tune Engines");
        }

        // Enable the solved object
        if (solvedScreen != null)
        {
            solvedScreen.SetActive(true);
        }
        puzzleArea.setSolved();
        Debug.Log("Puzzle solved! Trigger success actions here.");
        // Implement additional logic for when the puzzle is solved
    }
    private void UpdateReviewText()
    {
        if (reviewText != null)
        {
            reviewText.gameObject.SetActive(true); // Activate reviewText here
            StartCoroutine(ShowTextWithDelays());
        }
    }

    private IEnumerator ShowTextWithDelays()
    {

        reviewText.text = "All Engine Alerts Addressed";
        PlayEngineSound(); // Play sound for line 1
        yield return new WaitForSeconds(0.8f);
        reviewText.text = reviewText.text+"\nCorrect Approaches: "+correctCount+" / "+size+".";
        PlayEngineSound(); // Play sound for line 2
        yield return new WaitForSeconds(1.4f);
        if(correctCount==size)
        {
            reviewText.text = reviewText.text+"\nEngines Online!";
            PlayEngineSound(); // Play sound for line 3   
        }
        else
        {
            reviewText.text = reviewText.text+"\nTry Again!";
            PlayEngineSound(); // Play sound for line 3
        }
     

    }

    private void PlayEngineSound()
    {
        if (enginePuzzleAudioSource != null)
        {
            enginePuzzleAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("enginePuzzleAudioSource is not assigned.  No sound will play.");
        }
    }

}