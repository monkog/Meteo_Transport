using Meteo.GameBoard;
using Meteo.Items;
using Meteo.Items.Predators.Animals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Meteo.Levels
{
    class LevelOneBoss : Level
    {
        #region variables
        /// <summary>
        /// Time that Shark is under/above water
        /// </summary>
        private const int ELAPSED_TIME = 900;

        /// <summary>
        /// Last player position
        /// </summary>
        private Point m_lastPlayerPosition;
        /// <summary>
        /// Tracks Shark's time under/above water
        /// </summary>
        private Stopwatch m_timer;
        /// <summary>
        /// Shark position
        /// </summary>
        /// <remarks>Possible values are UnderWater and AboveWater</remarks>
        private SharkPosition m_position;
        #endregion

        enum SharkPosition
        {
            UnderWater,
            AboveWater
        }

        #region constructors
        public LevelOneBoss(Player player, int width, int height, int difficulty)
            : base(player, width, height, difficulty)
        {
            m_lastPlayerPosition = player.BoardPosition;
            LevelId = LevelNumber.OneBoss;
            m_position = SharkPosition.AboveWater;
            m_timer = new Stopwatch();
            m_timer.Start();
        }
        #endregion

        #region methods
        /// <summary>
        /// Loads the content of current level
        /// </summary>
        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            int itemWidth = GameBoard.BlockSize.Width * 2;
            int itemHeight = GameBoard.BlockSize.Height * 2;

            MutantShark predator = new MutantShark(Content.Load<Texture2D>("Items/Shark")
                , new Rectangle(m_lastPlayerPosition.X - 5, m_lastPlayerPosition.Y - 5, itemWidth, itemHeight), this, m_player);
            m_predators.Add(predator);

            predator = new MutantShark(Content.Load<Texture2D>("Items/Shark")
                , new Rectangle(m_lastPlayerPosition.X + 3, m_lastPlayerPosition.Y + 3, itemWidth, itemHeight), this, m_player);
            m_predators.Add(predator);

            GameBoard.initializeBoxes(content, m_player.BoardPosition);
        }

        /// <summary>
        /// Updates the level items
        /// </summary>
        public override void update()
        {
            base.update();

            if (m_timer.Elapsed.Milliseconds > ELAPSED_TIME)
            {
                int itemWidth = GameBoard.BlockSize.Width;
                int itemHeight = GameBoard.BlockSize.Height;
                int x = Math.Min(Board.WIDTH - 2, m_lastPlayerPosition.X);
                int y = Math.Min(Board.HEIGHT - 2, m_lastPlayerPosition.Y);

                MutantShark predator = new MutantShark(Content.Load<Texture2D>("Items/Shark")
                    , new Rectangle(x, y, itemWidth * 2, itemHeight * 2), this, m_player);
                m_predators.Add(predator);

                MutantShark mutantShark = m_predators[0] as MutantShark;
                mutantShark.ShouldDispose = true;
                m_lastPlayerPosition = m_player.BoardPosition;
                
                m_timer.Restart();
                m_position = (SharkPosition)(((int)m_position + 1) % 2);
            }
        }
        #endregion
    }
}
