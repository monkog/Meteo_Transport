using Meteo.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Meteo.Screens
{
    /// <summary>
    /// Keys settings screen
    /// </summary>
    class KeysOptionsScreen : MenuScreen
    {
        #region variables
        /// <summary>
        /// Left key menu entry
        /// </summary>
        MenuEntry m_leftMenuEntry;
        /// <summary>
        /// Right key menu entry
        /// </summary>
        MenuEntry m_rightMenuEntry;
        /// <summary>
        /// Up key menu entry
        /// </summary>
        MenuEntry m_upMenuEntry;
        /// <summary>
        /// Down key menu entry
        /// </summary>
        MenuEntry m_downMenuEntry;
        /// <summary>
        /// Turbo key menu entry
        /// </summary>
        MenuEntry m_turboMenuEntry;
        /// <summary>
        /// Was MenuEntry currently selected by user
        /// </summary>
        bool m_isSelected;
        /// <summary>
        /// Last keyboard state
        /// </summary>
        KeyboardState m_lastKeyboardState;
        /// <summary>
        /// Logged user
        /// </summary>
        User LoggedUser { get; set; }
        /// <summary>
        /// Did any of keys change
        /// </summary>
        bool m_selectionChanged;
        #endregion

        #region constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public KeysOptionsScreen(User user, string title)
            : base(title)
        {
            LoggedUser = user;

            // Create our menu entries.
            m_leftMenuEntry = new MenuEntry(string.Empty);
            m_rightMenuEntry = new MenuEntry(string.Empty);
            m_upMenuEntry = new MenuEntry(string.Empty);
            m_downMenuEntry = new MenuEntry(string.Empty);
            m_turboMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            m_leftMenuEntry.Selected += (sender, args) => { m_isSelected = true; };
            m_rightMenuEntry.Selected += (sender, args) => { m_isSelected = true; };
            m_upMenuEntry.Selected += (sender, args) => { m_isSelected = true; };
            m_downMenuEntry.Selected += (sender, args) => { m_isSelected = true; };
            m_turboMenuEntry.Selected += (sender, args) => { m_isSelected = true; };
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(m_leftMenuEntry);
            MenuEntries.Add(m_rightMenuEntry);
            MenuEntries.Add(m_upMenuEntry);
            MenuEntries.Add(m_downMenuEntry);
            MenuEntries.Add(m_turboMenuEntry);
            MenuEntries.Add(back);

            m_isSelected = false;
            m_selectionChanged = false;
            m_lastKeyboardState = Keyboard.GetState();
        }
        #endregion

        #region methods
        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            m_leftMenuEntry.Text = "Left: " + LoggedUser.LeftKey.ToString();
            m_rightMenuEntry.Text = "Right: " + LoggedUser.RightKey.ToString();
            m_upMenuEntry.Text = "Up: " + LoggedUser.UpKey.ToString();
            m_downMenuEntry.Text = "Down: " + LoggedUser.DownKey.ToString();
            m_turboMenuEntry.Text = "Turbo: " + LoggedUser.TurboKey.ToString();
        }

        /// <summary>
        /// Saves Keys options to XML file
        /// </summary>
        private void saveKeysToFile()
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

                node.RemoveChild(node.SelectSingleNode("LeftKey"));
                node.RemoveChild(node.SelectSingleNode("RightKey"));
                node.RemoveChild(node.SelectSingleNode("UpKey"));
                node.RemoveChild(node.SelectSingleNode("DownKey"));
                node.RemoveChild(node.SelectSingleNode("TurboKey"));

                XmlElement leftKey = doc.CreateElement("LeftKey");
                leftKey.InnerText = LoggedUser.LeftKey.ToString();
                XmlElement rightKey = doc.CreateElement("RightKey");
                rightKey.InnerText = LoggedUser.RightKey.ToString();
                XmlElement upKey = doc.CreateElement("UpKey");
                upKey.InnerText = LoggedUser.UpKey.ToString();
                XmlElement downKey = doc.CreateElement("DownKey");
                downKey.InnerText = LoggedUser.DownKey.ToString();
                XmlElement turboKey = doc.CreateElement("TurboKey");
                turboKey.InnerText = LoggedUser.TurboKey.ToString();

                node.AppendChild(leftKey);
                node.AppendChild(rightKey);
                node.AppendChild(upKey);
                node.AppendChild(downKey);
                node.AppendChild(turboKey);

                doc.SelectSingleNode("Users").RemoveChild(node);
                doc.DocumentElement.AppendChild(node);
                doc.Save(MeteoTransport.ConfigFile);

                break;
            }

            m_selectionChanged = false;
        }

        protected override void OnCancel()
        {
            if (m_selectionChanged)
                saveKeysToFile();
            ExitScreen();
        }
        #endregion

        #region events
        /// <summary>
        /// Changes the selected entry and handles the change.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (m_isSelected)
                checkKeys(input);
            else
                base.HandleInput(input);
        }

        private void checkKeys(InputState input)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (m_lastKeyboardState != keyboardState)
            {
                Keys[] keys = keyboardState.GetPressedKeys();

                if (keys != null && keys.GetLength(0) > 0)
                {
                    Keys key = keys[keys.GetLength(0) - 1];
                    if (key == Keys.Q || key == Keys.W || key == Keys.E || key == Keys.R || key == Keys.T || key == Keys.Y
                        || key == Keys.U || key == Keys.I || key == Keys.O || key == Keys.P || key == Keys.A || key == Keys.S
                        || key == Keys.D || key == Keys.F || key == Keys.G || key == Keys.H || key == Keys.J || key == Keys.K
                        || key == Keys.L || key == Keys.Z || key == Keys.X || key == Keys.C || key == Keys.V || key == Keys.B
                        || key == Keys.N || key == Keys.M || key == Keys.Up || key == Keys.Down || key == Keys.Left
                        || key == Keys.Right || key == Keys.Space || key == Keys.NumPad0 || key == Keys.NumPad1
                        || key == Keys.NumPad2 || key == Keys.NumPad3 || key == Keys.NumPad4 || key == Keys.NumPad5
                        || key == Keys.NumPad6 || key == Keys.NumPad7 || key == Keys.NumPad8 || key == Keys.NumPad9)
                    {
                        switch (SelectedIndex)
                        {
                            case 0: // Left key
                                LoggedUser.LeftKey = key;
                                break;
                            case 1: // Right key
                                LoggedUser.RightKey = key;
                                break;
                            case 2: // Up key
                                LoggedUser.UpKey = key;
                                break;
                            case 3: // Down key
                                LoggedUser.DownKey = key;
                                break;
                            case 4: // Turbo key
                                LoggedUser.TurboKey = key;
                                break;
                        }
                        SetMenuEntryText();
                        m_isSelected = false;
                    }
                }
                m_lastKeyboardState = keyboardState;
            }

            if (input.IsMenuSelect() || input.IsMenuCancel())
                m_isSelected = false;
            m_selectionChanged = true;
        }
        #endregion
    }
}
