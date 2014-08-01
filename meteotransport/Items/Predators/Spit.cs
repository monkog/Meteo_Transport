using Meteo.Items.Predators;
using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Items
{
    /// <summary>
    /// Spit thrown by MutantOctopus also used for generating BlackPirate's flares 
    /// </summary>
    public class Spit : Predator
    {
        #region variables
        /// <summary>
        /// Lifes that takes away from player when attacking
        /// </summary>
        internal int m_lifes;
        /// <summary>
        /// Boxes that takes away when attacking
        /// </summary>
        internal int m_boxes;
        /// <summary>
        /// Destination position
        /// </summary>
        internal Point EndPoint { get; set; }
        /// <summary>
        /// Distance to move in every step
        /// </summary>
        private Vector2 m_step;
        /// <summary>
        /// Can item be disposed
        /// </summary>
        private bool m_shouldDispose;
        /// <summary>
        /// How much did the spit move
        /// </summary>
        private double m_distance;
        /// <summary>
        /// How much does the spit have to move
        /// </summary>
        private double m_pathLength;
        /// <summary>
        /// Speed of the spit
        /// </summary>
        private static int SPEED = 3;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Item's texture</param>
        /// <param name="itemRectangle">Rectangle to draw the texture</param>
        public Spit(Texture2D texture, Rectangle itemRectangle, Point endPoint, Level level, Player player, int lifes, int boxes)
            : base(texture, itemRectangle, level, player)
        {
            Position = new Vector2(itemRectangle.X, itemRectangle.Y);
            m_shouldDispose = false;
            EndPoint = endPoint;
            m_lifes = lifes;
            m_boxes = boxes;

            if (Position.X == endPoint.X)
                m_step = new Vector2(0, SPEED * Math.Sign(endPoint.Y - Position.Y));
            else if (Position.Y == endPoint.Y)
                m_step = new Vector2(SPEED * Math.Sign(endPoint.X - Position.X), 0);
            else
            {
                float a, dx, dy;
                float x = Math.Sign(endPoint.X - Position.X);
                dx = endPoint.X - Position.X;
                dy = endPoint.Y - Position.Y;
                a = dy / dx;
                x = (float)(x * SPEED / (Math.Sqrt(a * a + 1)));
                m_step = new Vector2(x, a * x);
            }

            m_distance = 0;
            m_pathLength = Math.Sqrt(Math.Pow(endPoint.X - Position.X, 2) + Math.Pow(endPoint.Y - Position.Y, 2));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Reduces Player's lifes
        /// </summary>
        public override void attack()
        {
            reduceLifes(m_lifes);
            reduceBoxes(m_boxes);
            m_shouldDispose = true;
        }

        /// <summary>
        /// Updates an item
        /// </summary>
        internal override void update()
        {
            Position = new Vector2(Position.X + m_step.X, Position.Y + m_step.Y);
            m_distance += Math.Sqrt(Math.Pow(m_step.X, 2) + Math.Pow(m_step.Y, 2));
            if (m_distance >= m_pathLength)
                m_shouldDispose = true;
        }

        /// <summary>
        /// Can item be disposed
        /// </summary>
        /// <returns>Depending on whether item can be disposed</returns>
        internal override bool shouldDispose()
        {
            return m_shouldDispose;
        }

        /// <summary>
        /// Empty method
        /// </summary>
        internal override void leaveBonus()
        { }

        /// <summary>
        /// Prevents from blinding the spit
        /// </summary>
        /// <param name="realPath">Flare real path</param>
        /// <param name="maxPath">Max flare path</param>
        internal virtual void blind(double realPath, double maxPath)
        { }
        #endregion
    }
}
