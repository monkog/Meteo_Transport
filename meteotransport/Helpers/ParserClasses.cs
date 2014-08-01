using Meteo.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Helpers
{
    /// <summary>
    /// Helper class to parse Flare object
    /// </summary>
    public class FlareParser
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        internal float X;
        /// <summary>
        /// Y coordinate
        /// </summary>
        internal float Y;
        /// <summary>
        /// X coordinate of end point
        /// </summary>
        internal float XE;
        /// <summary>
        /// Y coordinate of end point
        /// </summary>
        internal float YE;

        public FlareParser(float x, float y, float xE, float yE)
        {
            X = x;
            Y = y;
            XE = xE;
            YE = yE;
        }
    }

    /// <summary>
    /// Helper class to parse Explosion object
    /// </summary>
    public class ExplosionParser
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        internal int X;
        /// <summary>
        /// Y coordinate
        /// </summary>
        internal int Y;
        /// <summary>
        /// Width
        /// </summary>
        internal int Width;
        /// <summary>
        /// Height
        /// </summary>
        internal int Height;
        /// <summary>
        /// Explosion level
        /// </summary>
        internal int Level;

        public ExplosionParser(int x, int y, int width, int height, int level)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Level = level;
        }
    }

    /// <summary>
    /// Helper class to parse Spit object
    /// </summary>
    public class SpitParser
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        internal float X;
        /// <summary>
        /// Y coordinate
        /// </summary>
        internal float Y;
        /// <summary>
        /// X coordinate of end point
        /// </summary>
        internal float XE;
        /// <summary>
        /// Y coordinate of end point
        /// </summary>
        internal float YE;
        /// <summary>
        /// Boxes to take away from player
        /// </summary>
        internal int Boxes;
        /// <summary>
        /// Lifes to take away from player
        /// </summary>
        internal int Lifes;

        public SpitParser(float x, float y, float xE, float yE, int boxes, int lifes)
        {
            X = x;
            Y = y;
            XE = xE;
            YE = yE;
            Boxes = boxes;
            Lifes = lifes;
        }
    }

    /// <summary>
    /// Helper class to parse predators
    /// </summary>
    public class SimpleParser
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        internal int X;
        /// <summary>
        /// Y coordinate
        /// </summary>
        internal int Y;
        /// <summary>
        /// Is predator blinded
        /// </summary>
        internal bool IsBlinded;
        /// <summary>
        /// How much time was predator blinded
        /// </summary>
        internal int BlindedTime;

        public SimpleParser(int x, int y, bool isBlinded, int blindedTime)
        {
            X = x;
            Y = y;
            IsBlinded = isBlinded;
            BlindedTime = blindedTime;
        }
    }

    /// <summary>
    /// Helper class to parse MutantCrab
    /// </summary>
    public class MutantCrabParser
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        internal int X;
        /// <summary>
        /// Y coordinate
        /// </summary>
        internal int Y;
        /// <summary>
        /// Is predator blinded
        /// </summary>
        internal bool IsBlinded;
        /// <summary>
        /// How much time was predator blinded
        /// </summary>
        internal int BlindedTime;
        /// <summary>
        /// Value of X change over a step
        /// </summary>
        internal float StepX;
        /// <summary>
        /// Value of Y change over a step
        /// </summary>
        internal float StepY;
        /// <summary>
        /// Destination X
        /// </summary>
        internal float DestX;
        /// <summary>
        /// Destination Y
        /// </summary>
        internal float DestY;
        /// <summary>
        /// Direction of X change
        /// </summary>
        internal int DirectX;
        /// <summary>
        /// Direction of Y change
        /// </summary>
        internal int DirectY;
        /// <summary>
        /// Whether MutantCrab has changed his direction
        /// </summary>
        internal bool ChangedDirection;

        public MutantCrabParser(int x, int y, bool isBlinded, int blindedTime
            , float stepX, float stepY, float destX, float destY, int directX, int directY, bool changedDirection)
        {
            X = x;
            Y = y;
            IsBlinded = isBlinded;
            BlindedTime = blindedTime;
            StepX = stepX;
            StepY = stepY;
            DestX = destX;
            DestY = destY;
            DirectX = directX;
            DirectY = directY;
            ChangedDirection = changedDirection;
        }
    }
}

