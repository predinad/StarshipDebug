using System.Collections;
using TMPro;
using UnityEngine;

/**
 * KeyBindButton is responsible for handling the key rebinding UI element.
 * It allows the player to rebind keys for specific game actions.
 */

public class KeyBindButton : MonoBehaviour
{
    public GameAction actionToRebind;
    public TMP_Text displayText;

    public void OnClickRebind()
    {
        StartCoroutine(WaitForKeyPress());
    }

    private IEnumerator WaitForKeyPress()
    {
        displayText.text = "...";
        bool keyAssigned = false;

        while (!keyAssigned)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    InputManager.Instance.SetKey(actionToRebind, key);
                    displayText.text = key.ToString();
                    keyAssigned = true;
                    break;
                }
            }
            yield return null;
        }
    }

    private void Start()
    {
        displayText.text = InputManager.Instance.GetKey(actionToRebind).ToString();
    }
}
