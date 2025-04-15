using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct PanelEntry
    {
        public string panelName;
        public GameObject panelObject;
    }

    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [SerializeField] private string gameSceneName = "MainScene";
    [SerializeField] private List<PanelEntry> uiPanelEntries;

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
