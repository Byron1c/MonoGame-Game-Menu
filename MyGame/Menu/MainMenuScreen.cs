#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using MenuManager.MenuSystem;
using MenuManager.ScreenSystem;
using Microsoft.Xna.Framework;
#endregion


namespace GameMenu
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            HasCursor = true;
            

            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game", this.Scale);
            MenuEntry optionsMenuEntry = new MenuEntry("Options", this.Scale);
            MenuEntry spacerMenuEntry = new MenuEntry(string.Empty, this.Scale);
            MenuEntry exitMenuEntry = new MenuEntry("Quit To Desktop", ColourEntriesOther, this.Scale);

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input


        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput( gameTime, input);

        }
        

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new MyGame.MyGame());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?\n";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, this.Scale);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
