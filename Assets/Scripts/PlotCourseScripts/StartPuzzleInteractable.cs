using System;
using UnityEngine;

public class StartPuzzleInteractable : MonoBehaviour
{
    [SerializeField] NavigationUIHandler navigationUIHandler; // Reference to the NavigationUIHandler script
    [SerializeField] private GameObject toolTipBar; // Array of GameObjects to deactivate when the puzzle starts
    [SerializeField] private string toolTipText;
    private bool playerInTrigger = false;

    private void Update()
    {
        // Check if the player is in the trigger area and the "E" key is pressed
        if (playerInTrigger && Input.GetKey(InputManager.Instance.GetKey(GameAction.Interact))) // Check if the player is in the trigger area and the interact key is pressed
        {
            // Call the method to start the puzzle
            navigationUIHandler.ShowFirstPanel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering object has the Player tag
        if (other.CompareTag("Player") && toolTipBar != null)
        {
            toolTipBar.SetActive(true); // Activate the tooltip bar when the player enters the trigger area
            toolTipBar.GetComponent<TMPro.TextMeshProUGUI>().text = toolTipText; // Set the tooltip text
            playerInTrigger = true; // Set the flag to indicate the player is in the trigger area
        }   
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (toolTipBar != null)
        {
            toolTipBar.SetActive(false); // Deactivate the tooltip bar when the player exits the trigger area
            playerInTrigger = false; // Reset the flag to indicate the player is no longer in the trigger area
        }
    }
}
