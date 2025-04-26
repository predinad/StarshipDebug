using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

// <summary>
// This script handles the flipping through of UI panels
// It starts with the first panel and allows the user to navigate through the panels linearly forward and backward
// It ensures to disable player movement and reenable it when the UI is closed
// </summary>
public class NavigationUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private bool playerCanUseKeysToFlipPanels = true; // Flag to check if the player can use keys to flip panels. Turn off in the inspector if you want to disable this feature.
    [SerializeField] private GameObject toolTipBar;
    private string toolTipText;
    private bool playerInTrigger = false;
    private int currentPanelIndex = -1;
    private bool hasJustOpened = false;

    private void Start()
    {
        DeactivateAllPanels(); // Ensure all panels are off initially
        toolTipText = $"Press ({InputManager.Instance.GetKey(GameAction.Interact)}) to interact"; // Set the tooltip text
    }

    void Update()
    {
        if (playerInTrigger && currentPanelIndex == -1 && Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.Interact)))
        {
            ShowFirstPanel(); // Show the first panel if the player presses the interact key
            hasJustOpened = true;
            return; // Exit Update to avoid closing right after opening
        }

        if (Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.Interact)))
        {
            if (hasJustOpened)
            {
                hasJustOpened = false; // Ignore this key press and reset the flag
            }
            else
            {
                CloseUI();
            }
        }

        if (currentPanelIndex != -1 && Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.Interact)))
        {
            CloseUI(); // Close the UI if the player presses the interact key
        }

        if (playerCanUseKeysToFlipPanels && Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.MoveRight)) && currentPanelIndex != -1)
        {
            ShowNextPanel(); // Show the next panel if the player presses the next key
        }

        if (playerCanUseKeysToFlipPanels && Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.MoveLeft)) && currentPanelIndex != -1)
        {
            ShowPreviousPanel(); // Show the previous panel if the player presses the previous key
        }
    }

    public void CloseUI()
    {
        DeactivateAllPanels();
        currentPanelIndex = -1;
        GameObject.Find("Player").GetComponent<PlayerCharacterController>().EnableMovement(); // Enable player movement when UI is closed
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
            GameObject.Find("Player").GetComponent<PlayerCharacterController>().DisableMovement(); // Enable player movement when UI is closed
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
            CloseUI(); // Close the UI if the player is on the last panel and presses the next key
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
            CloseUI(); // Close the UI if the player is on the first panel and presses the previous key
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering object has the Player tag
        if (other.CompareTag("Player"))
        {
            if (toolTipBar != null)
            {
                toolTipBar.SetActive(true); // Activate the tooltip bar when the player enters the trigger area
                toolTipBar.GetComponent<TMPro.TextMeshProUGUI>().text = toolTipText; // Set the tooltip text
            }
            playerInTrigger = true; // Set the flag to indicate the player is in the trigger area
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (toolTipBar != null)
            {
                toolTipBar.SetActive(false); // Deactivate the tooltip bar when the player exits the trigger area
            }
            playerInTrigger = false; // Reset the flag to indicate the player is no longer in the trigger area
        }
    }
}
