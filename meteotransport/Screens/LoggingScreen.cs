using Meteo.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Xml;

namespace Meteo.Screens
{
    class LoggingScreen : MenuScreen
    {
        #region Fields
        /// <summary>
        /// Logged user
        /// </summary>
        User LoggedUser { get; set; }
        /// <summary>
        /// Last keyboard state
        /// </summary>
        KeyboardState m_lastKeyboardState;
        /// <summary>
        /// Last mouse state
        /// </summary>
        MouseState m_lastMouseState;
        /// <summary>
        /// Actual password
        /// </summary>
        string m_password;

        MenuEntry m_usernameMenuEntry;
        MenuEntry m_passwordMenuEntry;
        MenuEntry m_logInMenuEntry;
        MenuEntry m_addUserMenuEntry;
        #endregion

        #region Constructors
        public LoggingScreen(string title)
            : base(title)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            m_usernameMenuEntry = new MenuEntry("Username: ");
            m_passwordMenuEntry = new MenuEntry("Password: ");
            m_logInMenuEntry = new MenuEntry("Log In");
            m_addUserMenuEntry = new MenuEntry("Add User");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            m_logInMenuEntry.Selected += LogInMenuEntrySelected;
            m_addUserMenuEntry.Selected += AddUserSelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(m_usernameMenuEntry);
            MenuEntries.Add(m_passwordMenuEntry);
            MenuEntries.Add(m_logInMenuEntry);
            MenuEntries.Add(m_addUserMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            m_password = "";
            m_lastKeyboardState = Keyboard.GetState();
            m_lastMouseState = Mouse.GetState();
        }
        #endregion

        #region Input
        /// <summary>
        /// Handles Add User button click
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void AddUserSelected(object sender, EventArgs e)
        {
            if (m_usernameMenuEntry.Text == "Username: " || m_passwordMenuEntry.Text == "Password: ")
            {
                const string message = "Provide a username and a password";
                MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, false, "");
                ScreenManager.AddScreen(confirmExitMessageBox);
                return;
            }

            User user = new User();
            user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(m_password, "md5");
            user.Username = m_usernameMenuEntry.Text.Substring(10);

            if (checkForUser(user, true))
            {
                const string message = "User already exists";
                MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, false, "");
                ScreenManager.AddScreen(confirmExitMessageBox);
                return;
            }

            user.LeftKey = Keys.A;
            user.RightKey = Keys.D;
            user.UpKey = Keys.W;
            user.DownKey = Keys.S;
            user.TurboKey = Keys.Space;

