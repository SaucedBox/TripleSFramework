using System.IO;
using NLua;

namespace TripleS.Scripting {

    /// <summary>
    /// Manages script on current map.
    /// </summary>
    public static class ScriptManager {

        private static Lua state;
        private static string script;

        public static void Init(string scriptDirectory, int scriptID)
        {
            string name = (string)ParamLoader.GetParam("script", scriptID, "location");
            string globalContents = "";

            for (int i = 0; i < 2; i++)
            {
                var n = name;
                if (i == 0)
                    n = "global";

                using (FileStream fs = File.OpenRead($"{scriptDirectory}/{n}.lua"))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        var s = sr.ReadToEnd();
                        if (i == 1)
                            script = s;
                        else
                            globalContents = s;
                    }
                }
            }

            state = new Lua();
            state.LoadCLRPackage();
            state.DoString(globalContents);
            state.DoString(script);
            ExecuteFunction("Init");
        }

        /// <summary>
        /// Finds local variable in map script.
        /// </summary>
        /// <param name="name">Param id of script</param>
        /// <returns>Value of variable</returns>
        /// <exception cref="ScriptExecption">If variable was not found</exception>
        public static object GetNonLocalVar(string name)
        {
            var res = state[name];
            if (res == null)
                throw new ScriptExecption($"Variable of {name} was not found.");
            return res;
        }

        /// <summary>
        /// Runs Lua script function.
        /// </summary>
        /// <param name="function">Name of function</param>
        /// <exception cref="ScriptExecption">If function was not found</exception>
        public static void ExecuteFunction(string function)
        {
            LuaFunction func = state[function] as LuaFunction;
            if (func == null)
                throw new ScriptExecption($"Function of {function} was not found.");
            func.Call();
        }
    }
}
