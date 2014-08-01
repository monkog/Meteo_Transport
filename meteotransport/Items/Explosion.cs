using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Meteo.Items
{
    class Explosion : Item
    {
        #region variables
        /// <summary>
        /// How much time does the explosion last
        /// </summary>
        private const int EXPLOSION_TIME = 5;
        /// <summary>
        /// Timer to count explosion time
        /// </summary>
        private Stopwatch m_timer;
        /// <summary>
        /// Level of the explosion
        /// </summary>
        internal int ExplosionLevel { get; set; }
        /// <summary>
        /// Size of single frame
        /// </summary>
        public static Size FRAME_SIZE = new Size(100, 100);
        /// <summary>
        /// Time elapsed from the last change of SpriteSheet frame
        /// </summary>
        int m_elapsedTime;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Item's texture</param>
        /// <param name="itemRectangle">Rectangle to draw the texture</param>
        public Explosion(Texture2D texture, Rectangle itemRectangle)
            : base(texture, itemRectangle)
        {
            Position = new Vector2(itemRectangle.X, itemRectangle.Y);
            m_timer = new Stopwatch();
            m_timer.Start();
            ExplosionLevel = 0;
            m_elapsedTime = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates an item
        /// </summary>
        internal override void update()
        {
            int elapsedMilliseconds = m_timer.Elapsed.Milliseconds;

            if (elapsedMilliseconds - m_elapsedTime >= 50)
            {
                ExplosionLevel++;
                m_elapsedTime += 50;
            }
        }

        /// <summary>
        /// Can item be disposed
        /// </summary>
        /// <returns>Depending on whether item can be disposed</returns>
        internal override bool shouldDispose()
        {
            return m_timer.Elapsed.Seconds > EXPLOSION_TIME || ExplosionLevel > 5;
        }

        /// <summary>
        /// Draws the explosion
        /// </summary>
        /// <param name="spriteBatch">Sprite Batch</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ItemImage, new Vector2((int)Position.X, (int)Position.Y)
                , new Rectangle((ExplosionLevel - 1) * FRAME_SIZE.Width, 0, 100, 100), Color.White, 0
                , Vector2.Zero, new Vector2(ItemSize.Width / 100f, ItemSize.Height / 100f), SpriteEffects.None, 0);
        }
        #endregion
    }
}
