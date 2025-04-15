using System.Collections.Generic;
using UnityEngine;

/**
 * InputManager is responsible for managing input key bindings for the game.
 * It allows for dynamic key rebinding and stores the key settings using PlayerPrefs.
 */

public enum GameAction { MoveUp, MoveDown, MoveLeft, MoveRight, Interact }

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private Dictionary<GameAction, KeyCode> keyBindings = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadKeyBindings();
    }

    public KeyCode GetKey(GameAction action)
    {
        if (keyBindings.ContainsKey(action))
        {
            return keyBindings[action];
        }
        else
        {
            return GetDefaultKey(action);
        }
    }


    public void SetKey(GameAction action, KeyCode newKey)
    {
        keyBindings[action] = newKey;
        PlayerPrefs.SetString(action.ToString(), newKey.ToString());
        PlayerPrefs.Save();
    }

    private void LoadKeyBindings()
{
    foreach (GameAction action in System.Enum.GetValues(typeof(GameAction)))
    {
        string saved = PlayerPrefs.GetString(action.ToString(), "");
        if (string.IsNullOrEmpty(saved))
        {
            keyBindings[action] = GetDefaultKey(action);
        }
        else if (System.Enum.TryParse(saved, out KeyCode parsedKey))
        {
            keyBindings[action] = parsedKey;
        }
        else
        {
            Debug.LogWarning($"Failed to parse key for {action}. Reverting to default.");
            keyBindings[action] = GetDefaultKey(action);
        }
    }
}


    private KeyCode GetDefaultKey(GameAction action)
    {
        return action switch
        {
            GameAction.MoveUp => KeyCode.W,
            GameAction.MoveDown => KeyCode.S,
            GameAction.MoveLeft => KeyCode.A,
            GameAction.MoveRight => KeyCode.D,
            GameAction.Interact => KeyCode.E,
            _ => KeyCode.None,
        };
    }
}

