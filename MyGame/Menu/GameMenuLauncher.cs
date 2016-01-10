using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MenuSystem;
using MenuManager.MenuSystem;
using MenuManager.ScreenSystem;

namespace GameMenu
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMenuLauncher : Game
    {

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }
        List<MenuEntry> menuEntries = new List<MenuEntry>();


        bool LogoScreen1Done = false;
        bool LogoScreen2Done = false;

        public ScreenManager ScreenManager { get; set; }



        public GameMenuLauncher()
        {

            SettingsManager.LoadSettings();

            Window.Title = "Game Menu for MonoGame";
            
            IsFixedTimeStep = true;

            Content.RootDirectory = "Content";

            //new-up components and add to Game.Components
            ScreenManager = new ScreenManager(this, SettingsManager.GetResolutionScale(SettingsManager.Settings.Resolution));
            Components.Add(ScreenManager);

            ScreenManager.graphicsInitialize();

            FrameRateCounter frameRateCounter = new FrameRateCounter(ScreenManager);
            frameRateCounter.DrawOrder = 101;
            Components.Add(frameRateCounter);


            LogoScreen1Done = false;
            LogoScreen2Done = false;

        }


        private void AddMenuScreens()
        {

            // Activate the first screens.
            ScreenManager.AddScreen(new BackgroundScreen("Logos/background"), null);

            ScreenManager.AddScreen(new MainMenuScreen(), null);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }



        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // this should not load the other screens until the logo screen has finished
            if (ScreenManager.GetScreens().Length == 0 && LogoScreen1Done == false && LogoScreen2Done == false)
            {

                // Second Screen to show - first is added on Initialization
                ScreenManager.AddScreen(new LogoScreen(4, "Logos/CompanyLogo", true), null);

                LogoScreen1Done = true;

            }


            // Load each of the introduction screens:

            if (ScreenManager.GetScreens().Length == 0 && LogoScreen1Done == true && LogoScreen2Done == false)
            {

                // FIRST Screen to show
                ScreenManager.AddScreen(new LogoScreen(4, "Logos/MainTitle", true), null);

                LogoScreen2Done = true;

            }

            // Then load the menu screen when the others are finished displaying
            if (ScreenManager.GetScreens().Length == 0 && LogoScreen1Done == true && LogoScreen2Done == true)
            {
                // add menu screens
                AddMenuScreens();
            }



        }
        


    }
}