using UnityEngine;

public class InteractablePuzzle : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;

    public void ActivatePuzzle()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log($"{gameObject.name}: Activated!");
        }
    }
}