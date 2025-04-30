using UnityEngine;
using MoonSharp.Interpreter;
using TMPro;
using System.Threading;

public class LuaScriptRunner : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public DroidController droid;
    public Script script;
    private Thread luaThread;

    private void Start()
    {
        UserData.RegisterAssembly();

    }

    public void RunCode()
    {
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
                Debug.LogError("Lua Error: " + ex.DecoratedMessage);
            }
        });
        luaThread.Start();
    }

    public void ResetCode()
    {
        if(luaThread.IsAlive)
            luaThread.Abort();
        luaThread = null;
        droid.ResetAll();
    }
}
