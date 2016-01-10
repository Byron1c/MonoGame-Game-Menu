#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using MenuManager.DrawingSystem;

namespace MenuManager.ScreenSystem
{

    /// <summary>
    ///   an enum of all available mouse buttons.
    /// </summary>
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton,
        ExtraButton1,
        ExtraButton2
    }


    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;
        public readonly MouseState[] CurrentMouseStates;
        private GamePadState _currentVirtualState;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;
        public readonly MouseState[] LastMouseStates;
        private GamePadState _lastVirtualState;

        public readonly bool[] GamePadWasConnected;

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        private Vector2 _cursor;
        private bool _cursorIsValid;
        private bool _cursorIsVisible;
        private bool _cursorMoved;
        private Sprite _cursorSprite;

        private Viewport _viewport;
        private ScreenManager _manager;

        private bool _handleVirtualStick;

        public MouseState MouseState
        {
            // HACK: fixed value for player number
            get { return CurrentMouseStates[0]; }
        }

        public MouseState PreviousMouseState
        {
            // HACK: fixed value for player number
            get { return LastMouseStates[0]; }
        }

        public bool ShowCursor
        {
            get { return _cursorIsVisible && _cursorIsValid; }
            set { _cursorIsVisible = value; }
        }

        public bool EnableVirtualStick
        {
            get { return _handleVirtualStick; }
            set { _handleVirtualStick = value; }
        }

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState(ScreenManager vManager)
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];
            CurrentMouseStates = new MouseState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
            LastMouseStates = new MouseState[MaxInputs];

            GamePadWasConnected = new bool[MaxInputs];

            _manager = vManager;

            _handleVirtualStick = false;

        }


        public void LoadContent()
        {
            _cursorSprite = new Sprite(_manager.Content.Load<Texture2D>("Cursors/CirclePointer"));
            _viewport = _manager.GraphicsDevice.Viewport;
        }

        /// <summary>
        /// Reads the latest state user input.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];
                LastMouseStates[i] = CurrentMouseStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState();// Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
                CurrentMouseStates[i] = Mouse.GetState(); //(PlayerIndex)i

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }

            // Get the raw touch state from the TouchPanel
            TouchState = TouchPanel.GetState();

            // Read in any detected gestures into our list for the screens to later process
            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }


            if (_handleVirtualStick)
            {
                _lastVirtualState = _currentVirtualState;
            }

            if (_handleVirtualStick)
            {

                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    _currentVirtualState = GamePad.GetState(PlayerIndex.One);
                }
                else
                {
                    _currentVirtualState = HandleVirtualStickWin();
                }

            }

            // Update cursor 
            // HACK: Fixed values below for player number CurrentGamePadStates[0]
            Vector2 oldCursor = _cursor;
            if (CurrentGamePadStates[0].IsConnected && CurrentGamePadStates[0].ThumbSticks.Left != Vector2.Zero)
            {
                Vector2 temp = CurrentGamePadStates[0].ThumbSticks.Left;
                _cursor += temp * new Vector2(300f, -300f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Mouse.SetPosition((int)_cursor.X, (int)_cursor.Y);
            }
            else
            {
                _cursor.X = CurrentMouseStates[0].X;
                _cursor.Y = CurrentMouseStates[0].Y;
            }
            _cursor.X = MathHelper.Clamp(_cursor.X, 0f, _viewport.Width);
            _cursor.Y = MathHelper.Clamp(_cursor.Y, 0f, _viewport.Height);

            if (_cursorIsValid && oldCursor != _cursor)
            {
                _cursorMoved = true;
            }
            else
            {
                _cursorMoved = false;
            }


            if (_viewport.Bounds.Contains(CurrentMouseStates[0].X, CurrentMouseStates[0].Y))
            {
                _cursorIsValid = true;
            }
            else
            {
                _cursorIsValid = false;
            }


        }


        public void Draw(float vCursorScale)
        {
            if (_cursorIsVisible && _cursorIsValid)
            {
                _manager.SpriteBatch.Begin();
                _manager.SpriteBatch.Draw(_cursorSprite.Texture, _cursor, null, Color.White, 0f, _cursorSprite.Origin, vCursorScale, SpriteEffects.None, 0f);
                _manager.SpriteBatch.End();
            }

        }



        private GamePadState HandleVirtualStickWin()
        {
            Vector2 _leftStick = Vector2.Zero;
            List<Buttons> _buttons = new List<Buttons>();
            // HACK: fixed values below this.CurrentKeyboardStates[0]
            if (this.CurrentKeyboardStates[0].IsKeyDown(Keys.A))
            {
                _leftStick.X -= 1f;
            }
            if (this.CurrentKeyboardStates[0].IsKeyDown(Keys.S))
            {
                _leftStick.Y -= 1f;
            }
            if (this.CurrentKeyboardStates[0].IsKeyDown(Keys.D))
            {
                _leftStick.X += 1f;
            }
            if (this.CurrentKeyboardStates[0].IsKeyDown(Keys.W))
            {
                _leftStick.Y += 1f;
            }
            if (this.CurrentKeyboardStates[0].IsKeyDown(Keys.Space))
            {
                _buttons.Add(Buttons.A);
            }
            if (this.CurrentKeyboardStates[0].IsKeyDown(Keys.LeftControl))
            {
                _buttons.Add(Buttons.B);
            }
            if (_leftStick != Vector2.Zero)
            {
                _leftStick.Normalize();
            }

            return new GamePadState(_leftStick, Vector2.Zero, 0f, 0f, _buttons.ToArray());
        }

        private GamePadState HandleVirtualStickWP7()
        {
            List<Buttons> _buttons = new List<Buttons>();
            Vector2 _stick = Vector2.Zero;

            return new GamePadState(_stick, Vector2.Zero, 0f, 0f, _buttons.ToArray());
        }


        /// <summary>
        /// Helper for checking if a key was pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsKeyPressed(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentKeyboardStates[i].IsKeyDown(key);
            }
            else
            {
                // Accept input from any player.
                return (IsKeyPressed(key, PlayerIndex.One, out playerIndex) ||
                        IsKeyPressed(key, PlayerIndex.Two, out playerIndex) ||
                        IsKeyPressed(key, PlayerIndex.Three, out playerIndex) ||
                        IsKeyPressed(key, PlayerIndex.Four, out playerIndex));
            }
        }


        public bool IsNewKeyRelease(Keys key)
        {
            // HACK: fixed value for player
            return (this.LastKeyboardStates[0].IsKeyDown(key) &&
                    this.CurrentKeyboardStates[0].IsKeyUp(key));
        }

        /// <summary>
        /// Helper for checking if a button was pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentGamePadStates[i].IsButtonDown(button);
            }
            else
            {
                // Accept input from any player.
                return (IsButtonPressed(button, PlayerIndex.One, out playerIndex) ||
                        IsButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
                        IsButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
                        IsButtonPressed(button, PlayerIndex.Four, out playerIndex));
            }
        }


        public bool IsMouseButtonPressed(MouseButtons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                if (button == MouseButtons.LeftButton)
                {
                    return CurrentMouseStates[i].LeftButton == ButtonState.Pressed;
                }
                else if (button == MouseButtons.RightButton)
                {
                    return CurrentMouseStates[i].RightButton == ButtonState.Pressed;
                }
                else
                {
                    return false; // shouldnt nit this
                }
            }
            else
            {
                // Accept input from any player.
                return (IsMouseButtonPressed(button, PlayerIndex.One, out playerIndex) ||
                        IsMouseButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
                        IsMouseButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
                        IsMouseButtonPressed(button, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        public bool IsNewMouseButtonPress(MouseButtons mouseButton, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;
                if (mouseButton == MouseButtons.LeftButton)
                {
                    return (CurrentMouseStates[i].LeftButton == ButtonState.Pressed &&
                            LastMouseStates[i].LeftButton == ButtonState.Released);
                }
                else if (mouseButton == MouseButtons.RightButton)
                {
                    return (CurrentMouseStates[i].RightButton == ButtonState.Pressed &&
                            LastMouseStates[i].RightButton == ButtonState.Released);
                }
                else
                {
                    return false; // this shouldnt be hit?!
                }

            }
            else
            {
                // Accept input from any player.
                return (IsNewMouseButtonPress(mouseButton, PlayerIndex.One, out playerIndex) ||
                        IsNewMouseButtonPress(mouseButton, PlayerIndex.Two, out playerIndex) ||
                        IsNewMouseButtonPress(mouseButton, PlayerIndex.Three, out playerIndex) ||
                        IsNewMouseButtonPress(mouseButton, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }
    }
}
