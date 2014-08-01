#region Using Statements
using Meteo.Screens;
using Microsoft.Xna.Framework;
using System;
#endregion

namespace Meteo
{
    /// <summary>
    /// The options screen
    /// </summary>
    class OptionsMenuScreen : MenuScreen
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
        public OptionsMenuScreen(User user, string title)
            : base(title)
        {
            MenuEntry keysMenuEntry = new MenuEntry("Keys");
            MenuEntry soundsMenuEntry = new MenuEntry("Sounds");

            MenuEntry back = new MenuEntry("Back");

            keysMenuEntry.Selected += KeysMenuEntrySelected;
            soundsMenuEntry.Selected += SoundsMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(keysMenuEntry);
            MenuEntries.Add(soundsMenuEntry);
            MenuEntries.Add(back);

            LoggedUser = user;
        }
        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler when the Keys menu entry is selected.
        /// </summary>
        void KeysMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new KeysOptionsScreen(LoggedUser, "Keys Options"));
        }


        /// <summary>
        /// Event handler when the Sound menu entry is selected.
        /// </summary>
        void SoundsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SoundOptionsScreen(LoggedUser, "Sound Options"));
        }
        #endregion
    }
}
