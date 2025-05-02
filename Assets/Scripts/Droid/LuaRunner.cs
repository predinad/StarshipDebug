using UnityEngine;
using MoonSharp.Interpreter;
using TMPro;
using System.Threading;
using System.Collections.Concurrent;

public class LuaScriptRunner : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public DroidController droid;
    [SerializeField] public TMP_Text errorMessage;
    public Script script;

    private Thread luaThread;

    private void Start()
    {
        UserData.RegisterAssembly();

    }

    public void RunCode()
    {
        errorMessage.text = "";
        string code = codeInputField.text;

        if (luaThread != null)
        {
            ResetCode();
        }

        script = new Script();
        script.Globals["robot"] = droid;

        luaThread = new Thread(() => {
            try
            {
                script.DoString(code);
            }
            catch (ScriptRuntimeException ex)
            {
                Debug.Log(ex.DecoratedMessage);
                droid.errors.Enqueue(ex.DecoratedMessage);
            }
            catch (SyntaxErrorException ex)
            {
                Debug.Log(ex.DecoratedMessage);
                droid.errors.Enqueue(ex.DecoratedMessage);
            }
        });
        luaThread.Start();
    }

    public void ResetCode()
    {
        if (luaThread != null && luaThread.IsAlive)
            luaThread.Abort();
        luaThread = null;
        droid.ResetAll();
    }
}
