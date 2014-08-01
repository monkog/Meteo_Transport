using Meteo.GameBoard;
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
    /// <summary>
    /// Second level with boss
    /// </summary>
    public class LevelTwoBoss : Level
    {
        #region variables
        #endregion

        #region constructors
        public LevelTwoBoss(Player player, int width, int height, int difficulty)
            : base(player, width, height, difficulty)
        {
            LevelId = LevelNumber.TwoBoss;
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
            int itemWidth = GameBoard.BlockSize.Width * 3;
            int itemHeight = GameBoard.BlockSize.Height * 3;

            for (int i = 0; i < 2; i++)
            {
                do
                {
                    GameBoard.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_predators, itemWidth, itemHeight);
                } while (!(x < Board.WIDTH - 2 && y < Board.HEIGHT - 2 && Math.Abs(x - m_player.BoardPosition.X) + Math.Abs(y - m_player.BoardPosition.Y) > 6));
                MutantOctopus mutantOctopus = new MutantOctopus(content.Load<Texture2D>("Items/Octopus")
                    , new Rectangle(x, y, itemWidth, itemHeight), this, m_player);
                m_predators.Add(mutantOctopus);
            }

            GameBoard.initializeBoxes(content, m_player.BoardPosition);
        }

        /// <summary>
        /// Updates LevelTwo with Boss
        /// </summary>
        public override void update()
        {
            base.update();
        }
        #endregion
    }
}
