using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PandaAttack.Utility
{
    public class InputHandler : Microsoft.Xna.Framework.GameComponent
    {
        #region Keyboard Field Region

        static KeyboardState keyboardState;
        static KeyboardState lastKeyboardState;

        #endregion

        #region Game Pad Field Region

        static GamePadState[] gamePadStates;
        static GamePadState[] lastGamePadStates;

        #endregion

        #region Mouse Region

        static MouseState mouseState;
        static MouseState lastMouseState;

        #endregion

        #region Keyboard Property Region

        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }

        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }

        #endregion

        #region Mouse Property Region

        public static MouseState MouseState
        {
            get { return mouseState; }
        }

        public static MouseState LastMouseState
        {
            get { return lastMouseState; }
        }

        #endregion

        #region Game Pad Property Region

        public static GamePadState[] GamePadStates
        {
            get { return gamePadStates; }
        }

        public static GamePadState[] LastGamePadStates
        {
            get { return lastGamePadStates; }
        }

        #endregion

        #region Constructor Region

        public InputHandler(Game1 game)
            : base(game)
        {
            keyboardState = Keyboard.GetState();

            mouseState = Mouse.GetState();

            gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];
            lastGamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);
        }

        #endregion

        #region XNA methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            lastGamePadStates = (GamePadState[])gamePadStates.Clone();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);

            base.Update(gameTime);
        }

        #endregion

        #region General Method Region

        public static void Flush()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
        }

        #endregion

        #region Keyboard Region

        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) &&
                lastKeyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        #endregion

        #region Mouse Region

        public static bool MouseButtonDown(ButtonState mouseButton)
        {
            return mouseButton == ButtonState.Pressed;
        }

        public static bool MouseButtonPressed(ButtonState previous, ButtonState mouseButton)
        {
            return mouseButton == ButtonState.Released && previous == ButtonState.Pressed;
        }

        public static bool MouseButtonReleased(ButtonState previous, ButtonState mouseButton)
        {
            return mouseButton == ButtonState.Pressed && previous == ButtonState.Released;
        }

        public static Vector2 CurrentMousePosition
        {
            get { return new Vector2(mouseState.X, mouseState.Y); }
        }

        #endregion

        #region Game Pad Region

        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonUp(button) &&
                lastGamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button) &&
                lastGamePadStates[(int)index].IsButtonUp(button);
        }

        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button);
        }

        #endregion
    }
}
