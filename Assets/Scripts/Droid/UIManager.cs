using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject initialText;
    [SerializeField] public GameObject victoryScreen;
    [SerializeField] public TMP_Text victoryMessage;

    [SerializeField] private GameObject mainScene;
    [SerializeField] private GameObject droidScene;

    [SerializeField] private GameObject trigger;

    [SerializeField] private LuaScriptRunner luaRunner;

    [SerializeField] private DroidController droid;

    [SerializeField] private GameObject level1;
    [SerializeField] private GameObject level2;
    [SerializeField] private GameObject level3;

    [SerializeField] private DroidSpawn level1Spawn;
    [SerializeField] private DroidSpawn level2Spawn;
    [SerializeField] private DroidSpawn level3Spawn;

    [Header("Main Tracker")]
    [SerializeField] private TaskManager taskManager; // Reference to the the global quest tracker
    
    [Header("ReactorPuzzle Launcher(to disable on victory)")]
    [SerializeField] private GameObject reactorPuzzle; // Reference to the the reactorPuzzle launcher

    private int level = 1;

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
        luaRunner.ResetCode();
        initialText.SetActive(false);
    }

    public void closePuzzle()
    {
        luaRunner.ResetCode();
        mainScene.SetActive(true);
        droidScene.SetActive(false);
        trigger.SetActive(false);

    }

    public void NextLevel()
    {
        victoryScreen.SetActive(false);
        level += 1;
        Debug.Log(level);

        switch (level) {
            case 2:
                level1.SetActive(false);
                level2.SetActive(true);
                droid.setSpawn(level2Spawn);
                luaRunner.ResetCode();
                victoryMessage.text = "You Fixed the Reactor!\n There's one more level, but it's entirely optional";
                break;

            case 3:
                level2.SetActive(false);
                level3.SetActive(true);
                droid.setSpawn(level3Spawn);
                luaRunner.ResetCode();

                //checks off the quest in the main tracker and disables icon for the reactorPuzzle
                if (taskManager != null)
                {
                    taskManager.CompleteTask("Fix Reactor");
                    taskManager.CheckIfAllComplete();
                }
             


                victoryMessage.text = "Congratulations!\n You beat the bonus level!";
                break;

            case 4:
                luaRunner.ResetCode();
                closePuzzle();
                if (reactorPuzzle != null)
                {
                    reactorPuzzle.gameObject.SetActive(false);
                }
                break;

        }
    }

    public void showVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }
}
