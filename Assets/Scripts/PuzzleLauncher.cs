using UnityEngine;

public class PuzzleLauncher : MonoBehaviour
{

    //components of the puzzle to launch
    [SerializeField] private GameObject[] objectsToActivate;

    private void OnTriggerEnter2D(Collider2D other)
    {
                 // Check if the entering object has the Player tag
        if (other.CompareTag("Player"))
        {
            // Activate all specified objects
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }   
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        
        if (objectsToActivate != null)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    Debug.Log("Initially deactivated: " + obj.name);
                }
            }
        }
    }
}
