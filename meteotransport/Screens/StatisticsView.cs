using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meteo.Items;
using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Meteo.Helpers;

namespace Meteo.Screens
{
    /// <summary>
    /// Views statics after a level
    /// </summary>
    class StatisticsScreen : MenuScreen
    {
        #region variables
        /// <summary>
        /// Current player
        /// </summary>
        private Player m_player;
        /// <summary>
        /// Logged user
        /// </summary>
        User LoggedUser { get; set; }
        /// <summary>
        /// Current level
        /// </summary>
        Level m_level;
        #endregion

        public event EventHandler<EventArgs> Ended;

        #region constructors
        public StatisticsScreen(User user, Player player, Level level, string message, string title)
            : base(title)
        {
            MenuEntry saveMenuEntry = new MenuEntry("Save Game");
            MenuEntry menuMenuEntry = new MenuEntry("MainMenu");
            MenuEntry nextMenuEntry = new MenuEntry(message);

            saveMenuEntry.Selected += saveMenuEntry_Selected;
            menuMenuEntry.Selected += menuMenuEntry_Selected;
            nextMenuEntry.Selected += nextMenuEntry_Selected;
            nextMenuEntry.Selected += OnCancel;

            MenuEntries.Add(new MenuEntry("Points: " + player.Points));
            MenuEntries.Add(new MenuEntry("Shot predators: " + player.ShotPredators));
            MenuEntries.Add(new MenuEntry("Total points: " + player.TotalPoints));
            MenuEntries.Add(nextMenuEntry);
            if (message != "Main Menu")
            {
                if (message != "Restart Level")
                    MenuEntries.Add(saveMenuEntry);
                MenuEntries.Add(menuMenuEntry);
            }

            m_player = player;
            m_usesMouse = false;
            LoggedUser = user;
            m_level = level;
            SelectedIndex = 3;
        }

        /// <summary>
        /// Changes the selected entry and handles the change.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input.IsMenuUp())
            {
                SelectedIndex--;

                if (SelectedIndex < 3)
                    SelectedIndex = MenuEntries.Count - 1;
            }

            if (input.IsMenuDown())
            {
                SelectedIndex++;

                if (SelectedIndex >= MenuEntries.Count)
                    SelectedIndex = 3;
            }

            PlayerIndex playerIndex;

            if (input.IsMenuSelect())
            {
                OnSelectEntry(SelectedIndex);
            }
        }

        /// <summary>
        /// Goes to main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuMenuEntry_Selected(object sender, EventArgs e)
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

        /// <summary>
        /// Saves game
        /// </summary>
        void saveMenuEntry_Selected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SaveScreen(LoggedUser, m_level, m_player, "Pick a file name"));
        }

        /// <summary>
        /// Generates next level
        /// </summary>
        void nextMenuEntry_Selected(object sender, EventArgs e)
        {
            Ended(this, EventArgs.Empty);
        }
        #endregion
    }
}
