#region File Description
//-----------------------------------------------------------------------------
// LoadingScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MenuManager.ScreenSystem;
#endregion

namespace MenuManager.MenuSystem
{
    /// <summary>
    /// The loading screen coordinates transitions between the menu system and the
    /// game itself. Normally one screen will transition off at the same time as
    /// the next screen is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    /// 
    /// - Tell all the existing screens to transition off.
    /// - Activate a loading screen, which will transition on at the same time.
    /// - The loading screen watches the state of the previous screens.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next screen, which may take a long time to load its data. The loading
    ///   screen will be the only thing displayed while this load is taking place.
    /// </summary>
    public class LogoScreen : GameScreen, IDisposable
    {
        #region Fields

        bool screenFinished;

        int SecondsToDisplay = 2;
        DateTime CountDownTime;

        ContentManager content;
        Texture2D backgroundTexture;
        float backgroundScale;
        float scaleGrowRate = 0.0005f;
        Boolean DoZoom;

        string texturePath;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        public LogoScreen(int vSecondsToDisplay, string vTexturePath, Boolean vDoZoom)
        {
            screenFinished = false;
            SecondsToDisplay = vSecondsToDisplay;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);

            texturePath = vTexturePath;
            CountDownTime = DateTime.Now.AddSeconds(SecondsToDisplay);
            DoZoom = vDoZoom;

            if (DoZoom)
            {
                backgroundScale = 0.9f; // start a little smaller
            }
            else
            {
                backgroundScale = 1.0f;
            }
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

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (screenFinished == true 
                || CountDownTime < DateTime.Now)
            {

                ExitScreen();
                //ScreenManager.RemoveScreen(this);


                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

            this.backgroundScale += scaleGrowRate;
           
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen;
            Vector2 halfTextureSize = new Vector2(backgroundTexture.Width / 2.0f, backgroundTexture.Height / 2.0f);
            Vector2 halfViewPort = new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f);
                        
            // if the Pic is LARGER than the viewport
            if (backgroundTexture.Height > viewport.Height) {
                // assume if height is bigger, width will be bigger too

                float aspect = (float)viewport.Height / (float)backgroundTexture.Height;

                fullscreen = new Rectangle(
                    (int)(0),
                    (int)(0),
                    (int)(viewport.Width),
                    (int)(viewport.Height)
                    );

            } else {
                // if the Pic is SMALLER than the viewport
                
                fullscreen = new Rectangle(
                    (int)(halfViewPort.X - halfTextureSize.X),
                    (int)(halfViewPort.Y - halfTextureSize.Y), 
                    backgroundTexture.Width, 
                    backgroundTexture.Height);

            }

             spriteBatch.Begin();


             
             if (DoZoom)
             {
                 spriteBatch.Draw(backgroundTexture, halfViewPort, null,
                      new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), 0.0f, halfTextureSize, backgroundScale, SpriteEffects.None, 0.0f);
             }
             else
             {
                 spriteBatch.Draw(backgroundTexture, fullscreen,
                                  new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
             }

            spriteBatch.End();


        }

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput( GameTime gameTime, InputState input)
        {
            //PlayerIndex playerIndex;

            if ((input.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.Space))
                || (input.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.Enter))
                || (input.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.Escape))
                || (input.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                || (input.MouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                )
            {
                screenFinished = true;
            }
            
            
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
                this.backgroundTexture.Dispose();
                this.content.Unload();
                this.content.Dispose();
            }
        }


    }
}
