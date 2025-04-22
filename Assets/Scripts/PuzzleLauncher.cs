using UnityEngine;

public class PuzzleLauncher : MonoBehaviour
{
    [Header("Puzzle Components")]
    [SerializeField] private GameObject[] objectsToActivate;

    [Header("EngineTracker for reset on")]
    [SerializeField] private EngineTracker engineTracker; // Reference to EngineTracker
    private bool solved=false;

    private void Start()
    {
        if (engineTracker == null)
        {
            Debug.LogError("EngineTracker is not assigned! Please assign it in the Inspector.");
            return;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !solved)
        {
            SetObjectsActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !solved)
        {
            SetObjectsActive(false);
            ResetTracker(); //reset when the player leaves
        }
    }

    private void SetObjectsActive(bool isActive)
    {
        if (objectsToActivate != null)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(isActive);
                }
            }
        }
    }

    public void ResetTracker()
    {
        if (engineTracker != null)
        {
            engineTracker.Reset();
            Debug.Log("EngineTracker Reset by Launcher.");
        }
    }

    public void setSolved(){
        solved=true;
    }
}