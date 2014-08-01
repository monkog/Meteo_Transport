using Meteo.GameBoard;
using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Meteo.Items
{
    public class Player : Item
    {
        #region variables
        /// <summary>
        /// Current game board
        /// </summary>
        private Board m_board;
        /// <summary>
        /// Current level
        /// </summary>
        private Level m_level;
        /// <summary>
        /// Logged user
        /// </summary>
        public User LoggedUser { get; set; }

        /// <summary>
        /// Max number of boxes to collect
        /// </summary>
        public static int MAX_BOXES = 12;
        /// <summary>
        /// Max number lifes
        /// </summary>
        public static int MAX_LIFES = 10;
        /// <summary>
        /// Max number of flares
        /// </summary>
        public static int MAX_FLARES = 10;
        /// <summary>
        /// Max level of turbo
        /// </summary>
        public static int MAX_TURBO_LEVEL = 10;
        /// <summary>
        /// Speed with turbo
        /// </summary>
        public static int TURBO_SPEED = 10;
        /// <summary>
        /// Normal speed
        /// </summary>
        public static int NORMAL_SPEED = 5;
        /// <summary>
        /// Current regular
        /// </summary>
        public static int SPEED = 5;
        /// <summary>
        /// Slow speed
        /// </summary>
        private static float SLOWSPEED = 0.5f;

        /// <summary>
        /// Left direction
        /// </summary>
        internal static Point LEFT = new Point(-1, 0);
        /// <summary>
        /// Right direction
        /// </summary>
        internal static Point RIGHT = new Point(1, 0);
        /// <summary>
        /// Up direction
        /// </summary>
        internal static Point UP = new Point(0, -1);
        /// <summary>
        /// Down direction
        /// </summary>
        internal static Point DOWN = new Point(0, 1);
        /// <summary>
        /// Points for collecting MeteoBox
        /// </summary>
        private static int METEO_POINTS = 600;
        /// <summary>
        /// Points for collecting RumBox
        /// </summary>
        private static int RUM_POINTS = 200;
        /// <summary>
        /// Points for collecting SpeedBox or FlareBox
        /// </summary>
        private static int BOX_POINTS = 100;
        /// <summary>
        /// Points for shooting pirate
        /// </summary>
        internal static int PIRATE_POINTS = 100;
        /// <summary>
        /// Points for shooting predator
        /// </summary>
        internal static int PREDATOR_POINTS = 200;
        /// <summary>
        /// Points for shooting a boss
        /// </summary>
        internal static int BOSS_POINTS = 300;
        /// <summary>
        /// Points for loosing MeteoBox
        /// </summary>
        private static int LOOSE_BOX_POINTS = -600;
        /// <summary>
        /// Points for loosing a life
        /// </summary>
        private static int LOOSE_LIFE_POINTS = -50;

        /// <summary>
        /// Current level number
        /// </summary>
        internal int CurrentLevel { get; set; }
        /// <summary>
        /// Number of lifes
        /// </summary>
        internal int Lifes { get; set; }
        /// <summary>
        /// Player's speed
        /// </summary>
        internal float Speed { get; set; }
        /// <summary>
        /// Number of collected boxes
        /// </summary>
        internal int Boxes { get; set; }
        /// <summary>
        /// Number of flares to fire
        /// </summary>
        internal int FlaresNumber { get; set; }
        /// <summary>
        /// Available turbo
        /// </summary>
        internal int TurboLevel { get; set; }
        /// <summary>
        /// Number of shot predators in current level
        /// </summary>
        internal int ShotPredators { get; set; }
        /// <summary>
        /// Number of collected points in current level
        /// </summary>
        internal int Points { get; set; }
        /// <summary>
        /// Number of shot predators in whole game
        /// </summary>
        internal int TotalShotPredators { get; set; }
        /// <summary>
        /// Number of collected points in whole game
        /// </summary>
        internal int TotalPoints { get; set; }

        /// <summary>
        /// Is player drunk
        /// </summary>
        internal bool IsDrunk { get; set; }
        /// <summary>
        /// How long is player drunk
        /// </summary>
        Stopwatch m_drunkStopwatch;
        /// <summary>
        /// How long did player swim in the same direction
        /// </summary>
        Stopwatch m_changeStopwatch;
        /// <summary>
        /// How much time has elapsed when Player was drunk
        /// </summary>
        internal int ElapsedDrunkSeconds;
        /// <summary>
        /// How long should player be drunk
        /// </summary>
        private const int DRUNK = 10;
        /// <summary>
        /// How often should player change direction when he is drunk
        /// </summary>
        private const int CHANGE_TIME = 1;

        /// <summary>
        /// Did Player reach next BoardPosition
        /// </summary>
        private bool m_finishedMoving;
        /// <summary>
        /// Distance Player moves in one step
        /// </summary>
        private Vector2 m_step;
        /// <summary>
        /// Player's destination position
        /// </summary>
        private Vector2 m_destination;
        /// <summary>
        /// Player's direction
        /// </summary>
        private Point m_direction;
        #endregion

        #region Constructors
        public Player(Texture2D texture, Rectangle itemRectangle, User user)
            : base(texture, itemRectangle)
        {
            Lifes = MAX_LIFES;
            FlaresNumber = MAX_FLARES;
            Speed = NORMAL_SPEED;
            SPEED = NORMAL_SPEED;
            m_finishedMoving = true;
            Position = Vector2.Zero;
            BoardPosition = Point.Zero;

            Points = 0;
            ShotPredators = 0;
            TotalPoints = 0;
            TotalShotPredators = 0;
            CurrentLevel = 1;
            LoggedUser = user;
            TurboLevel = 0;
            ElapsedDrunkSeconds = 0;
            m_drunkStopwatch = new Stopwatch();
            m_changeStopwatch = new Stopwatch();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Determines whether player should dispose
        /// </summary>
        /// <returns>Returns false as Player never disposes</returns>
        internal override bool shouldDispose()
        {
            return false;
        }

        /// <summary>
        /// Updates Player class
        /// </summary>
        internal override void update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (m_finishedMoving)
                updateSpeed(keyboardState);
            else
            {
                if (Position.Y + ItemSize.Height <= 0)
                    endLevel();

                checkDirection();
                Position = new Vector2(Position.X + m_step.X, Position.Y + m_step.Y);
                if ((m_direction.X > 0 && Position.X > m_destination.X) ||
                    (m_direction.X < 0 && Position.X < m_destination.X)
                    || (m_direction.Y > 0 && Position.Y > m_destination.Y) ||
                    (m_direction.Y < 0 && Position.Y < m_destination.Y))
                    Position = new Vector2(m_destination.X, m_destination.Y);
                else
                    return;
                m_finishedMoving = true;
            }
        }

        /// <summary>
        /// Changes player's direction when player is drunk
        /// </summary>
        private void changeDirection()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            int random = rand.Next(0, 2);
            if (m_direction == LEFT)
            {
                if (random == 0)
                    m_direction = UP;
                else if (random == 1)
                    m_direction = RIGHT;
                else
                    m_direction = DOWN;
            }
            else if (m_direction == RIGHT)
            {
                if (random == 0)
                    m_direction = UP;
                else if (random == 1)
                    m_direction = LEFT;
                else
                    m_direction = DOWN;
            }
            else if (m_direction == UP)
            {
                if (random == 0)
                    m_direction = RIGHT;
                else if (random == 1)
                    m_direction = LEFT;
                else
                    m_direction = DOWN;
            }
            else
            {
                if (random == 0)
                    m_direction = UP;
                else if (random == 1)
                    m_direction = LEFT;
                else
                    m_direction = RIGHT;
            }

        }

        /// <summary>
        /// Swaps values of two points
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        private void swap(ref Point a, ref Point b)
        {
            Point tmp = new Point(a.X, a.Y);
            a = new Point(b.X, b.Y);
            b = new Point(tmp.X, tmp.Y);
        }

        /// <summary>
        /// Changes player's direction when player is drunk
        /// </summary>
        private void swapDirection()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            int random = rand.Next(0, 2);
            if (m_direction == LEFT)
            {
                if (random == 0)
                    swap(ref LEFT, ref UP);
                else if (random == 1)
                    swap(ref LEFT, ref RIGHT);
                else
                    swap(ref LEFT, ref DOWN);
            }
            else if (m_direction == RIGHT)
            {
                if (random == 0)
                    swap(ref RIGHT, ref UP);
                else if (random == 1)
                    swap(ref RIGHT, ref LEFT);
                else
                    swap(ref RIGHT, ref DOWN);
            }
            else if (m_direction == UP)
            {
                if (random == 0)
                    swap(ref UP, ref RIGHT);
                else if (random == 1)
                    swap(ref UP, ref LEFT);
                else
                    swap(ref UP, ref DOWN);
            }
            else
            {
                if (random == 0)
                    swap(ref DOWN, ref UP);
                else if (random == 1)
                    swap(ref DOWN, ref LEFT);
                else
                    swap(ref DOWN, ref RIGHT);
            }

        }

        /// <summary>
        /// Restores previously changed directions
        /// </summary>
        private void restoreDirections()
        {
            LEFT = new Point(-1, 0);
            RIGHT = new Point(1, 0);
            UP = new Point(0, -1);
            DOWN = new Point(0, 1);
        }

        /// <summary>
        /// Updates the speed and direction of movement
        /// </summary>
        /// <param name="keyboardState">Keyboard state</param>
        private void updateSpeed(KeyboardState keyboardState)
        {
            Speed = SPEED;

            if (keyboardState.IsKeyDown(LoggedUser.RightKey) && BoardPosition.X < Board.WIDTH - 1)
                m_direction = RIGHT;
            else if (keyboardState.IsKeyDown(LoggedUser.LeftKey) && BoardPosition.X > 0)
                m_direction = LEFT;
            else if (keyboardState.IsKeyDown(LoggedUser.DownKey) && BoardPosition.Y < Board.HEIGHT - 1)
                m_direction = DOWN;
            else if (keyboardState.IsKeyDown(LoggedUser.UpKey))
            {
                if (!m_board.IsTopEnabled && !(BoardPosition.Y > 0))
                    return;
                m_direction = UP;
            }
            else
                return;

            if (IsDrunk && m_changeStopwatch.Elapsed.Seconds > CHANGE_TIME)
            {
                ElapsedDrunkSeconds = m_drunkStopwatch.Elapsed.Seconds;
                if (ElapsedDrunkSeconds > DRUNK)
                {
                    ElapsedDrunkSeconds = 0;
                    m_drunkStopwatch.Reset();
                    m_changeStopwatch.Reset();
                    IsDrunk = false;
                }
                else
                {
                    m_changeStopwatch.Restart();
                    do
                    {
                        changeDirection();
                    } while ((BoardPosition.X == 0 && m_direction == LEFT) || (BoardPosition.X == Board.WIDTH - 1 && m_direction == RIGHT)
                        || (BoardPosition.Y == 0 && m_direction == UP) || (BoardPosition.Y == Board.HEIGHT - 1 && m_direction == DOWN));
                }
            }

            if (keyboardState.IsKeyDown(LoggedUser.TurboKey) && TurboLevel > 0)
            {
                Speed = TURBO_SPEED;
                TurboLevel--;
            }

            BoardPosition = new Point(BoardPosition.X + m_direction.X, BoardPosition.Y + m_direction.Y);

            checkDirection();

            m_destination = new Vector2(Position.X + m_direction.X * m_board.BlockSize.Width
                , Position.Y + m_direction.Y * m_board.BlockSize.Height);
            Position = new Vector2(Position.X + m_direction.X * Speed, Position.Y + m_direction.Y * Speed);
            m_step = new Vector2(m_direction.X * Speed, m_direction.Y * Speed);
            m_finishedMoving = false;
        }

        /// <summary>
        /// Determines the speed of the ship depending on the direction
        /// </summary>
        private void checkDirection()
        {
            IceBlock iceBlock = m_board.getIceBlock(BoardPosition.X, BoardPosition.Y);
            if (iceBlock == null)
                return;
            if (iceBlock.FrozenLevel == IceBlock.MAX_FROZEN_LEVEL)
                Speed *= SLOWSPEED;
            iceBlock.unfreeze();
        }

        /// <summary>
        /// Sets the current level and board
        /// </summary>
        /// <param name="level">Current level</param>
        public void setLevel(Level level)
        {
            m_level = level;
            m_board = level.GameBoard;
            m_finishedMoving = true;
            Points = 0;
            Boxes = 0;
        }

        /// <summary>
        /// Increment number of collected MeteoBoxes
        /// </summary>
        private void collectMeteoBox()
        {
            Points += METEO_POINTS;
            Boxes++;
            if (Boxes == MAX_BOXES)
                m_board.IsTopEnabled = true;
        }

        /// <summary>
        /// Makes player unable of moving the ship
        /// </summary>
        public void drink()
        {
            Points += RUM_POINTS;
            IsDrunk = true;
            m_drunkStopwatch.Restart();
            m_changeStopwatch.Restart();
        }

        /// <summary>
        /// Sets player's number of flares to MAX_FLARES
        /// </summary>
        public void collectFlares()
        {
            Points += BOX_POINTS;
            FlaresNumber = MAX_FLARES;
        }

        /// <summary>
        /// Reduces Player's number of flares
        /// </summary>
        public void reduceFlares()
        {
            FlaresNumber--;
        }

        /// <summary>
        /// Sets player's number of lifes to MAX_LIFES
        /// </summary>
        public void recoverLifes()
        {
            Points += BOX_POINTS;
            Lifes = MAX_LIFES;
        }

        /// <summary>
        /// Sets the turbo counter to MAX_TURBO_LEVEL
        /// </summary>
        public void collectTurbo()
        {
            Points += BOX_POINTS;
            TurboLevel = MAX_TURBO_LEVEL;
        }

        /// <summary>
        /// Reduces players lifes
        /// </summary>
        /// <param name="lifes">Number of lifes to reduce</param>
        public void reduceLifes(int lifes)
        {
            Points += (LOOSE_LIFE_POINTS * lifes);
            if (Points < 0)
                Points = 0;
            Lifes -= lifes;
            if (Lifes <= 0)
                restartLevel();
        }

        /// <summary>
        /// Collects the found box 
        /// </summary>
        /// <remarks>
        /// If the Box is a type of MeteorBox it increases the number of collected boxes.
        /// Otherwise it calls a function connected to the given type
        /// </remarks>
        /// <param name="box">Box</param>
        public void collectBox(Box box)
        {
            if (box.Type == Box.BoxType.Meteo)
                collectMeteoBox();
            else if (box.Type == Box.BoxType.Oil)
                recoverLifes();
            else if (box.Type == Box.BoxType.Flare)
                collectFlares();
            else if (box.Type == Box.BoxType.Rum)
                drink();
            else if (box.Type == Box.BoxType.Speed)
                collectTurbo();
        }

        /// <summary>
        /// Reduces player's number of boxes 
        /// </summary>
        /// <param name="boxes">Number of boxes to reduce</param>
        public void reduceBoxes(int boxes)
        {
            Points += LOOSE_BOX_POINTS;
            if (Points < 0)
                Points = 0;
            Boxes -= boxes;
            m_board.IsTopEnabled = false;
        }

        /// <summary>
        /// Reduces the spped of Player
        /// </summary>
        public void reduceSpeed()
        {
            SPEED = (int)(NORMAL_SPEED * SLOWSPEED);
        }

        /// <summary>
        /// Sets Speed value to its normal value
        /// </summary>
        public void getNormalSpeed()
        {
            SPEED = NORMAL_SPEED;
        }

        /// <summary>
        /// Ends the level and sums up the statistics
        /// </summary>
        private void endLevel()
        {
            TotalPoints += Points;
            TotalShotPredators += ShotPredators;
            Boxes = 0;
            Lifes = MAX_LIFES;
            FlaresNumber = MAX_FLARES;
            m_finishedMoving = true;
            IsDrunk = false;
            m_drunkStopwatch.Reset();
            m_changeStopwatch.Reset();
            m_level.endLevel();
        }

        /// <summary>
        /// Restarts level
        /// </summary>
        private void restartLevel()
        {
            Lifes = MAX_LIFES;
            FlaresNumber = MAX_FLARES;
            TurboLevel = 0;
            Boxes = 0;
            IsDrunk = false;
            m_level.restartLevel();
            m_drunkStopwatch.Reset();
            m_changeStopwatch.Reset();
        }

        /// <summary>
        /// Sets the logged user
        /// </summary>
        /// <param name="user">Logged user</param>
        public void setUser(User user)
        {
            LoggedUser = user;
        }
        #endregion
    }
}
