using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Meteo.Items.Predators.Animals
{
    class MutantShark : Predator
    {
        #region variables
        /// <summary>
        /// Whether MutantShark should disappear
        /// </summary>
        internal bool ShouldDispose { get; set; }
        /// <summary>
        /// Lifes that takes away fromk player when attacking
        /// </summary>
        private const int LIFES = 4;
        #endregion

        #region constructors
        public MutantShark(Texture2D texture, Rectangle itemRectangle, Level level, Player player)
            : base(texture, itemRectangle, level, player)
        {
            m_update = true;
            ShouldDispose = false;
            MaxDistance = 0;
            Position = new Vector2(BoardPosition.X * itemRectangle.Width / 2, BoardPosition.Y * itemRectangle.Height / 2);
        }
        #endregion

        #region methods
        /// <summary>
        /// Attacks the player
        /// </summary>
        /// <remarks>Reduces player's life. Takes away two lifes</remarks>
        /// <param name="player">Player</param>
        public override void attack()
        {
            if (m_update)
            {
                m_player.reduceLifes(LIFES);
                m_update = false;
            }
        }

        /// <summary>
        /// Determines whether the following object should dispose
        /// </summary>
        /// <returns>true if the number of remaining tiles to move is 0</returns>
        internal override bool shouldDispose()
        {
            return ShouldDispose;
        }

        /// <summary>
        /// Updates the MutantShark class
        /// </summary>
        internal override void update()
        {
            base.update();
            if (!m_shouldUpdate)
            {
                BlindedSeconds += m_blindTimer.Elapsed.Milliseconds;
                m_blindTimer.Restart();
                if (BlindedSeconds > BLIND)
                {
                    m_blindTimer.Stop();
                    m_shouldUpdate = true;
                    IsBlinded = false;
                    m_stars = null;
                }
                return;
            }
        }

        /// <summary>
        /// Does nothing, as MutantShark doesn't leave any bonus
        /// </summary>
        internal override void leaveBonus()
        { }
        #endregion
    }
}
