using Meteo.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.GameBoard
{
    /// <summary>
    /// Keeps all items of game status
    /// </summary>
    public class GameStatus
    {
        #region variables
        /// <summary>
        /// Size of the status board
        /// </summary>
        private Size m_size;
        /// <summary>
        /// List of items
        /// </summary>
        private List<GameStatusItem> m_items;
        /// <summary>
        /// List of gray boxes
        /// </summary>
        private List<GameStatusItem> m_grayBoxes;
        /// <summary>
        /// List of boxes
        /// </summary>
        private List<GameStatusItem> m_boxes;
        /// <summary>
        /// List of gray lifes
        /// </summary>
        private List<GameStatusItem> m_grayLifes;
        /// <summary>
        /// List of lifes
        /// </summary>
        private List<GameStatusItem> m_lifes;
        /// <summary>
        /// List of gray turbo
        /// </summary>
        private List<GameStatusItem> m_grayTurbo;
        /// <summary>
        /// List of turbo
        /// </summary>
        private List<GameStatusItem> m_turbo;
        /// <summary>
        /// List of gray flares
        /// </summary>
        private List<GameStatusItem> m_grayFlares;
        /// <summary>
        /// List of flares
        /// </summary>
        private List<GameStatusItem> m_flares;
        /// <summary>
        /// Rows of boxes
        /// </summary>
        private static int ROWS = 3;
        /// <summary>
        /// Margin
        /// </summary>
        private static int MARGIN = 20;
        /// <summary>
        /// Player's boxes
        /// </summary>
        private int m_collectedBoxes;
        /// <summary>
        /// Player's flares
        /// </summary>
        private int m_collectedFlares;
        /// <summary>
        /// Player's available turbo
        /// </summary>
        private int m_turboCount;
        /// <summary>
        /// Player's lifes
        /// </summary>
        private int m_lifesCount;
        /// <summary>
        /// Player instance
        /// </summary>
        private Player m_player;
        /// <summary>
        /// Text to view
        /// </summary>
        private string[] m_text;
        /// <summary>
        /// Text locations
        /// </summary>
        private Vector2[] m_textLocations;
        /// <summary>
        /// Font
        /// </summary>
        private SpriteFont m_spriteFont;
        #endregion

        #region constructors
        public GameStatus(int gameStatusWidth, int gameStatusHeight, Player player)
        {
            m_size = new Size(gameStatusWidth, gameStatusHeight);
            m_items = new List<GameStatusItem>();
            m_grayBoxes = new List<GameStatusItem>();
            m_boxes = new List<GameStatusItem>();
            m_grayLifes = new List<GameStatusItem>();
            m_lifes = new List<GameStatusItem>();
            m_grayTurbo = new List<GameStatusItem>();
            m_turbo = new List<GameStatusItem>();
            m_grayFlares = new List<GameStatusItem>();
            m_flares = new List<GameStatusItem>();
            m_player = player;
            m_text = new[] { "LEVEL", "POINTS", "LIFES", "TURBO", "FLARES", "BOXES" };
        }
        #endregion

        #region methods
        /// <summary>
        /// Loads all items
        /// </summary>
        /// <param name="contentManager">Content Manager</param>
        public void loadContent(ContentManager contentManager)
        {
            m_spriteFont = contentManager.Load<SpriteFont>("menufont");

            int counter = 0;
            Texture2D background = contentManager.Load<Texture2D>("Backgrounds/StatusBackground");
            m_items.Add(new GameStatusItem(background, m_size, new Vector2(m_size.Width * 3, 0)));
            int x = m_size.Width * 3 + MARGIN;
            int y = MARGIN;
            Size size = new Size((m_size.Width - 2 * MARGIN) / Player.MAX_LIFES, (int)(1.5 * (m_size.Width - 2 * MARGIN) / Player.MAX_LIFES));

            m_textLocations = new Vector2[m_text.Length];

            m_textLocations[counter] = new Vector2(3 * m_size.Width + (m_size.Width - m_spriteFont.MeasureString(m_text[counter]).X) / 2, y);
            y += (int)m_spriteFont.MeasureString(m_text[counter++]).Y;

            x = m_size.Width * 3 + MARGIN;
            y += MARGIN;

            m_textLocations[counter] = new Vector2(3 * m_size.Width + (m_size.Width - m_spriteFont.MeasureString(m_text[counter]).X) / 2, y);
            y += (int)m_spriteFont.MeasureString(m_text[counter++]).Y;

            x = m_size.Width * 3 + MARGIN;
            y += MARGIN;


            m_textLocations[counter] = new Vector2(3 * m_size.Width + (m_size.Width - m_spriteFont.MeasureString(m_text[counter]).X) / 2, y);
            y += (int)m_spriteFont.MeasureString(m_text[counter++]).Y;
            Texture2D greenBar = contentManager.Load<Texture2D>("Helpers/GreenBar");
            Texture2D grayBar = contentManager.Load<Texture2D>("Helpers/GrayBar");

            for (int i = 0; i < Player.MAX_LIFES; i++)
            {
                m_lifes.Add(new GameStatusItem(greenBar, size, new Vector2(x, y)));
                m_grayLifes.Add(new GameStatusItem(grayBar, size, new Vector2(x, y)));
                x += size.Width;
            }

            x = m_size.Width * 3 + MARGIN;
            y += MARGIN + size.Height;

            m_textLocations[counter] = new Vector2(3 * m_size.Width + (m_size.Width - m_spriteFont.MeasureString(m_text[counter]).X) / 2, y);
            y += (int)m_spriteFont.MeasureString(m_text[counter++]).Y;

            for (int i = 0; i < Player.MAX_TURBO_LEVEL; i++)
            {
                m_turbo.Add(new GameStatusItem(greenBar, size, new Vector2(x, y)));
                m_grayTurbo.Add(new GameStatusItem(grayBar, size, new Vector2(x, y)));
                x += size.Width;
            }

            x = m_size.Width * 3 + MARGIN;
            y += MARGIN + size.Height;

            m_textLocations[counter] = new Vector2(3 * m_size.Width + (m_size.Width - m_spriteFont.MeasureString(m_text[counter]).X) / 2, y);
            y += (int)m_spriteFont.MeasureString(m_text[counter++]).Y;

            for (int i = 0; i < Player.MAX_FLARES; i++)
            {
                m_flares.Add(new GameStatusItem(greenBar, size, new Vector2(x, y)));
                m_grayFlares.Add(new GameStatusItem(grayBar, size, new Vector2(x, y)));
                x += size.Width;
            }

            x = m_size.Width * 3 + MARGIN;
            y += MARGIN + size.Height;
            size = new Size((m_size.Width - 4 * MARGIN) / (Player.MAX_BOXES / ROWS), (m_size.Width - 4 * MARGIN) / (Player.MAX_BOXES / ROWS));
            int margin = (m_size.Width - 2 * MARGIN) / (Player.MAX_BOXES / ROWS) - size.Width;

            m_textLocations[counter] = new Vector2(3 * m_size.Width + (m_size.Width - m_spriteFont.MeasureString(m_text[counter]).X) / 2, y);
            y += (int)m_spriteFont.MeasureString(m_text[counter++]).Y;
            Texture2D box = contentManager.Load<Texture2D>("Items/Box");
            Texture2D grayBox = contentManager.Load<Texture2D>("Items/GrayBox");

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < Player.MAX_BOXES / ROWS; j++)
                {
                    m_boxes.Add(new GameStatusItem(box, size, new Vector2(x, y)));
                    m_grayBoxes.Add(new GameStatusItem(grayBox, size, new Vector2(x, y)));
                    x += size.Width + margin;
                }
                y += size.Height + margin;
                x = m_size.Width * 3 + MARGIN;
            }
        }

        /// <summary>
        /// Draws all items
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        internal void draw(SpriteBatch spriteBatch)
        {
            foreach (GameStatusItem item in m_items)
                item.draw(spriteBatch);

            int i;

            for (i = 0; i < m_text.Length; i++)
                spriteBatch.DrawString(m_spriteFont, m_text[i], m_textLocations[i], Color.GreenYellow);

            drawProgressBar(spriteBatch, m_lifes, m_grayLifes, m_lifesCount, Player.MAX_LIFES);
            drawProgressBar(spriteBatch, m_turbo, m_grayTurbo, m_turboCount, Player.MAX_TURBO_LEVEL);
            drawProgressBar(spriteBatch, m_flares, m_grayFlares, m_collectedFlares, Player.MAX_FLARES);

            for (i = 0; i < m_collectedBoxes; i++)
                m_boxes[i].draw(spriteBatch);
            for (; i < Player.MAX_BOXES; i++)
                m_grayBoxes[i].draw(spriteBatch);
        }

        /// <summary>
        /// Draws progress bar
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="greenList">List of gray items</param>
        /// <param name="grayList">List of colourful items</param>
        /// <param name="value">Numver of colourful items</param>
        /// <param name="maxValue">Total number of items</param>
        private void drawProgressBar(SpriteBatch spriteBatch, List<GameStatusItem> greenList
            , List<GameStatusItem> grayList, int value, int maxValue)
        {
            int i;
            for (i = 0; i < value; i++)
                greenList[i].draw(spriteBatch);
            for (; i < maxValue; i++)
                grayList[i].draw(spriteBatch);
        }

        /// <summary>
        /// Updates the counters
        /// </summary>
        internal void update()
        {
            m_collectedBoxes = m_player.Boxes;
            m_collectedFlares = m_player.FlaresNumber;
            m_turboCount = m_player.TurboLevel;
            m_lifesCount = m_player.Lifes;
            m_text[0] = "LEVEL " + m_player.CurrentLevel;
            m_text[1] = "POINTS " + (m_player.TotalPoints + m_player.Points);
            m_textLocations[0] = new Vector2(3 * m_size.Width + (m_size.Width - m_spriteFont.MeasureString(m_text[0]).X) / 2, m_textLocations[0].Y);
            m_textLocations[1] = new Vector2(3 * m_size.Width + (m_size.Width - m_spriteFont.MeasureString(m_text[1]).X) / 2, m_textLocations[1].Y);
        }

        /// <summary>
        /// Adds an item to items list
        /// </summary>
        /// <param name="gameStatusItem">Item to add</param>
        public void addItem(GameStatusItem gameStatusItem)
        {
            m_items.Add(gameStatusItem);
        }
        #endregion
    }
}
