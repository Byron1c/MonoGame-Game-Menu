#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using MenuManager.MenuSystem;
using MenuSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace GameMenu
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization

        Texture2D gradientTexture;
        //PhysicsGameScreen gameScreen;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            HasCursor = true;
            //gameScreen = vGameScreen;

            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game", this.Scale);
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game", ColourEntriesOther, this.Scale);
            
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(new MenuEntry(string.Empty, this.Scale)); // spacer
            MenuEntries.Add(new MenuEntry(string.Empty, this.Scale)); // spacer
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(new MenuEntry(string.Empty, this.Scale)); // spacer
            MenuEntries.Add(new MenuEntry(string.Empty, this.Scale)); // spacer
            MenuEntries.Add(quitGameMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Paused\n\nAre you sure you want to quit this game?\n";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message, this.Scale);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen("Logos/background"),
                                                           new MainMenuScreen());
        }


        #endregion


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



        public override void Draw(GameTime gameTime)
        {
            // Draw the Background
            int height = 0;
            int yPos = 9999; // start artifically high so any value will replace it on first run

            // get the vertical values of the bounding rectangle
            foreach (MenuEntry entry in this.MenuEntries)
            {
                if (entry.Position.Y < yPos) yPos = (int)entry.Position.Y;
                if (entry.Position.Y > height) height += entry.GetHeight(this); //(int)entry.Position.Y;

            }
            yPos -= MenuEntries[0].GetHeight(this);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Fonts.GetFont("MenuSprite");

            Viewport viewport = SettingsManager.GetResolutionViewport(SettingsManager.Settings.Resolution); // ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString("Paused\n\nAre you sure you want to quit this game?\n") * this.Scale;
            Vector2 textPosition = (viewportSize / 2 - textSize / 2);


            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)yPos - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)height + vPad * 2);

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            spriteBatch.End();


            base.Draw(gameTime);

        }



        }
}
