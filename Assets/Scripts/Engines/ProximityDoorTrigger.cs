using UnityEngine;

public class ProximityDoorTrigger : MonoBehaviour
{
    private AutoDoor parentDoorScript;
    private string playerTag = "Player"; // Set this tag on your player GameObject

    void Start()
    {
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            Debug.Log($"Trigger's parent is: {parentTransform.name}");
            parentDoorScript = parentTransform.GetComponent<AutoDoor>();
            if (parentDoorScript == null)
            {
                Debug.LogError($"Parent GameObject '{parentTransform.name}' has no AutoDoor script attached!");
                enabled = false;
            }
        }
        else
        {
            Debug.LogError("This Trigger Object has no parent!");
            enabled = false;
        }
    }
    void OnTriggerEnter(Collider other) // For 3D colliders
    {
        if (parentDoorScript != null && other.CompareTag(playerTag))
        {
            parentDoorScript.Invoke("Open", 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other) // For 2D colliders
    {
        if (parentDoorScript != null && other.CompareTag(playerTag))
        {
            parentDoorScript.Invoke("Open", 0f);
        }
    }

    void OnTriggerExit(Collider other) // For 3D colliders
    {
        if (parentDoorScript != null && other.CompareTag(playerTag))
        {
            parentDoorScript.Invoke("Close", 0f);
        }
    }

    void OnTriggerExit2D(Collider2D other) // For 2D colliders
    {
        if (parentDoorScript != null && other.CompareTag(playerTag))
        {
            parentDoorScript.Invoke("Close", 0f);
        }
    }
}