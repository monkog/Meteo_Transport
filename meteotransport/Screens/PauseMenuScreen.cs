#region Using Statements
using Meteo.Helpers;
using Meteo.Items;
using Meteo.Levels;
using Meteo.Screens;
using Microsoft.Xna.Framework;
using System;
#endregion

namespace Meteo
{
    /// <summary>
    /// The pause menu
    /// </summary>
    class PauseMenuScreen : MenuScreen
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
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(User user, Level level, Player player, string title)
            : base(title)
        {
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry saveGameMenuEntry = new MenuEntry("Save Game");
            MenuEntry optionsGameMenuEntry = new MenuEntry("Options");
            MenuEntry aboutGameMenuEntry = new MenuEntry("About");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");

            resumeGameMenuEntry.Selected += OnCancel;
            saveGameMenuEntry.Selected += saveGameMenuEntry_Selected;
            optionsGameMenuEntry.Selected += OptionsGameMenuEntrySelected;
            aboutGameMenuEntry.Selected += AboutGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(saveGameMenuEntry);
            MenuEntries.Add(optionsGameMenuEntry);
            MenuEntries.Add(aboutGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);

            LoggedUser = user;
            m_level = level;
            m_player = player;
        }
        #endregion

        #region Handle Input
        /// <summary>
        /// Shows about menu
        /// </summary>
        void AboutGameMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new AboutScreen("About Game"));
        }

        /// <summary>
        /// Goes to options menu
        /// </summary>
        void OptionsGameMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(LoggedUser, "Options"));
        }

        /// <summary>
        /// Saves game
        /// </summary>
        void saveGameMenuEntry_Selected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SaveScreen(LoggedUser, m_level, m_player, "Pick a file name"));
        }

        /// <summary>
        /// Shows exit prompt window
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to exit this game?";
            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message, true, "");
            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;
            ScreenManager.AddScreen(confirmQuitMessageBox);
        }

        /// <summary>
        /// Goes to main menu
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen(LoggedUser, "Main Menu"));
        }
        #endregion
    }
}
