//using TreeEditor;
using UnityEngine;

public class Resetter : MonoBehaviour
{
    private Vector3 initialPosition;
    private bool initialized = false;

    void Start()
    {
        initialPosition = transform.position;
        initialized = true;
    }

    public void ResetPosition()
    {
        Debug.Log(initialPosition);
        if (initialized)
        {
            transform.position = initialPosition;
        }
        gameObject.SetActive(true);
    }
}