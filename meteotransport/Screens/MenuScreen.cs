#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Meteo
{
    /// <summary>
    /// Base class for screens that contain a menu
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields
        /// <summary>
        /// List of menu entries
        /// </summary>
        List<MenuEntry> m_menuEntries = new List<MenuEntry>();
        /// <summary>
        /// List of menu items rectangles
        /// </summary>
        List<MenuEntry> m_menuRectangles = new List<MenuEntry>();
        /// <summary>
        /// Currently selected index
        /// </summary>
        public int SelectedIndex { get; protected set; }
        /// <summary>
        /// Previous mouse state
        /// </summary>
        MouseState m_previousMouseState;
        /// <summary>
        /// Prevents from hoovering over labels not being buttons
        /// </summary>
        protected bool m_usesMouse = true;
        /// <summary>
        /// Menu title
        /// </summary>
        string m_menuTitle;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of menu entries
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return m_menuEntries; }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            m_menuTitle = menuTitle;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            m_previousMouseState = Mouse.GetState();
            SelectedIndex = 0;
        }
        #endregion

        #region Handle Input
        /// <summary>
        /// Changes the selected entry and handles the change.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input.IsMenuUp())
            {
                SelectedIndex--;

                if (SelectedIndex < 0)
                    SelectedIndex = m_menuEntries.Count - 1;
            }

            if (input.IsMenuDown())
            {
                SelectedIndex++;

                if (SelectedIndex >= m_menuEntries.Count)
                    SelectedIndex = 0;
            }

            if (input.IsMenuSelect())
            {
                OnSelectEntry(SelectedIndex);
            }
            else if (input.IsMenuCancel())
            {
                OnCancel();
            }
        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            m_menuEntries[entryIndex].OnSelectEntry();
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        /// <summary>
        /// Use OnCancel as a MenuEntry event handler
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Positions the menu entries
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, 175f);

            for (int i = 0; i < m_menuEntries.Count; i++)
            {
                MenuEntry menuEntry = m_menuEntries[i];
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.getWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                menuEntry.Position = position;
                position.Y += menuEntry.getHeight(this);
            }
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            MouseState mouseState = Mouse.GetState();

            for (int i = 0; i < m_menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == SelectedIndex);

                m_menuEntries[i].Update(this, isSelected, gameTime);
            }

            if (m_previousMouseState != mouseState && m_usesMouse)
                for (int i = 0; i < m_menuEntries.Count; i++)
                {
                    MenuEntry menuEntry = m_menuEntries[i];
                    Rectangle rectangle = new Rectangle((int)menuEntry.Position.X
                        , (int)menuEntry.Position.Y, (int)menuEntry.getWidth(this), (int)menuEntry.getHeight(this));
                    if (rectangle.Contains(new Point(mouseState.X, mouseState.Y)))
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            m_previousMouseState = mouseState;
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            for (int i = 0; i < m_menuEntries.Count; i++)
            {
                MenuEntry menuEntry = m_menuEntries[i];

                bool isSelected = IsActive && (i == SelectedIndex);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 35);
            Vector2 titleOrigin = font.MeasureString(m_menuTitle) / 2;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, m_menuTitle, titlePosition, Color.Black, 0,
                                   titleOrigin, 1.28f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, m_menuTitle, titlePosition, Color.GreenYellow, 0,
                                   titleOrigin, 1.25f, SpriteEffects.None, 0);

            spriteBatch.End();
        }
        #endregion
    }
}
