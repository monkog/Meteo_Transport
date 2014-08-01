using Meteo.Items;
using Meteo.Items.Predators;
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
    public class LevelOne : Level
    {
        #region variables
        #endregion

        public LevelOne(Player player, int width, int height, int difficulty)
            : base(player, width, height, difficulty)
        {
            LevelId = LevelNumber.One;
            m_sharkNumber = 0;
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
            Texture2D texture = content.Load<Texture2D>("Items/PirateShip");

            for (int i = 0; i < Difficulty; i++)
            {
                GameBoard.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_predators, itemWidth, itemHeight);
                GreenPirate predator = new GreenPirate(texture, new Rectangle(x, y, itemWidth, itemHeight), this, m_player);
                m_predators.Add(predator);
            }

            GameBoard.initializeBoxes(content, m_player.BoardPosition);
        }

        /// <summary>
        /// Updates the level items
        /// </summary>
        public override void update()
        {
            base.update();
            generateShark();
        }
        #endregion
    }
}
