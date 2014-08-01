using Meteo.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Meteo.GameBoard
{
    public class IceBlock : Item
    {
        #region variables
        /// <summary>
        /// How much is the IceBlock frozen
        /// </summary>
        public int FrozenLevel { get; protected set; }
        /// <summary>
        /// Did the explosion occure
        /// </summary>
        public bool IsBlind { get; protected set; }
        /// <summary>
        /// Max frozen level
        /// </summary>
        public static int MAX_FROZEN_LEVEL = 5;
        /// <summary>
        /// Frames to view from SpriteSheet
        /// </summary>
        public static int FRAMES = 5;
        /// <summary>
        /// Size of single frame
        /// </summary>
        public static Size FRAME_SIZE = new Size(100, 100);
        /// <summary>
        /// Controls freezing
        /// </summary>
        Stopwatch m_timer;
        #endregion

        #region constructors
        public IceBlock(Texture2D texture, Rectangle itemRectangle, int frozenLevel)
            : base(texture, itemRectangle)
        {
            FrozenLevel = frozenLevel;
            m_timer = new Stopwatch();
            m_timer.Start();
        }
        #endregion

        #region methods
        /// <summary>
        /// Sets the FrozenLevel to 0
        /// </summary>
        public void unfreeze()
        {
            FrozenLevel = 1;
            m_timer.Restart();
        }

        /// <summary>
        /// Increases the FrozenLevel
        /// </summary>
        private void freeze()
        {
            if (FrozenLevel < MAX_FROZEN_LEVEL)
                FrozenLevel++;
            else
                m_timer.Stop();
        }

        /// <summary>
        /// Updates elements in current class
        /// </summary>
        internal override void update()
        {
            if (m_timer.Elapsed.Seconds == 5)
            {
                FrozenLevel++;
                m_timer.Restart();
            }

            if (FrozenLevel == MAX_FROZEN_LEVEL)
                m_timer.Reset();
        }

        /// <summary>
        /// Determines whether object of the class should be disposed.
        /// </summary>
        /// <returns></returns>
        internal override bool shouldDispose()
        {
            return false;
        }

        /// <summary>
        /// Draws the Ice block
        /// </summary>
        /// <param name="spriteBatch">Sprite Batch</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ItemImage, new Vector2((int)Position.X, (int)Position.Y)
                , new Rectangle((FrozenLevel - 1) * FRAME_SIZE.Width, 0, 100, 100), Color.White, 0
                , Vector2.Zero, new Vector2(ItemSize.Width / 100f, ItemSize.Height / 100f), SpriteEffects.None, 0);
        }
        #endregion
    }
}
