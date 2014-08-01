using Meteo.Items;
using Meteo.Items.Predators.Animals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Levels
{
    /// <summary>
    /// Boss in the third level
    /// </summary>
    public class LevelThreeBoss : Level
    {
        #region variables
        /// <summary>
        /// Number of MutantCrabs
        /// </summary>
        internal int CrabsNumber;
        /// <summary>
        /// Max number of MutantCrabs
        /// </summary>
        private int MAX_CRABS = 1;
        #endregion

        #region constructors
        public LevelThreeBoss(Player player, int width, int height, int difficulty)
            : base(player, width, height, difficulty)
        {
            LevelId = LevelNumber.ThreeBoss;
            CrabsNumber = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads the content of current level
        /// </summary>
        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);

            generateMutantCrab();
            GameBoard.initializeBoxes(content, m_player.BoardPosition);
        }

        /// <summary>
        /// Updates LevelThreeBoss
        /// </summary>
        public override void update()
        {
            base.update();

            Random rand = new Random(DateTime.Now.Millisecond);
            int random = rand.Next(0, 10);

            if (CrabsNumber == MAX_CRABS)
                return;

            generateMutantCrab();
        }

        /// <summary>
        /// Generates MutantCrab
        /// </summary>
        private void generateMutantCrab()
        {
            int x = 0, y = 0;
            int itemWidth = GameBoard.BlockSize.Width * 2;
            int itemHeight = GameBoard.BlockSize.Height * 2;

            do
            {
                GameBoard.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_predators, itemWidth, itemHeight);
            } while (Math.Abs(x - m_player.BoardPosition.X) < 4 && Math.Abs(y - m_player.BoardPosition.Y) < 4);

            MutantCrab mutantCrab = new MutantCrab(Content.Load<Texture2D>("Items/Crab")
                , new Rectangle(x, y, itemWidth, itemHeight), this, m_player);
            m_predators.Add(mutantCrab);
            CrabsNumber++;
        }

        /// <summary>
        /// Lets know Level that MutantCrab was removed
        /// </summary>
        internal void removeCrab()
        {
            CrabsNumber--;
        }
        #endregion
    }
}
