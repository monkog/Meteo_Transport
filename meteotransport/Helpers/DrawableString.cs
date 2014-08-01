using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Items
{
    class DrawableString
    {
        #region Properties
        /// <summary>
        /// Text of the string
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Position of the text
        /// </summary>
        public Vector2 Position { get; set; }
        #endregion

        #region Constructors
        public DrawableString(string text, Vector2 position)
        {
            Text = text;
            Position = position;
        }
        #endregion
    }
}
