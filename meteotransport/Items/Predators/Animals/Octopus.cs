using Meteo.GameBoard;
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
    /// <summary>
    /// Predator that player fights with in second level
    /// </summary>
    public class Octopus : Predator
    {
        #region variables
        /// <summary>
        /// Lifes to take away while attacking
        /// </summary>
        private const int LIFES = 1;
        /// <summary>
        /// Boxes to take away while attacking
        /// </summary>
        private const int BOXES = 1;
        /// <summary>
        /// Time to live
        /// </summary>
        private const int LIFE_SECONDS = 10;

        /// <summary>
        /// Measures time till the end of Octopus's life time
        /// </summary>
        private Stopwatch m_lifeTimer;

        /// <summary>
        /// Determined whether the Octopus should dispose
        /// </summary>
        internal bool ShouldDispose { get; private set; }
        /// <summary>
        /// Current level
        /// </summary>
        private Level m_level;
        #endregion

        #region constructors
        public Octopus(Texture2D texture, Rectangle itemRectangle, Level level, Player player)
            : base(texture, itemRectangle, level, player)
        {
            m_lifeTimer = new Stopwatch();
            m_lifeTimer.Start();
            m_attackTimer.Start();
            m_timeElapsed = 0;
            m_update = false;
            ShouldDispose = false;
            m_level = level;
            MaxDistance = 0;
            Position = new Vector2(BoardPosition.X * itemRectangle.Width / 2, BoardPosition.Y * itemRectangle.Height / 2);
            //m_board.Items[BoardPosition.X, BoardPosition.Y + 1].Add(this);
            //m_board.Items[BoardPosition.X + 1, BoardPosition.Y].Add(this);
            //m_board.Items[BoardPosition.X + 1, BoardPosition.Y + 1].Add(this);
        }
        #endregion

        #region methods
        /// <summary>
        /// Reduces the speed of the ship
        /// </summary>
        private void reduceSpeed()
        {
            m_player.reduceSpeed();
        }

        /// <summary>
        /// Reduces player's number of boxes and lifes
        /// </summary>
        /// <remarks>
        /// Takes away one box and one life
        /// </remarks>
        private void shakeShip()
        {
            reduceBoxes(BOXES);
            reduceLifes(LIFES);
        }

        /// <summary>
        /// Attacks the player
        /// </summary>
        /// <remarks>Reduces player's life. Takes away two lifes</remarks>
        /// <param name="player">Player</param>
        public override void attack()
        {
            if (m_update && m_shouldUpdate)
            {
                shakeShip();
                m_update = false;
                m_attackTimer.Restart();
            }
            reduceSpeed();
        }

        /// <summary>
        /// Determines whether the following object should dispose
        /// </summary>
        /// <returns>Returns true if the elapsed time was greater than value of SECONDS</returns>
        internal override bool shouldDispose()
        {
            return ShouldDispose;
        }

        /// <summary>
        /// Updates the Octopus class
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

            if (m_lifeTimer.Elapsed.Seconds > LIFE_SECONDS)
            {
                m_player.getNormalSpeed();
                ShouldDispose = true;
                m_level.OctopusNumber--;
            }

            m_timeElapsed += m_attackTimer.Elapsed.Seconds;

            if (m_timeElapsed >= ATTACK_SECONDS)
            {
                m_timeElapsed = 0;
                m_update = true;
            }
        }

        /// <summary>
        /// Leaves on the board an OilBox
        /// </summary>
        internal override void leaveBonus()
        {
            leaveOilBox();
        }
        #endregion
    }
}
