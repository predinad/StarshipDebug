using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject initialText;
    [SerializeField] public GameObject victoryScreen;

    [SerializeField] private GameObject mainScene;
    [SerializeField] private GameObject droidScene;

    public void HideInitialText()
    {
        initialText.SetActive(false);
    }

    public void NextLevel()
    {
        victoryScreen.SetActive(false);

        mainScene.SetActive(true);
        droidScene.SetActive(false);
    }

    public void showVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }
}
