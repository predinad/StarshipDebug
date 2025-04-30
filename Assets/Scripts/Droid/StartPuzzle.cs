using UnityEngine;
using UnityEngine.SceneManagement;



public class StartPuzzle : MonoBehaviour
{

    private bool hasActivated = false;
    [SerializeField] private GameObject mainScene;
    [SerializeField] private GameObject droidScene;

    void OnEnable()
    {
        if (!hasActivated)
        {
            hasActivated = true;

        } else
        {
            LoadPuzzle();

        }

    }

    public void LoadPuzzle()
    {
        mainScene.SetActive(false);
        droidScene.SetActive(true);
    }
}