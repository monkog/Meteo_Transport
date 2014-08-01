#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Meteo
{
    /// <summary>
    /// Draws the logging background  screen
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region variables
        /// <summary>
        /// Content Manager
        /// </summary>
        ContentManager m_content;
        /// <summary>
        /// Background
        /// </summary>
        Texture2D m_backgroundTexture;

        #endregion

        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        #endregion

        #region methods
        /// <summary>
        /// Loads graphics content for this screen. 
        /// </summary>
        public override void LoadContent()
        {
            if (m_content == null)
                m_content = new ContentManager(ScreenManager.Game.Services, "Content");

            m_backgroundTexture = m_content.Load<Texture2D>("Backgrounds/Logging");
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            m_content.Unload();
        }


        /// <summary>
        /// Updates the background screen. 
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();
            spriteBatch.Draw(m_backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.End();
        }


        #endregion
    }
}
