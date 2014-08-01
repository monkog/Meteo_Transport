using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Meteo.Items;
using Microsoft.Xna.Framework.Graphics;
using Meteo.Helpers;
using Meteo.Items.Predators;

namespace Meteo.GameBoard
{
    /// <summary>
    /// Handles elements on board
    /// </summary>
    public class Board
    {
        #region variables
        /// <summary>
        /// Array of IceBlocks
        /// </summary>
        public IceBlock[,] IceBlocks { get; private set; }
        /// <summary>
        /// List of items on current BoardPosition
        /// </summary>
        internal List<Item>[,] Items;
        /// <summary>
        /// Width and Height of Board in px
        /// </summary>
        public Size Size { get; private set; }
        /// <summary>
        /// Width and Height of single block
        /// </summary>
        public Size BlockSize { get; private set; }

        /// <summary>
        /// Can current game be finished
        /// </summary>
        public bool IsTopEnabled { get; set; }
        /// <summary>
        /// Number of horizontal blocks
        /// </summary>
        public static int WIDTH = 18;
        /// <summary>
        /// Number of vertical blocks
        /// </summary>
        public static int HEIGHT = 11;
        #endregion

        #region constructor
        public Board(Size boardSize)
        {
            IceBlocks = new IceBlock[WIDTH, HEIGHT];
            Items = new List<Item>[WIDTH, HEIGHT];
            Size = boardSize;
            BlockSize = new Size(boardSize.Width / WIDTH, boardSize.Height / HEIGHT);
            IsTopEnabled = false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads the content of current level
        /// </summary>
        public virtual void loadContent(ContentManager contentManager, Point playerPosition)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            Texture2D texture = contentManager.Load<Texture2D>("Items/Ice");

            for (int i = 0; i < WIDTH; i++)
                for (int j = 0; j < HEIGHT; j++)
                {
                    Items[i, j] = new List<Item>();

                    if (random.Next(10, 50) < 25)
                        IceBlocks[i, j] = new IceBlock(texture, new Rectangle(i, j, Size.Width / WIDTH, Size.Height / HEIGHT), (random.Next(0, 1000) % 5 + 1));
                }
        }

        /// <summary>
        /// Updates the board
        /// </summary>
        internal void update()
        {
            for (int i = 0; i < Board.WIDTH; i++)
                for (int j = 0; j < Board.HEIGHT; j++)
                    if (IceBlocks[i, j] != null)
                        IceBlocks[i, j].update();
        }

        /// <summary>
        /// Draws items on board
        /// </summary>
        /// <param name="spriteBatch"></param>
        internal void draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Board.WIDTH; i++)
                for (int j = 0; j < Board.HEIGHT; j++)
                    if (IceBlocks[i, j] != null)
                        IceBlocks[i, j].draw(spriteBatch);

