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
using Meteo.GameBoard;

namespace Meteo.Levels
{
    /// <summary>
    /// Second level
    /// </summary>
    public class LevelTwo : Level
    {
        #region variables
        #endregion

        #region constructors
        public LevelTwo(Player player, int width, int height, int difficulty)
            : base(player, width, height, difficulty)
        {
            OctopusNumber = 0;
            LevelId = LevelNumber.Two;
            m_sharkNumber = 0;
        }
        #endregion

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
                RedPirate redPirate = new RedPirate(Content.Load<Texture2D>("Items/PirateShip")
                    , new Rectangle(x, y, itemWidth, itemHeight), this, m_player);
                m_predators.Add(redPirate);
            }

            GameBoard.initializeBoxes(content, m_player.BoardPosition);
        }

        /// <summary>
        /// Updates LevelTwo
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
        }
        #endregion
    }
}
