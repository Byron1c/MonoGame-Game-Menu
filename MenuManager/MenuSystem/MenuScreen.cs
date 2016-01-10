#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MenuManager.DrawingExtensions;
using MenuSystem;
using MenuManager.ScreenSystem;
#endregion

namespace MenuManager.MenuSystem
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public abstract class MenuScreen : GameScreen
    {
        #region Fields

        public List<MenuEntry> menuEntries = new List<MenuEntry>();
        public int selectedEntry = 0;
        string menuTitle;

        InputAction menuUp;
        InputAction menuDown;
        InputAction menuSelect;
        InputAction menuCancel;

        public Color ColourEntriesNormal = Color.White;
        public Color ColourEntriesOther = Color.PaleVioletRed;

        public float Scale = 1.0f;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        public int SelectedEntry 
        {
            get { return selectedEntry; }
            set { selectedEntry = value; }
        }

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            menuUp = new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp }, 
                new Keys[] { Keys.Up, Keys.W },
                new MouseButtons[] { },
                true);
            menuDown = new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { Keys.Down, Keys.S },
                new MouseButtons[] {  },
                true);
            menuSelect = new InputAction(
                new Buttons[] { Buttons.A, Buttons.Start },
                new Keys[] { Keys.Enter, Keys.Space },
                new MouseButtons[] { MouseButtons.LeftButton },
                true);
            menuCancel = new InputAction(
                new Buttons[] { Buttons.B, Buttons.Back },
                new Keys[] { Keys.Escape },
                new MouseButtons[] { MouseButtons.LeftButton },
                true);

            Scale = SettingsManager.GetResolutionScale(SettingsManager.Settings.Resolution);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            mouseHighlight(input, gameTime, this);

            // Move to the previous menu entry?
            if (menuUp.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                // if the entry is a blank then skip it
                while (menuEntries[selectedEntry].Text == "")
                {
                    selectedEntry--;
                    if (selectedEntry < 0)
                        selectedEntry = menuEntries.Count - 1;
                }


            }

            // Move to the next menu entry?
            if (menuDown.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry++;
                
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                // if the entry is a blank then skip it
                while (menuEntries[selectedEntry].Text == "")
                {
                    selectedEntry++;
                }

                
            }

            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }

            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntryPrevious(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntryPrevious(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnAccept(PlayerIndex playerIndex)
        {
            SaveSettings();
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnAccept(object sender, PlayerIndexEventArgs e)
        {
            // reset the scale just incase the user has changed the resolution
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                if (screen is MenuScreen)
                {
                    ((MenuScreen)screen).SetScale();
                }
            }


            OnAccept(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                // each entry is to be centered horizontally
                // use the viewport if the user has change the res - this stops the text positions from going off centre when changing res
                if (ScreenManager.GraphicsDevice.Viewport.Width != SettingsManager.GetResolutionViewport(SettingsManager.Settings.Resolution).Width) { 
                    position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;
                } else {
                    position.X = SettingsManager.GetResolutionViewport(SettingsManager.Settings.Resolution).Width / 2 - menuEntry.GetWidth(this) / 2;
                }

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += (int)(menuEntry.GetHeight(this) * menuEntry.Scale);
            }
        }


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                
                bool isSelected = IsActive && (i == selectedEntry);

                if (menuEntries[i].MouseHovering)
                {
                    selectedEntry = i;
                    isSelected = true;
                    
                }
                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        internal void mouseHighlight(InputState input, GameTime gameTime, MenuScreen vCurrentScreen)
        {
            Rectangle boundsMouse;
            Rectangle boundsEntry;

            foreach (MenuEntry entry in MenuEntries)
            {
                entry.MouseHovering = false;
            }

            foreach (MenuEntry entry in MenuEntries)
            {
                boundsMouse = new Rectangle(
                    (int)(input.MouseState.X),
                    (int)(input.MouseState.Y),
                    (int)(16 * this.Scale),  
                    (int)(16 * this.Scale) 
                    );

                boundsEntry = new Rectangle(
                         (int)(entry.Position.X),
                         (int)(entry.Position.Y - (entry.GetHeight(this) / 2)),
                         (int)(entry.GetWidth(this)),
                         (int)(entry.GetHeight(this))
                         );

                if (RectangleExtensions.GetIntersectionDepth(boundsEntry, boundsMouse, Vector2.Zero) != Vector2.Zero)
                {
                    entry.MouseHovering = true;
                    break;
                    
                }

            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Fonts.GetFont("MenuSprite");

            spriteBatch.Begin();



            //Texture2D dummyTexture = new Texture2D(graphics, 1, 1);
            //dummyTexture.SetData(new Color[] { Color.White });


            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);


                // TESTING
                //Rectangle boundsEntry = new Rectangle(
                //        (int)(menuEntry.Position.X),
                //        (int)(menuEntry.Position.Y - (menuEntry.GetHeight(this) / 2)),
                //        (int)(menuEntry.GetWidth(this)),
                //        (int)(menuEntry.GetHeight(this))
                //        );

                //spriteBatch.Draw(dummyTexture, boundsEntry, Color.DarkRed);

            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 150);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = ColourEntriesOther * TransitionAlpha; //new Color(252, 172, 172) * TransitionAlpha;
            float titleScale = 1.25f * this.Scale;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
            

            spriteBatch.End();
        }


        #endregion


        public void SetScale()
        {
            float newScale = SettingsManager.GetResolutionScale(SettingsManager.Settings.Resolution);

            this.Scale = newScale;

            ScreenManager.cursorScale = newScale;

            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Scale = newScale;
            }

        }


    }
}
