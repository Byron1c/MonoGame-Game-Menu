#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
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
using MenuManager.ScreenSystem;

#endregion

namespace MenuManager.MenuSystem
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    public class BackgroundScreen : GameScreen, IDisposable
    {
        #region Fields

        ContentManager content;
        Texture2D backgroundTexture;
        string texturePath;

        #endregion

        #region Initialization
              

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen(string vTexturePath)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            texturePath = vTexturePath;
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                backgroundTexture = content.Load<Texture2D>(texturePath);
            }
        }


        // TODO: set up clean up / unload

        ///// <summary>
        ///// Unloads graphics content for this screen.
        ///// </summary>
        //public override void Unload()
        //{
        //    content.Unload();
        //}


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        ///// <summary>
        ///// Draws the background screen.
        ///// </summary>
        //public override void Draw(GameTime gameTime)
        //{
        //    SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        //    Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
        //    Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

        //    spriteBatch.Begin();

        //    spriteBatch.Draw(backgroundTexture, fullscreen,
        //                     new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

        //    spriteBatch.End();
        //}


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport; //SettingsManager.GetResolutionRect(SettingsManager.Settings.Resolution);//  ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen;
            Vector2 halfTextureSize = new Vector2(backgroundTexture.Width / 2.0f, backgroundTexture.Height / 2.0f);
            Vector2 halfViewPort = new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f);



            // if the Pic is LARGER than the viewport
            if (backgroundTexture.Height > viewport.Height)
            {
                // assume if height is bigger, width will be bigger too

                float aspect = (float)viewport.Height / (float)backgroundTexture.Height;

                fullscreen = new Rectangle(
                    (int)(0),
                    (int)(0),
                    (int)(viewport.Width),
                    (int)(viewport.Height)
                    );

            }
            else
            {
                // if the Pic is SMALLER than the viewport

                fullscreen = new Rectangle(
                    (int)(halfViewPort.X - halfTextureSize.X),
                    (int)(halfViewPort.Y - halfTextureSize.Y),
                    backgroundTexture.Width,
                    backgroundTexture.Height);

            }

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();


        }


        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                backgroundTexture.Dispose();
                this.content.Unload();
                this.content.Dispose();
            }
        }

        }
}
