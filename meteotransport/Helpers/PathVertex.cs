using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Helpers
{
    /// <summary>
    /// Reprezents a vertex in a graph when looking for the shortest path using A* algorithm
    /// </summary>
    public class PathVertex
    {
        #region variables
        /// <summary>
        /// Cost of moving one edge
        /// </summary>
        private const int PATH_VALUE = 1;
        /// <summary>
        /// Total cost of the edge G + H
        /// </summary>
        public int F { get; set; }
        /// <summary>
        /// Cost of moving one edge
        /// </summary>
        public int G { get; set; }
        /// <summary>
        /// Estimated cost of the path from current point to the destination.
        /// Uses heuristics based on Manhattan distance
        /// </summary>
        public int H { get; set; }
        /// <summary>
        /// X coordinate
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Y coordinate
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Was vertex visited
        /// </summary>
        public bool Visited { get; set; }
        /// <summary>
        /// Parent vertex
        /// </summary>
        public PathVertex ParentVertex { get; set; }
        #endregion

        #region constructors
        public PathVertex(int x, int y, Point playerPosition)
        {
            X = x;
            Y = y;
            G = PATH_VALUE;
            H = Math.Abs(playerPosition.X - x) + Math.Abs(playerPosition.Y - y);
            F = G + H;
        }

        public PathVertex(Point point, Point playerPosition)
        {
            X = point.X;
            Y = point.Y;
            G = PATH_VALUE;
            H = Math.Abs(playerPosition.X - point.X) + Math.Abs(playerPosition.Y - point.Y);
            F = G + H;
        }
        #endregion

        #region methods
        /// <summary>
        /// Checks equality of coordinates
        /// </summary>
        /// <param name="vertex">Vertex to compare to</param>
        /// <returns>Returns true if coordinates match</returns>
        public override bool Equals(Object vertex)
        {
            if (vertex.GetType() == typeof(PathVertex))
            {
                PathVertex vert = vertex as PathVertex;
                return X == vert.X && Y == vert.Y;
            }
            if (vertex.GetType() == typeof(Point))
            {
                Point vert = (Point)vertex;
                return X == vert.X && Y == vert.Y;
            }
            return false;
        }

        /// <summary>
        /// Compares the total costs of two PathVertices
        /// </summary>
        /// <param name="vertex">Vertex to compare to</param>
        /// <returns>Returns the difference between the total cost of current vertex and the total cost of the vertex provided</returns>
        public int compareTo(PathVertex vertex)
        {
            return F - vertex.F;
        }
        #endregion
    }
}
