using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Screens
{
    /// <summary>
    /// Screen to choose difficulty level
    /// </summary>
    class DifficultyScreen : MenuScreen
    {

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user">Logged user</param>
        public DifficultyScreen(User user, string menuTitle)
            : base(menuTitle)
        {
            MenuEntry lowMenuEntry = new MenuEntry("Low");
            MenuEntry mediumMenuEntry = new MenuEntry("Medium");
            MenuEntry highMenuEntry = new MenuEntry("High");
            MenuEntry backMenuEntry = new MenuEntry("Back");

            lowMenuEntry.Selected += (sender, e) =>{ LoadingScreen.Load(ScreenManager, true, new GameplayScreen(user, 1, ""));};
            mediumMenuEntry.Selected += (sender, e) =>{LoadingScreen.Load(ScreenManager, true, new GameplayScreen(user, 2, ""));};
            highMenuEntry.Selected += (sender, e) =>{LoadingScreen.Load(ScreenManager, true, new GameplayScreen(user, 3, ""));};
            backMenuEntry.Selected += OnCancel;

            MenuEntries.Add(lowMenuEntry);
            MenuEntries.Add(mediumMenuEntry);
            MenuEntries.Add(highMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }
        #endregion
    }
}
