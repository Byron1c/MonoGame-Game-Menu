#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MenuManager.ScreenSystem;
#endregion

namespace MenuManager.MenuSystem
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public class MenuEntry
    {
        #region Fields

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;

        /// <summary>
        /// The colour of this entry
        /// </summary>
        Color TextColour;

        /// <summary>
        /// Flag to check if the mouse is over this menu entry
        /// </summary>
        bool mouseHovering;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 position;

        /// <summary>
        /// The overall size of the button, so it can be used to adjust based on things such as the resolution
        /// </summary>
        float scale;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        public bool MouseHovering
        {
            get { return mouseHovering; }
            set { mouseHovering = value; }
        }


        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }


        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }


        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        public event EventHandler<PlayerIndexEventArgs> SelectedPrevious;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        protected internal virtual void OnSelectEntryPrevious(PlayerIndex playerIndex)
        {
            if (SelectedPrevious != null)
                SelectedPrevious(this, new PlayerIndexEventArgs(playerIndex));
        }
        

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text, float vScale)
        {
            this.text = text;
            this.TextColour = Color.White;
            this.scale = vScale;
        }

        public MenuEntry(string text, Color vColour, float vScale)
        {
            this.text = text;
            this.TextColour = vColour;
            this.scale = vScale;
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false

            Color color; 

            // Draw the selected entry in yellow, otherwise white.
            color = isSelected ? Color.Yellow : TextColour; // Color.White;

            if (mouseHovering)
            {
                color = Color.Red;
            }


            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            
            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;
            scale *= this.scale;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Fonts.GetFont("MenuSprite");

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return (int)(screen.ScreenManager.Fonts.GetFont("MenuSprite").LineSpacing * this.scale);
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)(screen.ScreenManager.Fonts.GetFont("MenuSprite").MeasureString(Text).X * this.scale);
        }


        #endregion
    }
}
