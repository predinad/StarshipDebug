using System;
using UnityEngine;

public class StartPuzzleInteractable : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjectsToActivate; // Array of GameObjects to activate when the puzzle starts
    [SerializeField] private GameObject toolTipBar; // Array of GameObjects to deactivate when the puzzle starts
    [SerializeField] private string toolTipText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering object has the Player tag
        if (other.CompareTag("Player") && toolTipBar != null)
        {
            toolTipBar.SetActive(true); // Activate the tooltip bar when the player enters the trigger area
            toolTipBar.GetComponent<TMPro.TextMeshProUGUI>().text = toolTipText; // Set the tooltip text
        }   
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
        if (toolTipBar != null)
        {
            toolTipBar.SetActive(false); // Deactivate the tooltip bar when the player exits the trigger area
        }
    }
}
