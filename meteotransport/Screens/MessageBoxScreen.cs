#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Meteo
{
    /// <summary>
    /// A message box screen
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : MenuScreen
    {
        #region Fields
        /// <summary>
        /// Message to view in a MessageBox
        /// </summary>
        string m_message;
        /// <summary>
        /// Texture
        /// </summary>
        Texture2D m_txture;
        /// <summary>
        /// Is Cancel option enabled for this window
        /// </summary>
        bool m_isCancelEnabled;
        /// <summary>
        /// Y position to start drawing menu options on
        /// </summary>
        float m_initY;
        #endregion

        #region Events
        /// <summary>
        /// Event, when Player accepts
        /// </summary>
        public event EventHandler<EventArgs> Accepted;
        /// <summary>
        /// Event when player cancels
        /// </summary>
        public event EventHandler<EventArgs> Cancelled;

        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageBoxScreen(string message, bool isCancelEnabled, string title)
            : base(title)
        {
            m_message = message;
            IsPopup = true;
            m_isCancelEnabled = isCancelEnabled;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            if (isCancelEnabled)
            {
                MenuEntry yesMenuEntry = new MenuEntry("Yes");
                MenuEntry noMenuEntry = new MenuEntry("No");

                yesMenuEntry.Selected += (sender, args) =>
                {
                    if (Accepted != null)
                        Accepted(this, new EventArgs());
                    ExitScreen();
                };
                noMenuEntry.Selected += (sender, args) =>
                {
                    if (Cancelled != null)
                        Cancelled(this, new EventArgs());
                    ExitScreen();
                };

                MenuEntries.Add(yesMenuEntry);
                MenuEntries.Add(noMenuEntry);
            }
            else
            {
                MenuEntry okMenuEntry = new MenuEntry("OK");

                okMenuEntry.Selected += (sender, args) =>
                {
                    if (Accepted != null)
                        Accepted(this, new EventArgs());
                    ExitScreen();
                };

                MenuEntries.Add(okMenuEntry);
            }
        }

        /// <summary>
        /// Loads graphics content 
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            m_txture = content.Load<Texture2D>("Backgrounds/Prompt");
            m_initY = (ScreenManager.GraphicsDevice.Viewport.Height - ScreenManager.Font.MeasureString(m_message).Y) / 2;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Positions the menu entries 
        /// </summary>
        protected override void UpdateMenuEntryLocations()
        {
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, ScreenManager.GraphicsDevice.Viewport.Height - 175f);
            float y = m_initY;
            foreach (MenuEntry entry in MenuEntries)
            {
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - entry.getWidth(this) / 2;
                position.Y = y;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                entry.Position = position;
                y += entry.getHeight(this);
            }
        }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(m_message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            const int MARGIN = 60;
            textPosition.Y -= MARGIN;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - MARGIN, (int)textPosition.Y - MARGIN
                , (int)textSize.X + MARGIN * 2, (int)textSize.Y + MARGIN * 4);

            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();
            spriteBatch.Draw(m_txture, backgroundRectangle, color);
            spriteBatch.DrawString(font, m_message, textPosition, color);
            spriteBatch.End();

            float menuEntriesPosition = 0;
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntry entry = MenuEntries[i];
                entry.Position = new Vector2(entry.Position.X, menuEntriesPosition);
                menuEntriesPosition += entry.getHeight(this) + 20;
            }

            base.Draw(gameTime);
        }
        #endregion
    }
}
