using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meteo.Helpers;
using Meteo.Items;
using Meteo.Levels;
using Microsoft.Xna.Framework.Input;

namespace Meteo.Screens
{
    /// <summary>
    /// Screen when player wants to save his game
    /// </summary>
    class SaveScreen : MenuScreen
    {
        #region Fields
        /// <summary>
        /// Logged user
        /// </summary>
        User LoggedUser { get; set; }
        /// <summary>
        /// Game player
        /// </summary>
        Player m_player;
        /// <summary>
        /// Current level
        /// </summary>
        Level m_level;
        /// Last keyboard state
        /// </summary>
        KeyboardState m_lastKeyboardState;
        /// <summary>
        /// Last mouse state
        /// </summary>
        MouseState m_lastMouseState;
        /// <summary>
        /// Game name menu entry
        /// </summary>
        MenuEntry m_gameNameMenuEntry;
        /// <summary>
        /// Save game menu entry
        /// </summary>
        MenuEntry m_saveGameMenuEntry;
        /// <summary>
        /// Back menu entry
        /// </summary>
        MenuEntry m_backMenuEntry;
        #endregion

        #region constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title"></param>
        public SaveScreen(User user, Level level, Player player, string title)
            : base(title)
        {
            LoggedUser = user;
            m_level = level;
            m_player = player;
            m_lastKeyboardState = Keyboard.GetState();
            m_lastMouseState = Mouse.GetState();

            m_gameNameMenuEntry = new MenuEntry("Name: ");
            m_saveGameMenuEntry = new MenuEntry("Save");
            m_backMenuEntry = new MenuEntry("Back");

            m_saveGameMenuEntry.Selected += saveGameMenuEntry_Selected;
            m_saveGameMenuEntry.Selected += OnCancel;
            m_backMenuEntry.Selected += OnCancel;

            MenuEntries.Add(m_gameNameMenuEntry);
            MenuEntries.Add(m_saveGameMenuEntry);
            MenuEntries.Add(m_backMenuEntry);
        }
        #endregion

        #region events
        /// <summary>
        /// Saves game
        /// </summary>
        void saveGameMenuEntry_Selected(object sender, EventArgs e)
        {
            if (m_gameNameMenuEntry.Text == "Name: ")
            {
                const string message = "Provide a file name";
                MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, false, "");
                ScreenManager.AddScreen(confirmExitMessageBox);
                return;
            }

            GameSaver.saveGame(LoggedUser.Username, m_gameNameMenuEntry.Text.Substring(6), m_level, m_player);

            {
                const string message = "Game saved successfully";
                MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, false, "");
                ScreenManager.AddScreen(confirmExitMessageBox);
            }
        }

        /// <summary>
        /// Changes the selected entry and handles the change.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.Enter)
                || m_lastMouseState != mouseState)
                base.HandleInput(input);
            else
                checkKeys(input, keyboardState);
            m_lastKeyboardState = keyboardState;
            m_lastMouseState = mouseState;
        }

        /// <summary>
        /// Checks for the pressed keys
        /// </summary>
        /// <remarks>
        /// Updates Username and Password entries
        /// </remarks>
        /// <param name="input"></param>
        /// <param name="state"></param>
        private void checkKeys(InputState input, KeyboardState state)
        {
            if (m_lastKeyboardState != state)
            {
                Keys[] keys = state.GetPressedKeys();

                if (keys != null && keys.GetLength(0) > 0)
                {
                    Keys key = keys[keys.GetLength(0) - 1];
                    if (key == Keys.Q || key == Keys.W || key == Keys.E || key == Keys.R || key == Keys.T || key == Keys.Y
                    || key == Keys.U || key == Keys.I || key == Keys.O || key == Keys.P || key == Keys.A || key == Keys.S
                    || key == Keys.D || key == Keys.F || key == Keys.G || key == Keys.H || key == Keys.J || key == Keys.K
                    || key == Keys.L || key == Keys.Z || key == Keys.X || key == Keys.C || key == Keys.V || key == Keys.B
                    || key == Keys.N || key == Keys.M)
                    {
                        switch (SelectedIndex)
                        {
                            case 0: // Username
                                m_gameNameMenuEntry.Text += key.ToString();
                                break;
                        }
                    }
                    else if (key == Keys.Back)
                    {
                        switch (SelectedIndex)
                        {
                            case 0: // Username
                                if (m_gameNameMenuEntry.Text.Length > 6)
                                    m_gameNameMenuEntry.Text = m_gameNameMenuEntry.Text.Substring(0, m_gameNameMenuEntry.Text.Length - 1);
                                break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
