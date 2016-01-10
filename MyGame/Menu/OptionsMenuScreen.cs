#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using MenuSystem;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MenuManager.MenuSystem;
using MenuManager.ScreenSystem;

#endregion

namespace GameMenu
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry graphicsLevelMenuEntry;
        MenuEntry resolutionMenuEntry;
        MenuEntry fullScreenMenuEntry;
        MenuEntry volumeMenuEntry;

        MenuEntry spacerMenuEntry;

        InputAction menuSelectBack;

        //static Common.GraphicsDetailLevels currentGraphicsDetailLevel = Common.GraphicsDetailLevels.Low;

        static string[] resolutions = { "640x480", "800x480", "800x600", "1024x768", "1366x768"  }; // default to standard res

        static int currentResolution = 0;

        static bool fullScreen = true;

        //static int volume = 10;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            
            resolutions = GetSupportedResolutions();

            SettingsManager.LoadSettings();

            HasCursor = true;

            menuSelectBack = new InputAction(
                new Buttons[] { Buttons.A, Buttons.Start },
                new Keys[] { Keys.Enter, Keys.Space },
                new MouseButtons[] { MouseButtons.RightButton },
                true);
            


            // Create our menu entries.
            graphicsLevelMenuEntry = new MenuEntry(string.Empty, this.Scale);
            resolutionMenuEntry = new MenuEntry(string.Empty, this.Scale);
            fullScreenMenuEntry = new MenuEntry(string.Empty, this.Scale);
            volumeMenuEntry = new MenuEntry(string.Empty, this.Scale);

            spacerMenuEntry = new MenuEntry(string.Empty, this.Scale);

            SetMenuEntryText();

            MenuEntry accept = new MenuEntry("Accept", ColourEntriesOther, this.Scale);
            MenuEntry back = new MenuEntry("Back", ColourEntriesOther, this.Scale);

            // Hook up menu event handlers.

            resolutionMenuEntry.Selected += ResolutionMenuEntrySelected;
            resolutionMenuEntry.SelectedPrevious += ResolutionMenuEntrySelectedPrevious;
            fullScreenMenuEntry.Selected += FullScreenMenuEntrySelected;
            fullScreenMenuEntry.SelectedPrevious += FullScreenMenuEntrySelected;
            volumeMenuEntry.Selected += VolumeMenuEntrySelected;
            volumeMenuEntry.SelectedPrevious += VolumeMenuEntrySelectedPrevious;
            graphicsLevelMenuEntry.Selected += graphicsDetailLevelMenuEntrySelected;
            graphicsLevelMenuEntry.SelectedPrevious += graphicsDetailLevelMenuEntrySelectedPrevious;

            back.Selected += OnCancel;
            accept.Selected += OnAccept;

            // Add entries to the menu.
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(fullScreenMenuEntry); 
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(resolutionMenuEntry);
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(volumeMenuEntry);
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(graphicsLevelMenuEntry);
            MenuEntries.Add(spacerMenuEntry); // spacer
            MenuEntries.Add(spacerMenuEntry); // spacer

            MenuEntries.Add(accept);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {

            //resolutionMenuEntry.Text = "Resolution: " + resolutions[currentResolution];
            resolutionMenuEntry.Text = "Resolution: " + resolutions[GetResolutionArrayPos(SettingsManager.Settings.Resolution)];
            currentResolution = GetResolutionArrayPos(SettingsManager.Settings.Resolution);

            fullScreen = SettingsManager.Settings.IsFullScreen;
            fullScreenMenuEntry.Text = "Full Screen: " + (fullScreen ? "On" : "Off");

            graphicsLevelMenuEntry.Text = "Graphics Detail: " + SettingsManager.Settings.GraphicsDetailLevel;

            volumeMenuEntry.Text = "Volume: " + SettingsManager.Settings.VolumeMain;

            if (SettingsManager.Settings.VolumeMain == 0)
            {
                volumeMenuEntry.Text += " (Mute)";
            }

            if (SettingsManager.Settings.VolumeMain == 10) // Hack: Fixed value for max sound volume
            {
                volumeMenuEntry.Text += " (Full)";
            }
        }


        private int GetResolutionArrayPos(string vResolutionString)
        {

            for (int i = 0; i < resolutions.Length; ++i)
            {

                if (resolutions[i] == vResolutionString)
                {
                    return i;
                }

            }

            return 0;

        }


        public string[] GetSupportedResolutions()
        {

            List<string> Output = new List<string>();

            string currentRes = string.Empty;
            string previousRes = string.Empty;

            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                // remove duplicates (other frequencies)
                currentRes = mode.Width + "x" + mode.Height;
                if (previousRes!= currentRes) Output.Add(currentRes);

                previousRes = currentRes;

            }

            return Output.ToArray();

        }



        #endregion

        #region Handle Input

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);

            PlayerIndex playerIndex;

            if (menuSelectBack.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnSelectEntryPrevious(selectedEntry, playerIndex);
            }

            
        }


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void graphicsDetailLevelMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            SettingsManager.Settings.GraphicsDetailLevel++;

            if (SettingsManager.Settings.GraphicsDetailLevel > AppSettings.GraphicsDetailLevels.Full)
                SettingsManager.Settings.GraphicsDetailLevel = 0;

            SettingsManager.Settings.GraphicsDetailLevel = SettingsManager.Settings.GraphicsDetailLevel;

            //if (currentUngulate > Ungulate.Llama)
            //    currentUngulate = 0;

            SetMenuEntryText();
        }

        void graphicsDetailLevelMenuEntrySelectedPrevious(object sender, PlayerIndexEventArgs e)
        {
            SettingsManager.Settings.GraphicsDetailLevel--;

            if (SettingsManager.Settings.GraphicsDetailLevel < AppSettings.GraphicsDetailLevels.Low)
                SettingsManager.Settings.GraphicsDetailLevel = AppSettings.GraphicsDetailLevels.Full;

            SettingsManager.Settings.GraphicsDetailLevel = SettingsManager.Settings.GraphicsDetailLevel;

            //if (currentUngulate > Ungulate.Llama)
            //    currentUngulate = 0;

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void ResolutionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentResolution -= 1;
            if (currentResolution < 0) currentResolution = resolutions.Length - 1;

            SettingsManager.Settings.Resolution = resolutions[currentResolution];

            SetMenuEntryText();
        }

        void ResolutionMenuEntrySelectedPrevious(object sender, PlayerIndexEventArgs e)
        {
            currentResolution = (currentResolution + 1) % resolutions.Length;

            //currentResolution %= resolutions.Length;

            SettingsManager.Settings.Resolution = resolutions[currentResolution];

            SetMenuEntryText();
                        

        }

        


        /// <summary>
        /// Event handler for when the Frobnicate menu entry is selected.
        /// </summary>
        void FullScreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            fullScreen = !fullScreen;

            SettingsManager.Settings.IsFullScreen = fullScreen;

            SetMenuEntryText();
        }

        


        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        void VolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            SettingsManager.Settings.VolumeMain++;

            if (SettingsManager.Settings.VolumeMain >= 11)
            {
                SettingsManager.Settings.VolumeMain = 0;
            }

            SetMenuEntryText();
        }

        void VolumeMenuEntrySelectedPrevious(object sender, PlayerIndexEventArgs e)
        {
            SettingsManager.Settings.VolumeMain--;

            if (SettingsManager.Settings.VolumeMain < 0)
            {
                SettingsManager.Settings.VolumeMain = 10;
            }

            SetMenuEntryText();
        }

        #endregion
    }
}
