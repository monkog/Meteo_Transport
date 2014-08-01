using Meteo.GameBoard;
using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Items.Predators.Animals
{
    /// <summary>
    /// Boss from third level
    /// </summary>
    public class MutantCrab : Predator
    {
        #region variables
        /// <summary>
        /// Lifes to take away when attacking
        /// </summary>
        private int LIFES = Player.MAX_LIFES;

        /// <summary>
        /// Did the GreenPirate get to his target BoardPosition
        /// </summary>
        private bool m_finishedMoving;
        /// <summary>
        /// Step of the pirate in every loop
        /// </summary>
        internal Vector2 Step;
        /// <summary>
        /// Target BoardPosition
        /// </summary>
        internal Vector2 Destination;
        /// <summary>
        /// Direction of movement
        /// </summary>
        internal Point Direction;
        /// <summary>
        /// Ice blocks surrounding MutantCrab
        /// </summary>
        private List<IceBlock> m_iceBlocks;
        /// <summary>
        /// Current level
        /// </summary>
        private LevelThreeBoss m_level;
        /// <summary>
        /// Did the MutantCrab change direction
        /// </summary>
        internal bool ChangedDirection;
        /// <summary>
        /// Should MutantCrab dispose
        /// </summary>
        private bool m_shouldDispose;
        #endregion

        #region constructors
        public MutantCrab(Texture2D texture, Rectangle itemRectangle, Level level, Player player)
            : base(texture, itemRectangle, level, player)
        {
            m_speed = Player.NORMAL_SPEED * 0.5f;
            m_shouldDispose = false;
            m_update = true;
            m_finishedMoving = true;
            m_timeElapsed = 0;
            m_attackTimer.Start();
            m_level = (LevelThreeBoss)level;
            MaxDistance = 0;
            Position = new Vector2(BoardPosition.X * itemRectangle.Width / 2, BoardPosition.Y * itemRectangle.Height / 2);
            if (Math.Abs(m_player.Position.X - Position.X) < Math.Abs(m_player.Position.Y - Position.Y))
                Direction = new Point(0, Math.Sign(m_player.Position.Y - Position.Y));
            else
                Direction = new Point(Math.Sign(m_player.Position.X - Position.X), 0);
            ChangedDirection = false;

            m_iceBlocks = new List<IceBlock>();
            addIce(BoardPosition.X, BoardPosition.Y);
            addIce(BoardPosition.X, BoardPosition.Y + 1);
            addIce(BoardPosition.X + 1, BoardPosition.Y);
            addIce(BoardPosition.X + 1, BoardPosition.Y + 1);

            addIce(BoardPosition.X - 1, BoardPosition.Y - 1);
            addIce(BoardPosition.X, BoardPosition.Y - 1);
            addIce(BoardPosition.X + 1, BoardPosition.Y - 1);
            addIce(BoardPosition.X + 2, BoardPosition.Y - 1);
            addIce(BoardPosition.X + 2, BoardPosition.Y);
            addIce(BoardPosition.X + 2, BoardPosition.Y + 1);
            addIce(BoardPosition.X + 2, BoardPosition.Y + 2);
            addIce(BoardPosition.X + 1, BoardPosition.Y + 2);
            addIce(BoardPosition.X, BoardPosition.Y + 2);
            addIce(BoardPosition.X - 1, BoardPosition.Y + 2);
            addIce(BoardPosition.X - 1, BoardPosition.Y + 1);
            addIce(BoardPosition.X - 1, BoardPosition.Y);
        }
        #endregion

        #region methods
        /// <summary>
        /// Adds IceBlocks to list
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        void addIce(int x, int y)
        {
            IceBlock ice = m_board.getIceBlock(x, y);
            if (ice != null)
                m_iceBlocks.Add(ice);
        }

        private void throwIce(Point from, Point to)
        { }

        public override void attack()
        {
            if (m_update && m_shouldUpdate)
            {
                m_attackTimer.Restart();
                m_update = false;
                reduceLifes(LIFES);
            }
        }

        internal override void leaveBonus()
        { }

        /// <summary>
        /// Updates the class
        /// </summary>
        internal override void update()
        {
            base.update();
            if (!m_shouldUpdate)
            {
                if (m_blindTimer.Elapsed.Seconds > BLIND)
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

            if (m_iceBlocks.Count > 0)
            {
                if (m_update)
                {
                    IceBlock ice = m_iceBlocks[0];
                    m_iceBlocks.Remove(m_iceBlocks[0]);

                    m_board.IceBlocks[ice.BoardPosition.X, ice.BoardPosition.Y] = null;
                    m_attackTimer.Restart();

                    Spit iceBlock = new Spit(Content.Load<Texture2D>("Items/IceBlock")
                        , new Rectangle((int)Position.X + ItemSize.Width / 2, (int)Position.Y + ItemSize.Height / 2, m_board.BlockSize.Width, m_board.BlockSize.Height)
                        , new Point((int)m_player.Position.X, (int)m_player.Position.Y), m_level, m_player, Player.MAX_LIFES, 0);

                    m_level.m_spits.Add(iceBlock);
                    m_update = false;
                }
                if (m_iceBlocks.Count == 0)
                    if (Math.Abs(m_player.Position.X - Position.X) < Math.Abs(m_player.Position.Y - Position.Y))
                        Direction = new Point(0, Math.Sign(m_player.Position.Y - Position.Y));
                    else
                        Direction = new Point(Math.Sign(m_player.Position.X - Position.X), 0);
                return;
            }

            if (m_finishedMoving)
                checkDirection();
            else
            {
                //if (!m_changedDirection)
                //    if ((m_player.Position.X > Position.X && m_player.Position.X < Position.X + ItemSize.Width)
                //        || (m_player.Position.X + m_player.ItemSize.Width > Position.X && m_player.Position.X + m_player.ItemSize.Width < Position.X + ItemSize.Width))
                //    {
                //        m_direction = new Point(0, Math.Sign(m_player.Position.Y - Position.Y));
                //        m_changedDirection = true;
                //    }
                //    else if ((m_player.Position.Y > Position.Y && m_player.Position.Y < Position.Y + ItemSize.Height)
                //        || (m_player.Position.Y + m_player.ItemSize.Height > Position.Y && m_player.Position.Y + m_player.ItemSize.Height < Position.Y + ItemSize.Height))
                //    {
                //        m_direction = new Point(Math.Sign(m_player.Position.X - Position.X), 0);
                //        m_changedDirection = true;
                //    }
                Position = new Vector2(Position.X + Step.X, Position.Y + Step.Y);
                if ((Direction.X > 0 && Position.X > Destination.X) ||
                    (Direction.X < 0 && Position.X < Destination.X)
                    || (Direction.Y > 0 && Position.Y > Destination.Y) ||
                    (Direction.Y < 0 && Position.Y < Destination.Y))
                    Position = new Vector2(Destination.X, Destination.Y);
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
            Point playerPosition = m_player.BoardPosition;
            //if (!m_changedDirection)
            //    if ((m_player.Position.X > Position.X && m_player.Position.X < Position.X + ItemSize.Width)
            //            || (m_player.Position.X + m_player.ItemSize.Width > Position.X && m_player.Position.X + m_player.ItemSize.Width < Position.X + ItemSize.Width))
            //    {
            //        m_direction = new Point(0, Math.Sign(m_player.Position.Y - Position.Y));
            //        m_changedDirection = true;
            //    }
            //    else if ((m_player.Position.Y > Position.Y && m_player.Position.Y < Position.Y + ItemSize.Height)
            //        || (m_player.Position.Y + m_player.ItemSize.Height > Position.Y && m_player.Position.Y + m_player.ItemSize.Height < Position.Y + ItemSize.Height))
            //    {
            //        m_direction = new Point(Math.Sign(m_player.Position.X - Position.X), 0);
            //        m_changedDirection = true;
            //    }

            m_board.Items[BoardPosition.X, BoardPosition.Y].Remove(this);
            BoardPosition = new Point(BoardPosition.X + Direction.X, BoardPosition.Y + Direction.Y);

            if (BoardPosition.X > Board.WIDTH || BoardPosition.Y > Board.HEIGHT || BoardPosition.X < 0 || BoardPosition.Y < 0
                || Position.X < 0 || Position.X + ItemSize.Width > m_board.Size.Width || Position.Y < 0 || Position.Y + ItemSize.Height > m_board.Size.Height)
            {
                BoardPosition = new Point(BoardPosition.X - Direction.X, BoardPosition.Y - Direction.Y);
                IceBlock ice = m_board.IceBlocks[BoardPosition.X, BoardPosition.Y];
                if (ice != null)
                    ice.unfreeze();
                m_shouldDispose = true;
                m_shouldUpdate = false;
                m_level.removeCrab();
                return;
            }

            m_board.Items[BoardPosition.X, BoardPosition.Y].Add(this);

            Destination = new Vector2(Position.X + Direction.X * m_board.BlockSize.Width
                , Position.Y + Direction.Y * m_board.BlockSize.Height);
            Position = new Vector2(Position.X + Direction.X * m_speed, Position.Y + Direction.Y * m_speed);
            Step = new Vector2(Direction.X * m_speed, Direction.Y * m_speed);
            m_finishedMoving = false;
        }

        /// <summary>
        /// Should object be disposed
        /// </summary>
        /// <returns>Returns value of m_shouldDispose</returns>
        internal override bool shouldDispose()
        {
            return m_shouldDispose;
        }
        #endregion
    }
}
