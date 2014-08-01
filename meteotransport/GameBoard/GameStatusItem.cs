using Meteo.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.GameBoard
{
    /// <summary>
    /// Represents items in GameStatus
    /// </summary>
    public class GameStatusItem
    {
        #region variables
        /// <summary>
        /// Size of an item
        /// </summary>
        public Size Size { get; private set; }
        /// <summary>
        /// Imageof an item
        /// </summary>
        private Texture2D m_itemImage;

        /// <summary>
        /// Position
        /// </summary>
        public Vector2 Position { get; private set; }
        #endregion

        #region constructors
        public GameStatusItem(Texture2D texture, Size size, Vector2 position)
        {
            m_itemImage = texture;
            Size = size;
            Position = position;
        }
        #endregion

        #region methods
        /// <summary>
        /// Draws the item
        /// </summary>
        /// <param name="spriteBatch">Sprite batch</param>
        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_itemImage, new Rectangle((int)Position.X, (int)Position.Y, Size.Width, Size.Height), Color.White);
        }

        /// <summary>
        /// Updates the item
        /// </summary>
        /// <param name="m_keyboardState">Keyboard state</param>
        internal void update(KeyboardState m_keyboardState)
        { }
        #endregion
    }
}
