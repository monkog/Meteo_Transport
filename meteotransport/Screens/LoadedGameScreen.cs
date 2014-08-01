using Meteo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Screens
{
    /// <summary>
    /// Screen for chosing saves to load
    /// </summary>
    class LoadedGameScreen : MenuScreen
    {
        #region Fields
        /// <summary>
        /// Logged user
        /// </summary>
        User LoggedUser { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public LoadedGameScreen(User user, string title)
            : base(title)
        {
            LoggedUser = user;

            // Create our menu entries.
            List<string> entries = GameSaver.getSaveEntries(LoggedUser.Username);

            foreach (string entry in entries)
            {
                MenuEntry menuEntry = new MenuEntry(entry);
                menuEntry.Selected += (sender, args) => { menuEntry_Selected(sender, args, entry); };
                MenuEntries.Add(menuEntry);
            }

            MenuEntry back = new MenuEntry("Back");
            back.Selected += OnCancel;
            MenuEntries.Add(back);
        }
        #endregion

        #region Handle Input
        void menuEntry_Selected(object sender, EventArgs e, string fileName)
        {
            GameplayScreen game = new GameplayScreen(LoggedUser, 0, "");
            ScreenManager.AddScreen(game);
            game.loadGame(fileName);
        }
        #endregion
    }
}
