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
    /// Boss from second level
    /// </summary>
    public class MutantOctopus : Predator
    {
        #region variables
        /// <summary>
        /// Current level
        /// </summary>
        Level m_level;
        #endregion

        #region constructors
        public MutantOctopus(Texture2D texture, Rectangle itemRectangle, Level level, Player player)
            : base(texture, itemRectangle, level, player)
        {
            m_attackTimer.Start();
            m_timeElapsed = 0;
            m_update = true;
            MaxDistance = 0;
            Position = new Vector2(BoardPosition.X * itemRectangle.Width / 3, BoardPosition.Y * itemRectangle.Height / 3);
            m_level = level;
            //m_board.Items[BoardPosition.X, BoardPosition.Y + 1].Add(this);
            //m_board.Items[BoardPosition.X, BoardPosition.Y + 2].Add(this);
            //m_board.Items[BoardPosition.X + 1, BoardPosition.Y].Add(this);
            //m_board.Items[BoardPosition.X + 1, BoardPosition.Y + 1].Add(this);
            //m_board.Items[BoardPosition.X + 1, BoardPosition.Y + 2].Add(this);
            //m_board.Items[BoardPosition.X + 2, BoardPosition.Y].Add(this);
            //m_board.Items[BoardPosition.X + 2, BoardPosition.Y + 1].Add(this);
            //m_board.Items[BoardPosition.X + 2, BoardPosition.Y + 2].Add(this);
        }
        #endregion

        #region methods
        /// <summary>
        /// Reduces Player's lifes
        /// </summary>
        public override void attack()
        {
            if (m_update && m_shouldUpdate)
            {
                m_attackTimer.Restart();
                m_update = false;

                Spit spit = new Spit(Content.Load<Texture2D>("Items/Spit")
                    , new Rectangle((int)Position.X + ItemSize.Width / 2, (int)Position.Y + ItemSize.Height / 2, m_board.BlockSize.Width, m_board.BlockSize.Height)
                    , new Point((int)m_player.Position.X, (int)m_player.Position.Y)
                    , m_level, m_player, 3, Player.MAX_BOXES);

                m_level.m_spits.Add(spit);
            }
        }

        /// <summary>
        /// Updates MutantOctopus class
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
        }

        /// <summary>
        /// Whether MutantOctopus should dispose
        /// </summary>
        /// <returns>False</returns>
        internal override bool shouldDispose()
        {
            return false;
        }

        /// <summary>
        /// Empty method
        /// </summary>
        internal override void leaveBonus()
        { }
        #endregion
    }
}
