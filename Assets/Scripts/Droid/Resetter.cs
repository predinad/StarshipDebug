//using TreeEditor;
using UnityEngine;

public class Resetter : MonoBehaviour
{
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        gameObject.SetActive(true);
    }
}