            saveUser(user);
            {
                const string message = "User saved successfully";
                MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, false, "");
                ScreenManager.AddScreen(confirmExitMessageBox);
                return;
            }
        }

        /// <summary>
        /// Event handler for when the High Scores menu entry is selected.
        /// </summary>
        void LogInMenuEntrySelected(object sender, EventArgs e)
        {
            if (validateUser())
                ScreenManager.AddScreen(new MainMenuScreen(LoggedUser, "Main Menu"));
        }

        /// <summary>
        /// When the user cancels the main menu
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

        #region Methods

        /// <summary>
        /// Validates the user
        /// </summary>
        /// <returns>True if user exists, false otherwise</returns>
        public bool validateUser()
        {
            if (m_usernameMenuEntry.Text.Length == 10 || m_passwordMenuEntry.Text.Length == 10)
            {
                const string message = "Pass a username and a password!";
                MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, false, "");
                ScreenManager.AddScreen(confirmExitMessageBox);
                return false;
            }

            User user = new User();
            user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(m_password, "md5");
            user.Username = m_usernameMenuEntry.Text.Substring(10);

            LoggedUser = user;

            return checkForUser(user, false);
        }

        /// <summary>
        /// Checks XML passwords document for logged user
        /// </summary>
        /// <param name="user">Provided information</param>
        /// <param name="newUser">Is this a new user</param>
        /// <returns></returns>
        public bool checkForUser(User user, bool newUser)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(MeteoTransport.ConfigFile);

            XmlNodeList usersListNode = doc.SelectSingleNode("Users").SelectNodes("User");

            foreach (XmlNode node in usersListNode)
            {
                User currentUser = new User();
                currentUser.Username = node.SelectSingleNode("Username").InnerText;
                currentUser.Password = node.SelectSingleNode("Password").InnerText;
                if (user.Equals(currentUser) || (user.Username == currentUser.Username && newUser))
                {
                    user.LeftKey = KeyHelpers.parseKey(node, "LeftKey", Keys.Left);
                    user.RightKey = KeyHelpers.parseKey(node, "RightKey", Keys.Right);
                    user.UpKey = KeyHelpers.parseKey(node, "UpKey", Keys.Up);
                    user.DownKey = KeyHelpers.parseKey(node, "DownKey", Keys.Down);
                    user.TurboKey = KeyHelpers.parseKey(node, "TurboKey", Keys.Space);
                    user.MusicOn = bool.Parse(node.SelectSingleNode("MusicOn").InnerText);
                    user.SoundsOn = bool.Parse(node.SelectSingleNode("SoundsOn").InnerText);
                    return true;
                }
            }

            if (!newUser)
            {
                const string message = "No such user!";
                MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, false, "");
                ScreenManager.AddScreen(confirmExitMessageBox);
            }
            return false;
        }

        /// <summary>
        /// Adds user to XML file
        /// </summary>
        /// <param name="user">User to add</param>
        public void saveUser(User user)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(MeteoTransport.ConfigFile);

            XmlElement userNode = doc.CreateElement("User");
            XmlElement username = doc.CreateElement("Username");
            username.InnerText = user.Username;
            XmlElement password = doc.CreateElement("Password");
            password.InnerText = user.Password;
            XmlElement leftKey = doc.CreateElement("LeftKey");
            leftKey.InnerText = user.LeftKey.ToString();
            XmlElement rightKey = doc.CreateElement("RightKey");
            rightKey.InnerText = user.RightKey.ToString();
            XmlElement upKey = doc.CreateElement("UpKey");
            upKey.InnerText = user.UpKey.ToString();
            XmlElement downKey = doc.CreateElement("DownKey");
            downKey.InnerText = user.DownKey.ToString();
            XmlElement turboKey = doc.CreateElement("TurboKey");
            turboKey.InnerText = user.TurboKey.ToString();
            XmlElement musicVolume = doc.CreateElement("MusicOn");
            musicVolume.InnerText = user.MusicOn.ToString();
            XmlElement soundsVolume = doc.CreateElement("SoundsOn");
            soundsVolume.InnerText = user.SoundsOn.ToString();

            userNode.AppendChild(username);
            userNode.AppendChild(password);
            userNode.AppendChild(leftKey);
            userNode.AppendChild(rightKey);
            userNode.AppendChild(upKey);
            userNode.AppendChild(downKey);
            userNode.AppendChild(turboKey);
            userNode.AppendChild(musicVolume);
            userNode.AppendChild(soundsVolume);

            doc.DocumentElement.AppendChild(userNode);

            doc.Save(MeteoTransport.ConfigFile);
        }
        #endregion

        #region events
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
                                m_usernameMenuEntry.Text += key.ToString();
                                break;
                            case 1: // Password
                                m_passwordMenuEntry.Text += "*";
                                m_password += key.ToString();
                                break;
                        }
                    }
                    else if (key == Keys.Back)
                    {
                        switch (SelectedIndex)
                        {
                            case 0: // Username
                                if (m_usernameMenuEntry.Text.Length > 10)
                                    m_usernameMenuEntry.Text = m_usernameMenuEntry.Text.Substring(0, m_usernameMenuEntry.Text.Length - 1);
                                break;
                            case 1: // Password
                                if (m_passwordMenuEntry.Text.Length > 10)
                                {
                                    m_password = m_password.Substring(0, m_password.Length - 1);
                                    m_passwordMenuEntry.Text = m_passwordMenuEntry.Text.Substring(0, m_passwordMenuEntry.Text.Length - 1);
                                }
                                break;
                        }
                    }
                }
            }

            PlayerIndex playerIndex;
        }
        #endregion
    }
}
