using Meteo.GameBoard;
using Meteo.Helpers;
using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Meteo.Items.Predators
{
    public abstract class Predator : Item
    {
        #region variables
        /// <summary>
        /// Length of predator being blind
        /// </summary>
        protected int BLIND = 5000;
        /// <summary>
        /// Time span between attacks
        /// </summary>
        protected const int ATTACK_SECONDS = 3;

        /// <summary>
        /// Player instance
        /// </summary>
        protected Player m_player;
        /// <summary>
        /// Current board
        /// </summary>
        protected Board m_board;
        /// <summary>
        /// Curremt level
        /// </summary>
        protected Level m_level;
        /// <summary>
        /// Content Manager
        /// </summary>
        internal ContentManager Content { get; set; }
        /// <summary>
        /// Stars when predator is blinded
        /// </summary>
        protected Stars m_stars;

        /// <summary>
        /// Measures time when predator is blinded
        /// </summary>
        internal Stopwatch m_blindTimer;
        /// <summary>
        /// How much time has predator been blinded
        /// </summary>
        internal int BlindedSeconds;
        /// <summary>
        /// Speed of the predator
        /// </summary>
        protected float m_speed;
        /// <summary>
        /// Wheter predator is blinded
        /// </summary>
        /// <remarks>When predator is blinded, no movement or attack is performed</remarks>
        internal bool IsBlinded;
        /// <summary>
        /// Distance from Player, where Predator starts to attack;
        /// </summary>
        public int MaxDistance { get; protected set; }
        /// <summary>
        /// Keeps the track of the time elapsed from last attack
        /// </summary>
        protected int m_timeElapsed;
        /// <summary>
        /// Controls the time span between two attacks
        /// </summary>
        protected Stopwatch m_attackTimer;
        /// <summary>
        /// Determines whether Predator should attack again
        /// </summary>
        protected bool m_update;

        /// <summary>
        /// Array representation of path graph
        /// </summary>
        private PathVertex[,] m_vertexBoard;
        /// <summary>
        /// Found shortest path to the Player
        /// </summary>
        public List<Point> PredatorPath { get; private set; }
        #endregion

        #region constructors
        public Predator(Texture2D texture, Rectangle itemRectangle, Level level, Player player)
            : base(texture, itemRectangle)
        {
            m_level = level;
            m_player = player;
            m_board = level.GameBoard;
            Content = level.Content;
            BlindedSeconds = 0;
            m_blindTimer = new Stopwatch();
            m_attackTimer = new Stopwatch();
            PredatorPath = new List<Point>();
            if (GetType() != typeof(Spit))
                m_board.Items[BoardPosition.X, BoardPosition.Y].Add(this);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Abstract function representing attacking player
        /// </summary>
        public abstract void attack();

        /// <summary>
        /// Randomly leaves bonus on the board
        /// </summary>
        internal abstract void leaveBonus();

        /// <summary>
        /// Updates Stars class
        /// </summary>
        internal override void update()
        {
            if (m_stars != null)
                m_stars.update();
        }

        /// <summary>
        /// Draws Stars
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);
            if (m_stars != null)
                m_stars.draw(spriteBatch);
        }

        /// <summary>
        /// Reduces player's lifes
        /// </summary>
        /// <param name="lifes">Number of lifes to reduce</param>
        protected void reduceLifes(int lifes)
        {
            m_player.reduceLifes(lifes);
        }

        /// <summary>
        /// Blinds the predator. Prevents the predator from moving and attacking
        /// </summary>
        /// <param name="realPath">Flare real path</param>
        /// <param name="maxPath">Max flare path</param>
        internal virtual void blind(double realPath, double maxPath)
        {
            BLIND = 5000;
            m_shouldUpdate = false;
            IsBlinded = true;
            BlindedSeconds = 0;
            m_blindTimer.Restart();

            if (maxPath > 2 * realPath)
                BLIND = 3000;

            Random rand = new Random(DateTime.Now.Millisecond);
            int bonus = rand.Next(0, 5);
            if (bonus == 0)
                leaveBonus();

            m_stars = new Stars(Content.Load<Texture2D>("Items/Stars"), new Rectangle((int)Position.X, (int)Position.Y, ItemSize.Width, ItemSize.Height));
        }

        /// <summary>
        /// Make predator blinded
        /// </summary>
        /// <remarks>
        /// Used when loading a game to set all options
        /// </remarks>
        internal void setBlind()
        {
            if (IsBlinded == false)
                return;
            m_blindTimer.Start();
            m_shouldUpdate = false;
            m_stars = new Stars(Content.Load<Texture2D>("Items/Stars"), new Rectangle((int)Position.X, (int)Position.Y, ItemSize.Width, ItemSize.Height));
        }

        /// <summary>
        /// Blinds the predator when Player's speed is Turbo speed
        /// </summary>
        internal virtual void collidePredator()
        {
            if (IsBlinded == true)
                return;
            IsBlinded = true;
            blind(0, 0);
        }

        /// <summary>
        /// Reduces player's number of MeteorBox
        /// </summary>
        /// <param name="boxes">Number of boxes to reduce</param>
        protected void reduceBoxes(int boxes)
        {
            int reducedBoxes = Math.Min(boxes, m_player.Boxes);

            m_player.reduceBoxes(reducedBoxes);
            Texture2D texture = Content.Load<Texture2D>("Items/MeteoBox");

            for (int i = 0; i < reducedBoxes; i++)
            {
                int x = 0, y = 0;
                int itemWidth = m_board.BlockSize.Width;
                int itemHeight = m_board.BlockSize.Height;

                m_board.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_level.m_predators, itemWidth, itemHeight);

                Box box = new Box(texture, new Rectangle(x, y, itemWidth, itemHeight), Box.BoxType.Meteo);
                m_board.addItem(x, y, box);
            }
        }

        /// <summary>
        /// Leaves a FlareBox or a Speed box on the board
        /// </summary>
        protected void leavePirateBox()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int x = 0, y = 0;
            int itemWidth = m_board.BlockSize.Width;
            int itemHeight = m_board.BlockSize.Height;

            m_board.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_level.m_predators, itemWidth, itemHeight);

            Box box;
            if (random.Next() % 2 == 0)
            {
                Texture2D texture = Content.Load<Texture2D>("Items/FlareBox");
                box = new Box(texture, new Rectangle(x, y, itemWidth, itemHeight), Box.BoxType.Flare);
            }
            else
            {
                Texture2D texture = Content.Load<Texture2D>("Items/SpeedBox");
                box = new Box(texture, new Rectangle(x, y, itemWidth, itemHeight), Box.BoxType.Speed);
            }
            m_board.addItem(x, y, box);
        }

        /// <summary>
        /// Leaves an OilBox on the board
        /// </summary>
        protected void leaveOilBox()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int x = 0, y = 0;
            int itemWidth = m_board.BlockSize.Width;
            int itemHeight = m_board.BlockSize.Height;

            m_board.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_level.m_predators, itemWidth, itemHeight);

            Box box = new Box(Content.Load<Texture2D>("Items/OilBox"), new Rectangle(x, y, itemWidth, itemHeight), Box.BoxType.Oil);
            m_board.addItem(x, y, box);
        }

        /// <summary>
        /// Uses A star algorithm to determine the shortest path from predator to player
        /// </summary>
        /// <param name="fullPath">Whether graph should be full or not</param>
        public void createPath(bool fullPath)
        {
            if (fullPath)
                createFullPath();
            else
                createVerticesLists();

            PredatorPath = makePath(aStar(m_vertexBoard[BoardPosition.X, BoardPosition.Y], m_vertexBoard[m_player.BoardPosition.X, m_player.BoardPosition.Y]));
        }

        /// <summary>
        /// A star algorithm
        /// </summary>
        /// <param name="start">Start vertex</param>
        /// <param name="end">End vertex</param>
        /// <returns>List of path vertices in reverse</returns>
        private LinkedList<PathVertex> aStar(PathVertex start, PathVertex end)
        {
            List<PathVertex> closed = new List<PathVertex>();
            List<PathVertex> open = new List<PathVertex>();
            if (start == null)
            {
                start = new PathVertex(BoardPosition, m_player.BoardPosition);
                m_vertexBoard[BoardPosition.X, BoardPosition.Y] = start;
            }
            open.Add(start);
            start.G = 0;

            while (open.Count > 0)
            {
                open.Sort((a, b) => a.F > b.F ? 1 : (a.F < b.F ? -1 : 0));
                PathVertex x = open[0];

                if (x == end)
                    return reconstructPath(x.ParentVertex, end);
                open.Remove(x);
                closed.Add(x);

                LinkedList<PathVertex> neighbors = findNeighbors(x);
                foreach (PathVertex y in neighbors)
                {
                    if (closed.Contains(y))
                        continue;
                    int g = x.G + x.G;
                    bool isBetter = false;

                    if (!open.Contains(y))
                    {
                        open.Add(y);
                        isBetter = true;
                    }
                    else if (g < y.G)
                        isBetter = true;

                    if (isBetter == true)
                    {
                        y.ParentVertex = x;
                        y.G = g;
                        y.F = y.G + y.H;
                    }
                }
            }
            return new LinkedList<PathVertex>();
        }

        /// <summary>
        /// Returns list of all neighbor Vertices that are available to move into
        /// </summary>
        /// <param name="x">Vertex, we search neighbors for</param>
        /// <returns>Returns list of neighbors</returns>
        private LinkedList<PathVertex> findNeighbors(PathVertex x)
        {
            LinkedList<PathVertex> neighbors = new LinkedList<PathVertex>();

            if (x.X > 0 && m_vertexBoard[x.X - 1, x.Y] != null)
                neighbors.AddFirst(m_vertexBoard[x.X - 1, x.Y]);
            if (x.X < Board.WIDTH - 1 && m_vertexBoard[x.X + 1, x.Y] != null)
                neighbors.AddFirst(m_vertexBoard[x.X + 1, x.Y]);
            if (x.Y > 0 && m_vertexBoard[x.X, x.Y - 1] != null)
                neighbors.AddFirst(m_vertexBoard[x.X, x.Y - 1]);
            if (x.Y < Board.HEIGHT - 1 && m_vertexBoard[x.X, x.Y + 1] != null)
                neighbors.AddFirst(m_vertexBoard[x.X, x.Y + 1]);

            return neighbors;
        }

        /// <summary>
        /// Reconstructs the shortest path
        /// </summary>
        /// <param name="x">Vertex the Predator is on</param>
        /// <param name="y">Target vertex</param>
        /// <returns></returns>
        private LinkedList<PathVertex> reconstructPath(PathVertex x, PathVertex y)
        {
            PathVertex current = y;
            LinkedList<PathVertex> path = new LinkedList<PathVertex>();
            path.AddFirst(y);

            while (current.ParentVertex != null)
            {
                path.AddFirst(current.ParentVertex);
                current = current.ParentVertex;
            }
            return path;
        }

        /// <summary>
        /// Makes path available for Predators to parse
        /// </summary>
        /// <param name="list">PathVertex list that we get coordinates from</param>
        /// <returns>Returns list with Points that are the found path</returns>
        private List<Point> makePath(LinkedList<PathVertex> list)
        {
            List<Point> pathList = new List<Point>();
            LinkedListNode<PathVertex> node = list.First;
            int index = 0;

            while (node != null && index < 10)
            {
                pathList.Add(new Point(node.Value.X, node.Value.Y));
                node = node.Next;
                index++;
            }
            return pathList;
        }

        /// <summary>
        /// Creates graphs necessary for A star algorithm
        /// </summary>
        private void createVerticesLists()
        {
            m_vertexBoard = new PathVertex[Board.WIDTH, Board.HEIGHT];

            for (int i = 0; i < Board.WIDTH; i++)
                for (int j = 0; j < Board.HEIGHT; j++)
                    if (!m_board.isFrozen(i, j))
                        m_vertexBoard[i, j] = new PathVertex(i, j, m_player.BoardPosition);
        }

        /// <summary>
        /// Creates a full graph
        /// </summary>
        protected void createFullPath()
        {
            m_vertexBoard = new PathVertex[Board.WIDTH, Board.HEIGHT];

            for (int i = 0; i < Board.WIDTH; i++)
                for (int j = 0; j < Board.HEIGHT; j++)
                    m_vertexBoard[i, j] = new PathVertex(i, j, m_player.BoardPosition);
        }
        #endregion
    }
}
