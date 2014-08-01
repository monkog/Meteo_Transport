using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Screens
{
    class AboutScreen : MenuScreen
    {
        #region Fields
        /// <summary>
        /// About texture
        /// </summary>
        Texture2D texture;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutScreen(string title)
            : base(title)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            MenuEntry back = new MenuEntry("Back");
            back.Selected += OnCancel;
            MenuEntries.Add(back);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads "about" page
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();
            ContentManager content = new ContentManager(ScreenManager.Game.Services, "Content");

            texture = content.Load<Texture2D>("Helpers/About");
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. 
        /// </summary>
        protected override void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, ScreenManager.GraphicsDevice.Viewport.Height - 100f);
            MenuEntry menuEntry = MenuEntries[0];
            position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.getWidth(this) / 2;

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            // set the entry's position
            menuEntry.Position = position;
        }

        /// <summary>
        /// Draws the About screen
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(texture, new Rectangle((ScreenManager.GraphicsDevice.Viewport.Width - 500) / 2
                , (ScreenManager.GraphicsDevice.Viewport.Height - 500) / 2, 500, 500)
                , new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }
        #endregion
    }
}
