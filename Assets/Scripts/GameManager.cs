using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    /// Structure for UI panel entries
    /// Each entry contains a name and a reference to the GameObject representing the panel.
    [System.Serializable]
    public struct PanelEntry
    {
        public string panelName;
        public GameObject panelObject;
    }

    /// Structure for task UI
    /// Each task UI contains a reference to the GameObject and a TextMeshProUGUI component for displaying text.
    [System.Serializable]
    public struct TaskUI
    {
        public GameObject taskUIObject;
        public TextMeshProUGUI showTaskButtonText;
    }
    public TaskUI taskUI;

    // Scene names for each scene
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [SerializeField] private string gameSceneName = "MainScene";

    // List of UI panels to be managed.
    [SerializeField] private List<PanelEntry> uiPanelEntries;

    /// Dictionary to hold references to UI panels.
    /// The key is the panel name, and the value is the GameObject reference.
    private Dictionary<string, GameObject> uiPanels = new();

    void Awake()
    {
        // Build dictionary from the serialized list
        foreach (var entry in uiPanelEntries)
        {
            if (!uiPanels.ContainsKey(entry.panelName) && entry.panelObject != null)
            {
                uiPanels.Add(entry.panelName, entry.panelObject);
            }
        }
    }

    void Update()
    {
        // Pause and resume the game using the Escape key
        if (IsPanelActive("PausePanel") && Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
        else if (!IsPanelActive("PausePanel") && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        LoadScene(gameSceneName);
    } 

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        LoadScene(mainMenuSceneName);
    }

    public void OpenSettings()
    {
        DisableAllPanels();
        ShowPanel("SettingsPanel");
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        DisableAllPanels();
        ShowPanel("PausePanel");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        DisableAllPanels();
        ShowPanel("GamePanel");
    }
    
    public void ShowMainMenu() 
    {
        DisableAllPanels();
        ShowPanel("MainMenuPanel");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ToggleShowingTaskUI()
    {
        if (taskUI.taskUIObject != null && taskUI.showTaskButtonText != null)
        {
            if(taskUI.taskUIObject.activeSelf)
            {
                taskUI.taskUIObject.SetActive(false);
                taskUI.showTaskButtonText.text = "Show Task List +";
            }
            else
            {
                taskUI.taskUIObject.SetActive(true);
                taskUI.showTaskButtonText.text = "Hide Task List -";
            }
        }
        else
        {
            Debug.LogWarning("Task UI is not assigned in the GameManager.");
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void ShowPanel(string name)
    {
        if (uiPanels.TryGetValue(name, out var panel)) panel.SetActive(true);
    }

    private void HidePanel(string name)
    {
        if (uiPanels.TryGetValue(name, out var panel)) panel.SetActive(false);
    }

    private bool IsPanelActive(string name)
    {
        return uiPanels.TryGetValue(name, out var panel) && panel.activeSelf;
    }

    private void DisableAllPanels()
    {
        foreach (var entry in uiPanels)
        {
            if(entry.Value != null && entry.Value.activeSelf)
            {
                entry.Value.SetActive(false);
            }
        }
    }
}
