using System;
using TripleS.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TripleS {

    /// <summary>
    /// Debug input console.
    /// </summary>
    public static class DebugConsole
    {
        static string command;

        public static SpriteFont DebugFont { get; set; }
        public static bool Active { get; set; }
        public static Keys ToggleInput { get; set; }
        public static Keys EnterInput { get; set; }
        public static string LastCommand { get; private set; }
        public static int FontSize { get; set; }

        public static event EventHandler<CommandEventArgs> OnCommand;

        public static void OpenConsole()
        {
            Active = true;
            GameInputs.IncludeSpaces = true;
            GameInputs.MaxInputs = 32;
            GameInputs.InputFilter = InputType.All;
            GameInputs.ToggleInputs(true);
            command = "";
        }

        public static void CloseConsole()
        {
            Active = false;
            GameInputs.ToggleInputs(false);
        }

        public static void Update()
        {
            if (Active)
            {
                if (!GameInputs.TakingInput)
                    GameInputs.ToggleInputs(true);

                command = GameInputs.Input;

                if (GameInputs.OncePress(EnterInput))
                {
                    LastCommand = command;
                    CommandEventArgs args = new CommandEventArgs();
                    args.Command = LastCommand;
                    OnCommand(null, args);
                    CloseConsole();
                }

                if (GameInputs.OncePress(ToggleInput))
                    CloseConsole();
            }
            else
            {
                if (GameInputs.OncePress(ToggleInput))
                {
                    Active = false;
                    OpenConsole();
                }
            }
        }

        public static void Draw(Renderer renderer)
        {
            if (Active && DebugFont != null)
            {
                Vector2 pos = renderer.View.Position + new Vector2(5, 5);
                renderer.DrawText(DebugFont, command, pos, 0, 0, col: Color.Cyan, size: (float)(0.8f / FontSize));
            }
        }
    }

    public class CommandEventArgs : EventArgs
    {
        public string Command { get; set; }
    }
}
