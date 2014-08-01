using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Items
{
    /// <summary>
    /// Represents all boxes held in game
    /// </summary>
    public class Box : Item
    {
        #region Fields
        /// <summary>
        /// Type of the current box
        /// </summary>
        public BoxType Type { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Box texture</param>
        /// <param name="rectangle">Item rectangle</param>
        public Box(Texture2D texture, Rectangle rectangle, BoxType type)
            : base(texture, rectangle)
        {
            Type = type;
        }
        #endregion

        #region Methods
        internal override void update()
        {

        }

        internal override bool shouldDispose()
        {
            return false;
        }
        #endregion

        public enum BoxType
        {
            Meteo,
            Rum,
            Speed,
            Flare,
            Oil
        }
    }
}
