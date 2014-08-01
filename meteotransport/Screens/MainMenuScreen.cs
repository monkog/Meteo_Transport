#region Using Statements
using Meteo.Levels;
using Meteo.Screens;
using Microsoft.Xna.Framework;
using System;
#endregion

namespace Meteo
{
    /// <summary>
    /// The main menu screen
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Fields
        /// <summary>
        /// Logged user
        /// </summary>
        User LoggedUser { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(User user, string title)
            : base(title)
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("New Game");
            MenuEntry loadGameMenuEntry = new MenuEntry("Load Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry highScoresMenuEntry = new MenuEntry("High Scores");
            MenuEntry aboutMenuEntry = new MenuEntry("About");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            loadGameMenuEntry.Selected += LoadGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            highScoresMenuEntry.Selected += HighScoresMenuEntrySelected;
            aboutMenuEntry.Selected += AboutMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(loadGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(highScoresMenuEntry);
            MenuEntries.Add(aboutMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            LoggedUser = user;
        }
        #endregion

        #region events
        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new DifficultyScreen(LoggedUser, "Game Difficulty"));
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(LoggedUser, "Options"));
        }

        /// <summary>
        /// Event handler for when the About menu entry is selected.
        /// </summary>
        void AboutMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new AboutScreen("About Game"));
        }

        /// <summary>
        /// Event handler for when the High Scores menu entry is selected.
        /// </summary>
        void HighScoresMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new HighScoresScreen("High Scores"));
        }

        /// <summary>
        /// Event handler for when the Load Game menu entry is selected.
        /// </summary>
        void LoadGameMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new LoadedGameScreen(LoggedUser, ""));
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to exit this game?";
            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, true, "");
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            ScreenManager.AddScreen(confirmExitMessageBox);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }
        #endregion
    }
}
