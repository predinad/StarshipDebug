using UnityEngine;

public class NavigationUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    private int currentPanelIndex = -1;

    private void Start()
    {
        DeactivateAllPanels(); // Ensure all panels are off initially
    }

    private void DeactivateAllPanels()
    {
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            canvas.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ShowFirstPanel()
    {
        DeactivateAllPanels();
        if (canvas.transform.childCount > 0)
        {
            currentPanelIndex = 0;
            canvas.transform.GetChild(currentPanelIndex).gameObject.SetActive(true);
        }
    }

    public void ShowNextPanel()
    {
        if (currentPanelIndex < canvas.transform.childCount - 1)
        {
            canvas.transform.GetChild(currentPanelIndex).gameObject.SetActive(false);
            currentPanelIndex++;
            canvas.transform.GetChild(currentPanelIndex).gameObject.SetActive(true);
        }
        else if (currentPanelIndex >= canvas.transform.childCount - 1)
        {
            canvas.transform.GetChild(currentPanelIndex).gameObject.SetActive(false);
            currentPanelIndex = -1; // Reset to -1 to indicate no panel is active
        }
    }

    public void ShowPreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            canvas.transform.GetChild(currentPanelIndex).gameObject.SetActive(false);
            currentPanelIndex--;
            canvas.transform.GetChild(currentPanelIndex).gameObject.SetActive(true);
        }
        else if (currentPanelIndex == 0)
        {
            canvas.transform.GetChild(currentPanelIndex).gameObject.SetActive(false);
            currentPanelIndex = -1; // Reset to -1 to indicate no panel is active
        }
    }
}
