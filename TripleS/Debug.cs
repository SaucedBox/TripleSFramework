using System;
using TripleS.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TripleS {

    /// <summary>
    /// Helper class for debuging.
    /// </summary>
    public static class Debuger {

        public static SpriteFont DebugFont { get; set; }

        /// <summary>
        /// Prints message to VS output console.
        /// </summary>
        public static void Print(object message)
        {
            System.Diagnostics.Debug.Print($"{message}");
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        public static void DrawLine(Renderer renderer, Line line, Texture2D texture, Color color)
        {
            for (int i = (int)line.GetLeast().X; i <= (int)line.GetMost().X; i++)
            {
                Vector2 drawPos = new Vector2(i, line.GetMost().Y);
                float y = CollisionEngine.InterceptLineFundemental(line, drawPos);
                drawPos.Y = y;
                renderer.BasicDraw(texture, drawPos, 2, 0, col: color);
            }
        }

        /// <summary>
        /// Draws a line with bounds and other markings.
        /// </summary>
        public static void DrawDebugLine(Renderer renderer, Line line, Texture2D texture)
        {
            renderer.BasicDraw(texture, line.Start, 0, 0, col: new Color(0.3f, 0, 0));
            renderer.BasicDraw(texture, line.End, 0, 0, col: new Color(0.3f, 0, 0));
            renderer.BasicDraw(texture, line.Second, 0, 0, col: Color.Pink);
            renderer.BasicDraw(texture, line.Third, 0, 0, col: Color.Pink);
            DrawLine(renderer, line, texture, Color.Red);
            renderer.BasicDraw(texture, line.Bounds, 0, 0, col: new Color(0.2f, 0, 0, 0.2f));
        }

        /// <summary>
        /// Draws text on screen.
        /// </summary>
        public static void DrawDebugText(Renderer ren, Vector2 pos, Color color, string text, float fontSize)
        {
            ren.DrawText(DebugFont, text, pos, 0, 0, 0.5f / fontSize, col: color);
        }

        /// <summary>
        /// Draws a pixel on screen.
        /// </summary>
        public static void DrawDebugSquare(Renderer ren, Vector2 pos, Color color, float scale)
        {
            ren.BasicDraw(SSS.Square, pos, 0, 0, scale, col: color);
        }

        /// <summary>
        /// Draws a solid rectangle on screen.
        /// </summary>
        public static void DrawDebugRect(Renderer ren, Rectangle pos, Color color, float scale)
        {
            ren.BasicDraw(SSS.Square, pos, 0, 0, scale, col: color);
        }
    }

    /// <summary>
    /// Debug input console.
    /// </summary>
    public static class DebugConsole
    {
        static string command;

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
            if (Active && Debuger.DebugFont != null)
            {
                Vector2 pos = renderer.View.Position + new Vector2(5, 5);
                renderer.DrawText(Debuger.DebugFont, command, pos, 0, 0, col: Color.Cyan, size: (float)(0.8f / FontSize));
            }
        }
    }

    public class CommandEventArgs : EventArgs
    {
        public string Command { get; set; }
    }
}
