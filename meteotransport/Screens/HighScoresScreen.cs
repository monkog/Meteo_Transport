using Meteo.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Meteo.Screens
{
    class HighScoresScreen : MenuScreen
    {
        #region variables
        /// <summary>
        /// High scores file path
        /// </summary>
        private string m_highScoresFile = @".\Conf\highscores.xml";
        /// <summary>
        /// Max number of high scores entries
        /// </summary>
        int m_maxEntries = 10;
        #endregion

        #region constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public HighScoresScreen(string title)
            : base(title)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            MenuEntry back = new MenuEntry("Back");
            back.Selected += OnCancel;
            MenuEntries.Add(back);

            m_usesMouse = false;
        }
        #endregion

        #region methods
        /// <summary>
        /// Loads High Scores entries
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();
            loadHighScores();
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. 
        /// </summary>
        protected override void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, ScreenManager.GraphicsDevice.Viewport.Height - 50f);
            MenuEntry menuEntry = MenuEntries[0];
            position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.getWidth(this) / 2;

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            // set the entry's position
            menuEntry.Position = position;
        }

        /// <summary>
        /// Loads HighScores view
        /// </summary>
        private void loadHighScores()
        {
            if (!File.Exists(m_highScoresFile))
            {
                File.Create(m_highScoresFile).Close();
                StreamWriter writer = new StreamWriter(m_highScoresFile);
                writer.WriteLine("<Users>\n</Users>");
                writer.Close();
            }

            loadHighScoresFromFile();
        }

        /// <summary>
        /// Checks for HighScores file and reads from it
        /// </summary>
        private void loadHighScoresFromFile()
        {
            SpriteFont font = ScreenManager.Font;

            XmlDocument doc = new XmlDocument();
            doc.Load(m_highScoresFile);

            XmlNodeList usersListNode =
                doc.SelectSingleNode("Users").SelectNodes("User");

            List<BestUser> bestUsers = new List<BestUser>();
            foreach (XmlNode node in usersListNode)
            {
                BestUser user = new BestUser();
                user.Username = node.SelectSingleNode("Username").InnerText;
                user.Points = int.Parse(node.SelectSingleNode("Points").InnerText);

                if (user.Points < 0)
                    user.Points = 0;
                bestUsers.Add(user);
            }

            bestUsers.Sort((x, y) => x.Points > y.Points ? -1 : 1);

            int index = 0;
            float maxX = 0f;
            Vector2 position = new Vector2(0f, 175f);
            foreach (BestUser user in bestUsers)
            {
                string text = (index + 1) + ". " + user.Username + " " + user.Points;
                Vector2 size = font.MeasureString(text);

                if (maxX < size.X)
                    maxX = size.X;

                MenuEntry score = new MenuEntry(text);
                score.Position = position;
                MenuEntries.Add(score);
                index++;
                if (index >= m_maxEntries)
                    break;
                position = new Vector2(0, position.Y + size.Y);
            }

            maxX = (ScreenManager.GraphicsDevice.Viewport.Width - maxX) / 2;

            for (int i = 1; i < MenuEntries.Count; i++)
                MenuEntries[i].Position = new Vector2(maxX, MenuEntries[i].Position.Y);
        }

        /// <summary>
        /// Handles input.
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            if (input.IsMenuSelect())
            {
                OnSelectEntry(0);
            }
            else if (input.IsMenuCancel())
            {
                OnCancel();
            }
        }

        /// <summary>
        /// Saves High score
        /// </summary>
        private void saveScore()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(m_highScoresFile);

            XmlNodeList usersListNode =
                doc.SelectSingleNode("Users").SelectNodes("User");

            foreach (XmlNode node in usersListNode)
            {
                BestUser user = new BestUser();
                user.Username = node.SelectSingleNode("Username").InnerText;

                user.Points = int.Parse(node.SelectSingleNode("Points").InnerText);
                doc.RemoveChild(node);

                XmlElement userNode = doc.CreateElement("User");
                XmlElement username = doc.CreateElement("Username");
                username.InnerText = user.Username;
                XmlElement points = doc.CreateElement("Points");
                points.InnerText = user.Points.ToString();

                userNode.AppendChild(username);
                userNode.AppendChild(points);
            }
        }
        #endregion

        /// <summary>
        /// Represents best user
        /// </summary>
        private class BestUser
        {
            /// <summary>
            /// Username
            /// </summary>
            public string Username { get; set; }
            /// <summary>
            /// Points
            /// </summary>
            public int Points { get; set; }
        }
    }
}
