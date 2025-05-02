using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject initialText;
    [SerializeField] public GameObject victoryScreen;

    [SerializeField] private GameObject mainScene;
    [SerializeField] private GameObject droidScene;

    [SerializeField] private GameObject trigger;

    [Header("Main Tracker")]
    [SerializeField] private TaskManager taskManager; // Reference to the the global quest tracker
    
    [Header("ReactorPuzzle Launcher(to disable on victory)")]
    [SerializeField] private GameObject reactorPuzzle; // Reference to the the reactorPuzzle launcher

    private void Awake()
    {
        if (taskManager == null)
        {
            Debug.LogError("taskManager is not assigned to UIManager! Please assign it in the Inspector.");
            enabled = false;
        }
        if (reactorPuzzle == null)
        {
            Debug.LogError("reactorPuzzle is not assigned to EUIManager! Please assign it in the Inspector.");
            enabled = false;
        }

    }

    public void HideInitialText()
    {
        initialText.SetActive(false);
    }

    public void closePuzzle()
    {
        mainScene.SetActive(true);
        droidScene.SetActive(false);
        trigger.SetActive(false);

    }

    public void NextLevel()
    {
        victoryScreen.SetActive(false);

        closePuzzle();

        //checks off the quest in the main tracker and disables icon for the reacorPuzzle
        if (taskManager != null)
        {
            taskManager.CompleteTask("Fix Reactor");
	    	taskManager.CheckIfAllComplete();
        }

	 	if (reactorPuzzle != null)
	 	{
	 		reactorPuzzle.gameObject.SetActive(false);
	 	}

    }

    public void showVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }
}
