using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace TripleS 
{
    /// <summary>
    /// Helper for inputs.
    /// </summary>
    public static class GameInputs {

        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;

        static MouseState currentMouseState;
        static MouseState previousMouseState;

        public static bool TakingInput { get; private set; }
        public static int MaxInputs { get; set; }
        public static InputType InputFilter { get; set; }
        public static string Input { get; private set; }
        public static char[] CustomInputFilter { get; set; }
        public static bool IncludeSpaces { get; set; }

        /// <summary>
        /// Updates keyboard state.
        /// </summary>
        public static void UpdateState()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public static KeyboardState GetKeyboard()
        {
            return currentKeyState;
        }

        public static MouseState GetMouse()
        {
            return currentMouseState;
        }

        /// <summary>
        /// Returns true when key is pressed only once.
        /// </summary>
        public static bool OncePress(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true when left mouse is pressed only once.
        /// </summary>
        public static bool OnceLeftClick(ButtonState state)
        {
            return currentMouseState.LeftButton == state && previousMouseState.LeftButton != state;
        }

        /// <summary>
        /// Returns true when right mouse is pressed only once.
        /// </summary>
        public static bool OnceRightClick(ButtonState state)
        {
            return currentMouseState.RightButton == state && previousMouseState.RightButton != state;
        }

        /// <summary>
        /// Returns true when middle mouse is pressed only once.
        /// </summary>
        public static bool OnceMiddleClick(ButtonState state)
        {
            return currentMouseState.MiddleButton == state && previousMouseState.MiddleButton != state;
        }

        /// <summary>
        /// Updates text inputs (use for events).
        /// </summary>
        public static void TextInputHand(object sender, TextInputEventArgs args)
        {
            if (TakingInput)
            {
                var cha = args.Character;
                if (args.Key == Keys.Back && Input.Length > 0)
                {
                    Input = Input.Remove(Input.Length - 1);
                }
                else if (!char.IsWhiteSpace(cha) && !char.IsSeparator(cha) && !char.IsControl(cha) && !char.IsSymbol(cha))
                {
                    if (Input.Length <= MaxInputs)
                    {
                        bool filter = true;
                        switch (InputFilter)
                        {
                            case InputType.All:
                                filter = true;
                                break;
                            case InputType.Alphabet:
                                filter = char.IsLetter(cha);
                                break;
                            case InputType.Numerals:
                                filter = char.IsNumber(cha);
                                break;
                            case InputType.AlphabetPunc:
                                filter = char.IsLetter(cha) || char.IsPunctuation(cha);
                                break;
                            case InputType.NumeralsPunc:
                                filter = char.IsNumber(cha) || char.IsPunctuation(cha);
                                break;
                            case InputType.AllNoPunc:
                                filter = !char.IsPunctuation(cha);
                                break;
                            case InputType.Custom:
                                if (CustomInputFilter != null) {
                                    foreach (char custom in CustomInputFilter) {
                                        filter = cha == custom;
                                        if (filter)
                                            break;
                                    }
                                }
                                break;
                        }

                        if (filter)
                            Input += cha;
                    }
                }
                else if (args.Key == Keys.Space && IncludeSpaces)
                {
                    Input += ' ';
                }
            }
            else
                Input = "";
        }

        public static void ResetInput()
        {
            Input = "";
        }

        public static void ToggleInputs(bool state)
        {
            if (state != TakingInput)
            {
                ResetInput();
                TakingInput = state;
            }
        }
    }

    public enum InputType
    {
        All,
        AllNoPunc,
        Alphabet,
        Numerals,
        AlphabetPunc,
        NumeralsPunc,
        Custom
    }
}