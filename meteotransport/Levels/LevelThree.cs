using Meteo.Items;
using Meteo.Items.Predators;
using Meteo.Items.Predators.Animals;
using Meteo.Items.Predators.Pirates;
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
    /// Third level
    /// </summary>
    public class LevelThree : Level
    {
        #region variables
        /// <summary>
        /// Max number of crabs in game
        /// </summary>
        private int MAX_CRABS = 3;

        /// <summary>
        /// Number of crabs in game;
        /// </summary>
        public int CrabsNumber { get; private set; }
        #endregion

        public LevelThree(Player player, int width, int height, int difficulty)
            : base(player, width, height, difficulty)
        {
            MAX_CRABS = difficulty;
            CrabsNumber = 0;
            LevelId = LevelNumber.Three;
        }

        #region Methods
        /// <summary>
        /// Loads the content of current level
        /// </summary>
        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);

            int x = 0, y = 0;
            int itemWidth = GameBoard.BlockSize.Width;
            int itemHeight = GameBoard.BlockSize.Height;

            for (int i = 0; i < Difficulty; i++)
            {
                GameBoard.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_predators, itemWidth, itemHeight);
                BlackPirate blackPirate = new BlackPirate(Content.Load<Texture2D>("Items/PirateShip"), new Rectangle(x, y, itemWidth, itemHeight), this, m_player);
                m_predators.Add(blackPirate);
            }

            GameBoard.initializeBoxes(content, m_player.BoardPosition);
        }
        /// <summary>
        /// Updates LevelThree
        /// </summary>
        public override void update()
        {
            base.update();

            generateShark();

            bool octopusCollides = false;

            foreach (Predator predator in m_predators)
                if (predator.GetType() == typeof(Octopus))
                    if (m_player.Speed != Player.TURBO_SPEED && predator.collides(m_player, 0, 0))
                        octopusCollides = true;

            if (octopusCollides == true)
                m_player.reduceSpeed();
            else
                m_player.getNormalSpeed();

            generateOctopus();

            Random random = new Random(DateTime.Now.Millisecond);
            if (random.Next(0, 1000) > MAX_CRABS || CrabsNumber == MAX_CRABS)
                return;

            CrabsNumber++;
            int x = 0, y = 0;
            int itemWidth = GameBoard.BlockSize.Width;
            int itemHeight = GameBoard.BlockSize.Height;

            GameBoard.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_predators, itemWidth, itemHeight);
            Crab crab = new Crab(Content.Load<Texture2D>("Items/Crab"), new Rectangle(x, y, itemWidth, itemHeight), this, m_player);
            m_predators.Add(crab);
        }
        #endregion
    }
}
