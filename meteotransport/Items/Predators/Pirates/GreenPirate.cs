﻿using Meteo.GameBoard;
using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Meteo.Items.Predators
{
    class GreenPirate : Predator
    {
        #region variables
        /// <summary>
        /// Number of boxes to take away when attacking
        /// </summary>
        private const int NUMBER_OF_BOXES = 1;

        /// <summary>
        /// Did the GreenPirate get to his target BoardPosition
        /// </summary>
        private bool m_finishedMoving;
        /// <summary>
        /// Step of the pirate in every loop
        /// </summary>
        private Vector2 m_step;
        /// <summary>
        /// Target BoardPosition
        /// </summary>
        private Vector2 m_destination;
        /// <summary>
        /// Direction of movement
        /// </summary>
        private Point m_direction;
        #endregion

        #region constructors
        public GreenPirate(Texture2D texture, Rectangle itemRectangle, Level level, Player player)
            : base(texture, itemRectangle, level, player)
        {
            m_speed = Player.NORMAL_SPEED * 0.5f;
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
        /// <remarks>Takes away two of Player's MeteorBoxes and places them randomly on the board</remarks>
        public override void attack()
        {
            if (m_update && m_shouldUpdate)
            {
                m_attackTimer.Restart();
                m_update = false;
                reduceBoxes( NUMBER_OF_BOXES);
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
                    IsBlinded = false;
                    m_stars = null;
                }
                return;
            }

            m_timeElapsed += m_attackTimer.Elapsed.Seconds;

            if (m_timeElapsed >= ATTACK_SECONDS)
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
