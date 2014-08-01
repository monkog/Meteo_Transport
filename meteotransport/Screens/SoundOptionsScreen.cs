using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Meteo.Screens
{
    /// <summary>
    /// Sound options screen
    /// </summary>
    class SoundOptionsScreen : MenuScreen
    {
        #region variables
        /// <summary>
        /// Music menu entry
        /// </summary>
        MenuEntry m_musicMenuEntry;
        /// <summary>
        /// Sounds menu entry
        /// </summary>
        MenuEntry m_soundsMenuEntry;

        /// <summary>
        /// Is music on
        /// </summary>
        static bool m_musicOn;
        /// <summary>
        /// Are sounds on
        /// </summary>
        static bool m_soundsOn;
        /// <summary>
        /// Logged user and his preferences
        /// </summary>
        private User LoggedUser;
        /// <summary>
        /// Did user change the values
        /// </summary>
        private bool m_valuesChanged;
        #endregion

        #region constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public SoundOptionsScreen(User user, string title)
            : base(title)
        {
            // Create our menu entries.
            m_musicMenuEntry = new MenuEntry(string.Empty);
            m_soundsMenuEntry = new MenuEntry(string.Empty);

            LoggedUser = user;
            m_musicOn = user.MusicOn;
            m_soundsOn = user.SoundsOn;

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            m_musicMenuEntry.Selected += MusicMenuEntrySelected;
            m_soundsMenuEntry.Selected += SoundsMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(m_musicMenuEntry);
            MenuEntries.Add(m_soundsMenuEntry);
            MenuEntries.Add(back);

            m_valuesChanged = false;
        }
        #endregion

        #region methods
        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            m_musicMenuEntry.Text = "Music: " + (m_musicOn ? "On" : "Off");
            m_soundsMenuEntry.Text = "Sounds: " + (m_soundsOn ? "On" : "Off");
            m_valuesChanged = true;
        }
        #endregion

        #region events
        /// <summary>
        /// Event handler for when the Music menu entry is selected.
        /// </summary>
        void MusicMenuEntrySelected(object sender, EventArgs e)
        {
            m_musicOn = !m_musicOn;
            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Sounds menu entry is selected.
        /// </summary>
        void SoundsMenuEntrySelected(object sender, EventArgs e)
        {
            m_soundsOn = !m_soundsOn;
            SetMenuEntryText();
        }

        /// <summary>
        /// Saves chosen volume value to XML file
        /// </summary>
        private void saveSounds()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(MeteoTransport.ConfigFile);

            XmlNodeList usersListNode =
                doc.SelectSingleNode("Users").SelectNodes("User");

            foreach (XmlNode node in usersListNode)
            {
                User user = new User();
                user.Username = node.SelectSingleNode("Username").InnerText;

                if (user.Username != LoggedUser.Username)
                    continue;

                node.RemoveChild(node.SelectSingleNode("MusicOn"));
                node.RemoveChild(node.SelectSingleNode("SoundsOn"));

                LoggedUser.MusicOn = m_musicOn;
                LoggedUser.SoundsOn = m_soundsOn;

                XmlElement musicVolume = doc.CreateElement("MusicOn");
                musicVolume.InnerText = LoggedUser.MusicOn.ToString();
                XmlElement soundsVolume = doc.CreateElement("SoundsOn");
                soundsVolume.InnerText = LoggedUser.SoundsOn.ToString();

                node.AppendChild(musicVolume);
                node.AppendChild(soundsVolume);

                doc.SelectSingleNode("Users").RemoveChild(node);
                doc.DocumentElement.AppendChild(node);
                doc.Save(MeteoTransport.ConfigFile);

                break;
            }

            m_valuesChanged = false;
        }

        /// <summary>
        /// User pressed esc
        /// </summary>
        protected override void OnCancel()
        {
            if (m_valuesChanged)
                saveSounds();
            ExitScreen();
        }
        #endregion
    }
}
