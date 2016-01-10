#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MenuManager.ScreenSystem;
using MenuSystem;

#endregion

namespace MenuManager.MenuSystem
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class MessageBoxScreen : GameScreen
    {
        #region Fields

        string message;
        Texture2D gradientTexture;

        InputAction menuSelect;
        InputAction menuCancel;

        private float scale = 1.0f;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, float vScale)
            : this(message, true, vScale)
        { }


        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText, float vScale)
        {
            //const string usageText = "\nA button, Space, Enter = ok" +
            //                         "\nB button, Esc = cancel";
            const string usageText = "\nSpace or Enter to Quit\n" +
                                     "\nEsc to Cancel"; 
            
            if (includeUsageText)
                this.message = message + usageText;
            else
                this.message = message;

            HasCursor = true;
            IsPopup = true;

            scale = vScale;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            menuSelect = new InputAction(
                new Buttons[] { Buttons.A, Buttons.Start },
                new Keys[] { Keys.Space, Keys.Enter },
                new MouseButtons[] { MouseButtons.LeftButton },
                true);
            menuCancel = new InputAction(
                new Buttons[] { Buttons.B, Buttons.Back },
                new Keys[] { Keys.Escape, Keys.Back },
                new MouseButtons[] { MouseButtons.RightButton },
                true);
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                ContentManager content = ScreenManager.Game.Content;
                gradientTexture = content.Load<Texture2D>("Textures/gradient");
            }
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Fonts.GetFont("MenuSprite");

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = SettingsManager.GetResolutionViewport(SettingsManager.Settings.Resolution); //  ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message) * scale;
            Vector2 textPosition = (viewportSize / 2 - textSize / 2);


            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            // Draw the message box text.
            //spriteBatch.DrawString(font, message, textPosition, color);
            spriteBatch.DrawString(font, message, textPosition, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1.0f);

            spriteBatch.End();
        }


        #endregion
    }
}
