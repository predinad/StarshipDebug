using UnityEngine;
using System.Collections;
using TMPro;

public class EngineTracker : MonoBehaviour
{
    [Header("attach puzzlearea")]
    [SerializeField] private PuzzleLauncher puzzleArea; // Explicitly assign the puzzle area
    [SerializeField] private int size = 6; // Serialized field for customization in Inspector

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI statusText; //displays x / size of the puzzles solved
    [SerializeField] private GameObject reviewBackground; //graphic background in case the display text is hard to read, currently not implemented
    [SerializeField] private TextMeshProUGUI reviewText; //a text panel to display the results of the engine puzzles
    [SerializeField] private GameObject solvedScreen; //just a simple graphic to take the place of the x / size when engines are online. could be replaced by a series of objects to activate

    [Header("Engine Tracker")]
    [SerializeField] private TaskManager taskManager; // Reference to the the global quest tracker
    [Header("Sound Effects")]
    [SerializeField] private AudioSource enginePuzzleAudioSource;

     [Header("Player object")]
    [SerializeField] private GameObject playerObject; // Explicitly assign the player object

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
        if (taskManager == null)
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
        // Ensure Player Object is assigned
        if (playerObject == null)
        {
            Debug.LogError("Player Object is not assigned! Please assign it in the Inspector.");
            enabled = false;
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
        //Debug.LogError($"Attempt made on puzzle index number: {index}.");
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

        //if all questions answered, process results
            if (answeredCount == size)
            {
                EvaluateAnswers();
                StartCoroutine(CoRoutineFinalReport()); 
            }

        }
    }


    private void EvaluateAnswers()
    {
        //Debug.Log("EvaluateAnwers called.");
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
        reviewText.text = "";
        if (reviewBackground != null)
        {
            reviewBackground.gameObject.SetActive(false);
        }
        solved = false;
    }

    private void UpdateStatusText()
    {
        statusText.text = $"{answeredCount}/{answerArray.Length}";
    }

    public bool IsPuzzleSolved()
    {
        return solved;
    }

    private void OnPuzzleSolved()
    {
        //Debug.Log("Onpuzzlesolved called.");
        // Disable the status text
        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
        }

        if (taskManager != null)
        {
            taskManager.CompleteTask("Tune Engines");
        }



    }


    private IEnumerator CoRoutineFinalReport()
    {
        DisablePlayerMovement();
        yield return new WaitForSeconds(1.5f);
        //load visual card
        
        if (reviewBackground != null)
        {
            reviewBackground.gameObject.SetActive(true);
        }
        if (reviewText != null)
        {
            reviewText.gameObject.SetActive(true); // Activate reviewText here
            
        }  
        DisablePlayerMovement();
        reviewText.text = "Task: Tune  Engines";//reset final report text   
        reviewText.text = reviewText.text+"\nAll Engine Alerts Addressed";
        PlayEngineSound(successSound); // Play sound for line 1


        yield return new WaitForSeconds(1.2f);
        reviewText.text = reviewText.text + "\nCorrect Approaches: " + correctCount + " / " + size + ".";
        PlayEngineSound(successSound); // Play sound for line 2
        yield return new WaitForSeconds(1.2f);
        if (correctCount == size)
        {
            reviewText.text = reviewText.text + "\nEngines Online!";
            PlayEngineSound(puzzleCompleteSound); // Play sound for line 3
            // Enable the solved object
            if (solvedScreen != null)
            {
                solvedScreen.SetActive(true);
            }
            puzzleArea.setSolved();

        }
        else
        {
            reviewText.text = reviewText.text + "\nTry Again Later.";
            PlayEngineSound(failureSound); // Play sound for line 3
        }

        yield return new WaitForSeconds(1.6f);
        reviewBackground.gameObject.SetActive(false);
        //animations and sounds are done playing now



        EnablePlayerMovement();

    }

    private void DisablePlayerMovement()
    {
        // Deactivate player movement
        if (playerObject != null)
            {
                PlayerCharacterController playerMovement = playerObject.GetComponent<PlayerCharacterController>();
                if (playerMovement != null)
                {
                playerMovement.DisableMovement();
                    //Debug.Log("Player movement disabled.");
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
    }
    private void EnablePlayerMovement()
    {
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
    }

    private void PlayEngineSound(AudioClip audio)
    {
        if (enginePuzzleAudioSource != null)
        {
            enginePuzzleAudioSource.PlayOneShot(audio);
        }
        else
        {
            Debug.LogWarning("enginePuzzleAudioSource is not assigned.  No sound will play.");
        }
    }

}