            for (int i = 0; i < Board.WIDTH; i++)
                for (int j = 0; j < Board.HEIGHT; j++)
                    foreach (Item item in Items[i, j])
                        item.draw(spriteBatch);
        }

        /// <summary>
        /// Initializes the board.
        /// </summary>
        /// <remarks>Adds MeteorBoxes and RumBoxes randomly to the board. The number of RumBoxes varies from 2 - 7</remarks>
        internal void initializeBoxes(ContentManager contentManager, Point playerPosition)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int x = 0, y = 0;
            int itemWidth = BlockSize.Width;
            int itemHeight = BlockSize.Height;

            Texture2D texture = contentManager.Load<Texture2D>("Items/MeteoBox");
            for (int i = 0; i < Player.MAX_BOXES; i++)
            {
                generateBossCoordinates(ref x, ref y, playerPosition, new List<Predator>(), itemWidth, itemHeight);

                Box box = new Box(texture, new Rectangle(x, y, itemWidth, itemHeight), Box.BoxType.Meteo);
                addItem(x, y, box);
            }

            int rumBoxes = random.Next(0, 5) + 2;
            texture = contentManager.Load<Texture2D>("Items/RumBox");
            for (int i = 0; i < rumBoxes; i++)
            {
                generateBossCoordinates(ref x, ref y, playerPosition, new List<Predator>(), itemWidth, itemHeight);

                Box box = new Box(texture, new Rectangle(x, y, itemWidth, itemHeight), Box.BoxType.Rum);
                addItem(x, y, box);
            }
        }

        /// <summary>
        /// Returns the IceBlock on given coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns></returns>
        public IceBlock getIceBlock(int x, int y)
        {
            if (x < 0 || x >= IceBlocks.GetLength(0) || y < 0 || y >= IceBlocks.GetLength(1))
                return null;
            return IceBlocks[x, y];
        }

        /// <summary>
        /// Returns all items except Player that currently are on the given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public List<Item> isFree(int x, int y)
        {
            return Items[x, y];
        }

        /// <summary>
        /// Determines whether the following IceBlock is frozen
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Returns true if the frozen level of the IceBlock on given coordinates is MAX_FROZEN_LEVEL, otherwise false</returns>
        public bool isFrozen(int x, int y)
        {
            return IceBlocks[x, y] != null && IceBlocks[x, y].FrozenLevel == IceBlock.MAX_FROZEN_LEVEL;
        }

        /// <summary>
        /// Ads an Item to the given coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="item">Item to leave</param>
        public void addItem(int x, int y, Item item)
        {
            Items[x, y].Add(item);
        }

        /// <summary>
        /// Generates coordinates for predators
        /// </summary>
        /// <remarks>Generates coordinates so that two items won't start being on the same BoardPosition</remarks>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        internal void generateCoordinates(ref int x, ref int y, Point playerPosition)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            x = random.Next(0, WIDTH - 1);
            y = random.Next(0, HEIGHT - 1);
            bool collides = true;

            while (isFree(x, y).Count > 0 || isFrozen(x, y)
                || (playerPosition.X == x && playerPosition.Y == y))
            {
                x = random.Next(0, WIDTH - 1);
                y = random.Next(0, HEIGHT - 1);
            }
        }

        /// <summary>
        /// Generates coordinates for predators
        /// </summary>
        /// <remarks>Generates coordinates so that two items won't start being on the same BoardPosition</remarks>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="predators">List of predators</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        internal void generateBossCoordinates(ref int x, ref int y, Point playerPosition, List<Predator> predators, int width, int height)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            x = random.Next(0, WIDTH - 1);
            y = random.Next(0, HEIGHT - 1);
            bool collides = true;

            while (collides)
            {
                x = random.Next(0, WIDTH - 1);
                y = random.Next(0, HEIGHT - 1);
                while (collides && (isFree(x, y).Count > 0 || isFrozen(x, y)
                    || (playerPosition.X == x && playerPosition.Y == y)))
                {
                    x = random.Next(0, WIDTH - 1);
                    y = random.Next(0, HEIGHT - 1);
                }
                collides = false;
                Rectangle rect = new Rectangle(x * BlockSize.Width, y * BlockSize.Height, width, height);
                foreach (Predator predator in predators)
                    if (rect.Intersects(new Rectangle((int)predator.Position.X, (int)predator.Position.Y, predator.ItemSize.Width, predator.ItemSize.Height)))
                        collides = true;
            }
        }

        /// <summary>
        /// Enables loading the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="iceblocks">List of IceBlock locations and parameters</param>
        public void setBoard(ContentManager content, List<KeyValuePair<Point, int>> iceblocks)
        {
            IceBlocks = new IceBlock[WIDTH, HEIGHT];
            Texture2D texture = content.Load<Texture2D>("Items/Ice");

            foreach (KeyValuePair<Point, int> ice in iceblocks)
                IceBlocks[ice.Key.X, ice.Key.Y] = new IceBlock(texture
                    , new Rectangle(ice.Key.X, ice.Key.Y, Size.Width / WIDTH, Size.Height / HEIGHT), ice.Value);

            Items = new List<Item>[WIDTH, HEIGHT];

            for (int i = 0; i < WIDTH; i++)
                for (int j = 0; j < HEIGHT; j++)
                    Items[i, j] = new List<Item>();
        }

        /// <summary>
        /// Enables loading boxes on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="boxes">List of Box' locations and types</param>
        public void setBoxes(ContentManager content, List<KeyValuePair<Point, string>> boxes)
        {
            foreach (KeyValuePair<Point, string> box in boxes)
                switch (box.Value)
                {
                    case "Meteo":
                        addItem(box.Key.X, box.Key.Y, new Box(content.Load<Texture2D>("Items/MeteoBox")
                    , new Rectangle(box.Key.X, box.Key.Y, Size.Width / WIDTH, Size.Height / HEIGHT), Box.BoxType.Meteo));
                        break;
                    case "Speed":
                        addItem(box.Key.X, box.Key.Y, new Box(content.Load<Texture2D>("Items/SpeedBox")
                    , new Rectangle(box.Key.X, box.Key.Y, Size.Width / WIDTH, Size.Height / HEIGHT), Box.BoxType.Speed));
                        break;
                    case "Flare":
                        addItem(box.Key.X, box.Key.Y, new Box(content.Load<Texture2D>("Items/FlareBox")
                    , new Rectangle(box.Key.X, box.Key.Y, Size.Width / WIDTH, Size.Height / HEIGHT), Box.BoxType.Flare));
                        break;
                    case "Rum":
                        addItem(box.Key.X, box.Key.Y, new Box(content.Load<Texture2D>("Items/RumBox")
                    , new Rectangle(box.Key.X, box.Key.Y, Size.Width / WIDTH, Size.Height / HEIGHT), Box.BoxType.Rum));
                        break;
                    case "Oil":
                        addItem(box.Key.X, box.Key.Y, new Box(content.Load<Texture2D>("Items/OilBox")
                    , new Rectangle(box.Key.X, box.Key.Y, Size.Width / WIDTH, Size.Height / HEIGHT), Box.BoxType.Oil));
                        break;
                }
        }
        #endregion
    }
}
