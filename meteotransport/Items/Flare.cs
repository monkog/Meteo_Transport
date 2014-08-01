using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Items
{
    /// <summary>
    /// Flare 
    /// </summary>
    public class Flare : Item
    {
        #region variables
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
        /// How much did the flare move
        /// </summary>
        internal double m_distance;
        /// <summary>
        /// Max flare distance
        /// </summary>
        private static int MAX_DISTANCE = 200;
        /// <summary>
        /// How much does the flare have to move
        /// </summary>
        internal double m_pathLength;
        /// <summary>
        /// Speed of the flare
        /// </summary>
        private static int SPEED = 5;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Item's texture</param>
        /// <param name="itemRectangle">Rectangle to draw the texture</param>
        public Flare(Texture2D texture, Rectangle itemRectangle, Point endPoint)
            : base(texture, itemRectangle)
        {
            m_shouldDispose = false;
            EndPoint = endPoint;

            if (Position.X == endPoint.X)
                m_step = new Vector2(0, SPEED);
            else if (Position.Y == endPoint.Y)
                m_step = new Vector2(SPEED, 0);
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
        /// Updates an item
        /// </summary>
        internal override void update()
        {
            Position = new Vector2(Position.X + m_step.X, Position.Y + m_step.Y);
            m_distance += Math.Sqrt(Math.Pow(m_step.X, 2) + Math.Pow(m_step.Y, 2));
            if (m_distance >= m_pathLength || m_distance > MAX_DISTANCE)
            {
                EndPoint = new Point((int)Position.X, (int)Position.Y);
                m_shouldDispose = true;
            }
        }

        /// <summary>
        /// Can item be disposed
        /// </summary>
        /// <returns>Depending on whether item can be disposed</returns>
        internal override bool shouldDispose()
        {
            return m_shouldDispose;
        }
        #endregion
    }
}
