using Meteo.GameBoard;
using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Items.Predators.Animals
{
    class Shark : Predator
    {
        #region Fields
        /// <summary>
        /// Tiles to swim before termination
        /// </summary>
        internal int RemainingTiles;
        /// <summary>
        /// Max tiles to swim
        /// </summary>
        private const int MAX_TILES = 10;
        /// <summary>
        /// Lifes to take away when attacking
        /// </summary>
        private const int LIFES = 2;
        /// <summary>
        /// Did reach destination position
        /// </summary>
        private bool m_finishedMoving;
        /// <summary>
        /// Distance moved in every step
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
        /// <summary>
        /// Current Level
        /// </summary>
        Level m_level;
        #endregion

        #region constructors
        public Shark(Texture2D texture, Rectangle itemRectangle, Level level, Player player)
            : base(texture, itemRectangle, level, player)
        {
            m_speed = Player.NORMAL_SPEED * 0.3f;
            m_finishedMoving = true;
            m_update = false;
            RemainingTiles = MAX_TILES;
            MaxDistance = 0;
            m_level = level;
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
            if (!m_update && m_shouldUpdate)
            {
                reduceLifes(LIFES);
                RemainingTiles = 0;
                m_level.destroyShark();
                m_update = false;
            }
        }

        /// <summary>
        /// Determines whether the following object should dispose
        /// </summary>
        /// <returns>true if the number of remaining tiles to move is 0</returns>
        internal override bool shouldDispose()
        {
            return RemainingTiles <= 0;
        }

        /// <summary>
        /// Resuces the number of remaining tiles to swim
        /// </summary>
        private void swim()
        {
            RemainingTiles--;
            if (RemainingTiles <= 0)
                m_level.destroyShark();
        }

        /// <summary>
        /// Updates the Shark class
        /// </summary>
        /// <param name="m_keyboardState">Keyboard state</param>
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
                    BlindedSeconds = 0;
                    m_shouldUpdate = true;
                    IsBlinded = false;
                    m_stars = null;
                }
                return;
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
            //Point playerPosition = m_player.BoardPosition;
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
            swim();
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
