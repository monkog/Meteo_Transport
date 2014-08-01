using Meteo.GameBoard;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Meteo.Items
{
    /// <summary>
    /// Base class for all items
    /// </summary>
    public abstract class Item
    {
        #region variables
        /// <summary>
        /// Size in px of an image
        /// </summary>
        public Size ItemSize { get; private set; }
        /// <summary>
        /// Image of an item
        /// </summary>
        protected Texture2D ItemImage { get; private set; }
        /// <summary>
        /// Determines whether the predator has been blinded
        /// </summary>
        protected bool m_shouldUpdate;

        /// <summary>
        /// Position on the board
        /// </summary>
        /// <remarks>Determines the coordinates in the array of the IceBlock that the current item is on</remarks>
        public Point BoardPosition { get; set; }
        /// <summary>
        /// Podition on the screen
        /// </summary>
        /// <remarks>Determines the coordinates of the Item on the screen</remarks>
        public Vector2 Position { get; set; }
        #endregion

        #region constructors
        public Item(Texture2D texture, Rectangle itemRectangle)
        {
            m_shouldUpdate = true;
            BoardPosition = new Point(itemRectangle.X, itemRectangle.Y);
            Position = new Vector2(BoardPosition.X * itemRectangle.Width, BoardPosition.Y * itemRectangle.Height);

            if (itemRectangle.Width == 0 || itemRectangle.Height == 0)
                ItemSize = new Size(ItemImage.Width, ItemImage.Height);
            else
                ItemSize = new Size(itemRectangle.Width, itemRectangle.Height);
            ItemImage = texture;
        }
        #endregion

        #region methods
        /// <summary>
        /// Draws the item
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ItemImage, new Rectangle((int)Position.X, (int)Position.Y, ItemSize.Width, ItemSize.Height), Color.White);
        }

        /// <summary>
        /// Checks the collision with other item
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns></returns>
        public bool collides(Item item, int x, int y)
        {
            Rectangle rectangle = new Rectangle((int)Position.X, (int)Position.Y, ItemSize.Width, ItemSize.Height);

            if (x == 0 && y == 0)
                return item != null && rectangle.Intersects(new Rectangle((int)item.Position.X
                    , (int)item.Position.Y, item.ItemSize.Width, item.ItemSize.Height));

            return item != null && (Math.Abs(item.BoardPosition.X - BoardPosition.X) <= x
                && Math.Abs(item.BoardPosition.Y - BoardPosition.Y) <= y);
        }

        /// <summary>
        /// Updates an item
        /// </summary>
        internal abstract void update();

        /// <summary>
        /// Can item be disposed
        /// </summary>
        /// <returns>Depending on whether item can be disposed</returns>
        internal abstract bool shouldDispose();
        #endregion
    }
}
