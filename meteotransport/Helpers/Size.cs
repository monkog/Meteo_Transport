namespace Meteo.Items
{
    /// <summary>
    /// Reprezents a size of an object
    /// </summary>
    public class Size
    {
        #region variables
        /// <summary>
        /// Width
        /// </summary>
        public int Width;
        /// <summary>
        /// Height
        /// </summary>
        public int Height;
        #endregion

        #region constructors
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
        #endregion
    }
}
