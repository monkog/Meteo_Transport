using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Items.Predators.Pirates
{
    /// <summary>
    /// Predator from second level
    /// </summary>
    public class RedPirate : Predator
    {
        #region variables
        /// <summary>
        /// Number of boxes to take away when attacking
        /// </summary>
        private const int NUMBER_OF_BOXES = 1;
        /// <summary>
        /// Max distance from Player to be safe
        /// </summary>
        private const int MAX_TILES = 2;
        /// <summary>
        /// Lifes to take away when attacking
        /// </summary>
        private const int LIFES = 2;

        /// <summary>
        /// Did Pirate reach goal position
        /// </summary>
        private bool m_finishedMoving;
        /// <summary>
        /// Movement in every step
        /// </summary>
        private Vector2 m_step;
        /// <summary>
        /// Destination position
        /// </summary>
        private Vector2 m_destination;
        /// <summary>
        /// Direction to move
        /// </summary>
        private Point m_direction;
        #endregion

        #region constructors
        public RedPirate(Texture2D texture, Rectangle itemRectangle, Level level, Player player)
            : base(texture, itemRectangle, level, player)
        {
            m_speed = Player.NORMAL_SPEED * 0.6f;
            m_update = true;
            m_finishedMoving = true;
            m_timeElapsed = 0;
            m_attackTimer.Start();
            MaxDistance = 0;
        }
        #endregion

        #region methods
        /// <summary>
        /// Attacks the player
        /// </summary>
        /// <remarks>Takes away two of Player's MeteoBoxes and places them randomly on the board.
        /// If Player is out of boxes, takes away two lifes.
        /// </remarks>
        public override void attack()
        {
            if (m_update && m_shouldUpdate)
            {
                m_attackTimer.Restart();
                m_update = false;
                if (m_player.Boxes > 0)
                    reduceBoxes(NUMBER_OF_BOXES);
                else
                    reduceLifes(LIFES);
            }
        }

        /// <summary>
        /// Updates the GreenPirate class
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
                    m_stars = null;
                }
                return;
            }

            m_timeElapsed += m_attackTimer.Elapsed.Seconds;

            if (m_timeElapsed >= 3)
            {
                m_timeElapsed = 0;
                m_update = true;
            }

            if (m_finishedMoving)
            {
                createPath(false);
                checkDirection();
            }
            else
            {
                Position = new Vector2(Position.X + m_step.X, Position.Y + m_step.Y);
                if ((m_direction.X > 0 && Position.X > m_destination.X) ||
                    (m_direction.X < 0 && Position.X < m_destination.X)
                    || (m_direction.Y > 0 && Position.Y > m_destination.Y) ||
                    (m_direction.Y < 0 && Position.Y < m_destination.Y))
                    Position = new Vector2(m_destination.X, m_destination.Y);
                else
                    return;
                m_finishedMoving = true;
            }
        }

        /// <summary>
        /// Checks the direction to move
        /// </summary>
        private void checkDirection()
        {
            if (PredatorPath.Count < 2)
                return;
            BoardPosition = PredatorPath[0];
            Point playerPosition = PredatorPath[1];

            if ((playerPosition.X != BoardPosition.X) || (playerPosition.Y != BoardPosition.Y))
            {
                if (Math.Abs(playerPosition.X - BoardPosition.X) > Math.Abs(playerPosition.Y - BoardPosition.Y))
                    if (playerPosition.X > BoardPosition.X)
                        m_direction = new Point(1, 0);
                    else
                        m_direction = new Point(-1, 0);
                else if (playerPosition.Y > BoardPosition.Y)
                    m_direction = new Point(0, 1);
                else
                    m_direction = new Point(0, -1);
            }
            else
                m_direction = new Point(0, 0);

            m_board.Items[BoardPosition.X, BoardPosition.Y].Remove(this);
            BoardPosition = new Point(BoardPosition.X + m_direction.X, BoardPosition.Y + m_direction.Y);
            m_board.Items[BoardPosition.X, BoardPosition.Y].Add(this);

            m_destination = new Vector2(Position.X + m_direction.X * m_board.BlockSize.Width
                , Position.Y + m_direction.Y * m_board.BlockSize.Height);
            Position = new Vector2(Position.X + m_direction.X * m_speed, Position.Y + m_direction.Y * m_speed);
            m_step = new Vector2(m_direction.X * m_speed, m_direction.Y * m_speed);
            m_finishedMoving = false;
        }

        /// <summary>
        /// Determines wehether  the GreenPirate should dispose
        /// </summary>
        /// <returns>Returns false as GreenPirate shouldn't dispose on any condition</returns>
        internal override bool shouldDispose()
        {
            return false;
        }

        /// <summary>
        /// Leaves the pirate bonus
        /// </summary>
        internal override void leaveBonus()
        {
            leavePirateBox();
        }
        #endregion
    }
}
