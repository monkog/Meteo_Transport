using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Meteo.Items
{
    /// <summary>
    /// Represents stars above blinded predator
    /// </summary>
    public class Stars : Item
    {
        #region variables
        /// <summary>
        /// Timer to count Stars time
        /// </summary>
        private Stopwatch m_timer;
        /// <summary>
        /// State of Stars
        /// </summary>
        internal int StarsLevel { get; set; }
        /// <summary>
        /// Size of single frame
        /// </summary>
        public static Size FRAME_SIZE = new Size(100, 100);
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Item's texture</param>
        /// <param name="itemRectangle">Rectangle to draw the texture</param>
        public Stars(Texture2D texture, Rectangle itemRectangle)
            : base(texture, itemRectangle)
        {
            Position = new Vector2(itemRectangle.X, itemRectangle.Y);
            m_timer = new Stopwatch();
            m_timer.Start();
            StarsLevel = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates an item
        /// </summary>
        internal override void update()
        {
            if (m_timer.Elapsed.Milliseconds >= 100)
            {
                StarsLevel = (StarsLevel + 1) % 5;
                m_timer.Restart();
                Console.WriteLine(StarsLevel);
            }
        }

        /// <summary>
        /// Can item be disposed
        /// </summary>
        /// <returns>Returns false</returns>
        internal override bool shouldDispose()
        {
            return false;
        }

        /// <summary>
        /// Draws the Stars
        /// </summary>
        /// <param name="spriteBatch">Sprite Batch</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ItemImage, new Vector2((int)Position.X, (int)Position.Y)
                , new Rectangle(StarsLevel* FRAME_SIZE.Width, 0, 100, 100), Color.White, 0
                , Vector2.Zero, new Vector2(ItemSize.Width / 100f, ItemSize.Height / 100f), SpriteEffects.None, 0);
        }
        #endregion
    }
}
