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
    /// Predator from third level
    /// </summary>
    public class BlackPirate : Predator
    {
        #region Fields
        /// <summary>
        /// Current level
        /// </summary>
        Level m_level;

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
        public BlackPirate(Texture2D texture, Rectangle itemRectangle, Level level, Player player)
            : base(texture, itemRectangle, level, player)
        {
            m_speed = Player.NORMAL_SPEED * 0.5f;
            MaxDistance = 3;
            m_level = level;
            m_update = true;
            m_timeElapsed = 0;
            m_attackTimer.Start();
            m_finishedMoving = true;
        }
        #endregion

        #region methods
        /// <summary>
        /// Updates the pirate
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
        /// Attacks the Player
        /// </summary>
        public override void attack()
        {
            if (m_update && m_shouldUpdate)
            {
                Spit spit = new Spit(Content.Load<Texture2D>("Items/Flare")
                    , new Rectangle((int)Position.X + ItemSize.Width / 2, (int)Position.Y + ItemSize.Height / 2, m_board.BlockSize.Width, m_board.BlockSize.Height)
                    , new Point((int)m_player.Position.X, (int)m_player.Position.Y)
                    , m_level, m_player, 1, 1);
                m_level.m_spits.Add(spit);

                m_attackTimer.Restart();
                m_update = false;
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
        /// Can Pirate be disposed
        /// </summary>
        /// <returns>Returns false</returns>
        internal override bool shouldDispose()
        {
            return false;
        }

        /// <summary>
        /// Leaves bonus box
        /// </summary>
        internal override void leaveBonus()
        {
            leavePirateBox();
        }
        #endregion
    }
}
