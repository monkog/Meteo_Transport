using Meteo.GameBoard;
using Meteo.Helpers;
using Meteo.Items;
using Meteo.Items.Predators;
using Meteo.Items.Predators.Animals;
using Meteo.Items.Predators.Pirates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo.Levels
{
    public class Level
    {
        #region variables
        /// <summary>
        /// Current game board
        /// </summary>
        public Board GameBoard { get; private set; }
        /// <summary>
        /// Shows game statistics
        /// </summary>
        public GameStatus Status { get; private set; }
        /// <summary>
        /// Player
        /// </summary>
        protected Player m_player;
        /// <summary>
        /// Previous mouse state
        /// </summary>
        private MouseState m_lastMouseState;
        /// <summary>
        /// Content manager for loading predators
        /// </summary>
        internal ContentManager Content { get; set; }

        /// <summary>
        /// Number of shot predators in current level
        /// </summary>
        public int ShotPredators { get; private set; }
        /// <summary>
        /// Number of points gained in current level
        /// </summary>
        public int Points { get; private set; }
        /// <summary>
        /// List of predators in current level
        /// </summary>
        internal List<Predator> m_predators;
        /// <summary>
        /// List of octopus' spit in current level
        /// </summary>
        internal List<Spit> m_spits;
        /// <summary>
        /// List of flares in current level
        /// </summary>
        internal List<Flare> m_flares;
        /// <summary>
        /// List of explosions in current level
        /// </summary>
        internal List<Explosion> m_explosions;
        /// <summary>
        /// Id of current level
        /// </summary>
        internal LevelNumber LevelId { get; set; }
        /// <summary>
        /// Game difficulty
        /// </summary>
        internal int Difficulty { get; set; }

        /// <summary>
        /// Max Shark number in game
        /// </summary>
        private int MAX_SHARK = 1;
        /// <summary>
        /// Determines how many sharks are on the board
        /// </summary>
        protected int m_sharkNumber;
        /// <summary>
        /// Max Octopus number in game
        /// </summary>
        private int MAX_OCTOPUS = 1;
        /// <summary>
        /// Number of Octopus in game
        /// </summary>
        internal int OctopusNumber { get; set; }

        /// <summary>
        /// Explosion sound effect
        /// </summary>
        SoundEffect m_explosionSound;
        /// <summary>
        /// Are sounds on
        /// </summary>
        internal bool SoundsOn { get; set; }
        #endregion

        #region Constructors
        public Level(Player player, int width, int height, int difficulty)
        {
            Difficulty = difficulty;
            MAX_SHARK = difficulty;
            MAX_OCTOPUS = difficulty;
            m_sharkNumber = 0;

            m_predators = new List<Predator>();
            m_flares = new List<Flare>();
            m_explosions = new List<Explosion>();
            m_spits = new List<Spit>();

            GameBoard = new Board(new Size((int)(3 * width / 4), height));

            m_player = player;
            m_player.BoardPosition = new Point(Board.WIDTH / 2, Board.HEIGHT / 2);
            m_player.Position = new Vector2(m_player.BoardPosition.X * GameBoard.BlockSize.Width, m_player.BoardPosition.Y * GameBoard.BlockSize.Height);

            Status = new GameStatus((int)(width / 4), height, m_player);

            m_lastMouseState = Mouse.GetState();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Loads the content of current level
        /// </summary>
        public virtual void loadContent(ContentManager content)
        {
            Content = content;
            GameBoard.loadContent(content, m_player.BoardPosition);
            Status.loadContent(content);
            m_explosionSound = content.Load<SoundEffect>("Sounds/Explosion");
        }

        /// <summary>
        /// Updates the level items
        /// </summary>
        public virtual void update()
        {
            GameBoard.update();
            m_player.update();

            if (m_player.BoardPosition.Y < 0)
            {
                return;
            }

            List<Predator> predatorsToRemove = new List<Predator>();
            foreach (Predator predator in m_predators)
            {
                predator.update();

                if (predator.collides(m_player, 0, 0) && m_player.Speed == Player.TURBO_SPEED)
                    predator.collidePredator();
                else if (predator.GetType() == typeof(MutantOctopus))
                    predator.attack();
                else if (predator.collides(m_player, predator.MaxDistance, predator.MaxDistance))
                    predator.attack();

                if (predator.shouldDispose())
                    predatorsToRemove.Add(predator);
            }
            foreach (Predator predator in predatorsToRemove)
            {
                m_predators.Remove(predator);
                GameBoard.Items[predator.BoardPosition.X, predator.BoardPosition.Y].Remove(predator);
            }

            List<Box> boxesToRemove = new List<Box>();
            List<Item> items = GameBoard.isFree(m_player.BoardPosition.X, m_player.BoardPosition.Y);
            if (items.Count > 0)
                foreach (Item item in items)
                    if (item.GetType() == typeof(Box))
                    {
                        m_player.collectBox((Box)item);
                        boxesToRemove.Add((Box)item);
                    }
            foreach (Box box in boxesToRemove)
                GameBoard.Items[box.BoardPosition.X, box.BoardPosition.Y].Remove(box);

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && m_lastMouseState.LeftButton != ButtonState.Pressed
                && m_player.FlaresNumber > 0)
            {
                Flare flare = new Flare(Content.Load<Texture2D>("Items/Flare")
                    , new Rectangle(m_player.BoardPosition.X, m_player.BoardPosition.Y, GameBoard.BlockSize.Width, GameBoard.BlockSize.Height)
                    , new Point(mouseState.X - GameBoard.BlockSize.Width / 2, mouseState.Y - GameBoard.BlockSize.Height / 2));
                m_flares.Add(flare);
                m_player.reduceFlares();
            }
            m_lastMouseState = mouseState;

            List<Flare> flaresToRemove = new List<Flare>();
            foreach (Flare flare in m_flares)
            {
                flare.update();
                if (flare.shouldDispose())
                {
                    flaresToRemove.Add(flare);
                    if (SoundsOn)
                        m_explosionSound.Play();
                    Explosion explosion = new Explosion(Content.Load<Texture2D>("Items/Explosion")
                        , new Rectangle(flare.EndPoint.X, flare.EndPoint.Y
                        , GameBoard.BlockSize.Width, GameBoard.BlockSize.Height));
                    m_explosions.Add(explosion);
                }

                foreach (Predator predator in m_predators)
                    if (flare.collides(predator, 0, 0))
                    {
                        if (predator.GetType() == typeof(GreenPirate) || predator.GetType() == typeof(RedPirate) || predator.GetType() == typeof(BlackPirate))
                            m_player.Points += Player.PIRATE_POINTS;
                        else
                            m_player.Points += Player.PREDATOR_POINTS;

                        m_player.ShotPredators++;
                        predator.blind(flare.m_distance, flare.m_pathLength);
                        flaresToRemove.Add(flare);
                        if (SoundsOn)
                            m_explosionSound.Play();
                        Explosion explosion = new Explosion(Content.Load<Texture2D>("Items/Explosion")
                            , new Rectangle((int)predator.Position.X, (int)predator.Position.Y
                            , predator.ItemSize.Width, predator.ItemSize.Height));
                        m_explosions.Add(explosion);
                    }
            }

            foreach (Flare flare in flaresToRemove)
                m_flares.Remove((Flare)flare);

            List<Explosion> itemsToRemove = new List<Explosion>();
            foreach (Explosion explosion in m_explosions)
            {
                explosion.update();
                if (explosion.shouldDispose())
                    itemsToRemove.Add(explosion);
            }

            foreach (Explosion explosion in itemsToRemove)
                m_explosions.Remove(explosion);

            List<Spit> spitToRemove = new List<Spit>();
            foreach (Spit spit in m_spits)
            {
                spit.update();
                if (spit.collides(m_player, 0, 0))
                {
                    if (spit.m_boxes == 1 && SoundsOn)
                        m_explosionSound.Play();
                    spit.attack();
                    Explosion explosion = new Explosion(Content.Load<Texture2D>("Items/Explosion")
                        , new Rectangle((int)m_player.Position.X, (int)m_player.Position.Y
                        , m_player.ItemSize.Width, m_player.ItemSize.Height));
                    m_explosions.Add(explosion);
                }

                if (spit.shouldDispose())
                    spitToRemove.Add(spit);
            }

            foreach (Spit spit in spitToRemove)
                m_spits.Remove(spit);

            Status.update();
        }

        /// <summary>
        /// Generates Shark
        /// </summary>
        protected void generateShark()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            if (random.Next(0, 1000) > 10 || m_sharkNumber == MAX_SHARK)
                return;

            m_sharkNumber++;
            int x = 0, y = 0;
            int itemWidth = GameBoard.BlockSize.Width;
            int itemHeight = GameBoard.BlockSize.Height;
            Texture2D texture = Content.Load<Texture2D>("Items/Shark");

            do
            {
                GameBoard.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_predators, itemWidth, itemHeight);
            } while (!(Math.Abs(x - m_player.BoardPosition.X) > 4 && Math.Abs(y - m_player.BoardPosition.Y) > 4));
            Shark shark = new Shark(texture, new Rectangle(x, y, itemWidth, itemHeight), this, m_player);
            m_predators.Add(shark);
        }

        /// <summary>
        /// Generates Octopus
        /// </summary>
        protected void generateOctopus()
        {
            int x = 0, y = 0;
            int itemWidth = GameBoard.BlockSize.Width * 2;
            int itemHeight = GameBoard.BlockSize.Height * 2;

            Random random = new Random(DateTime.Now.Millisecond);
            if (random.Next(0, 1000) < MAX_OCTOPUS && OctopusNumber < MAX_OCTOPUS)
            {
                OctopusNumber++;
                if (random.Next(0, 9) == 0)
                {
                    x = m_player.BoardPosition.X;
                    y = m_player.BoardPosition.Y;
                }
                else
                    do
                    {
                        GameBoard.generateBossCoordinates(ref x, ref y, m_player.BoardPosition, m_predators, itemWidth, itemHeight);
                    } while (!(x < Board.WIDTH - 2 && y < Board.HEIGHT - 2));

                Predator predator = new Octopus(Content.Load<Texture2D>("Items/Octopus")
                    , new Rectangle(x, y, itemWidth, itemHeight), this, m_player);
                m_predators.Add(predator);
            }
        }

        /// <summary>
        /// Draws elements in current level
        /// </summary>
        public virtual void draw(SpriteBatch spriteBatch)
        {
            GameBoard.draw(spriteBatch);
            m_player.draw(spriteBatch);

            foreach (Flare flare in m_flares)
                flare.draw(spriteBatch);

            foreach (Explosion explosion in m_explosions)
                explosion.draw(spriteBatch);

            foreach (Spit spit in m_spits)
                spit.draw(spriteBatch);
            Status.draw(spriteBatch);
        }

        /// <summary>
        /// Lets know the Level class that shark was destroyed and another one can be generated
        /// </summary>
        public void destroyShark()
        {
            m_sharkNumber--;
        }

        /// <summary>
        /// Invokes event connected to ending level
        /// </summary>
        internal void endLevel()
        {
            Ended(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invokes event connected to restarting level
        /// </summary>
        internal void restartLevel()
        {
            Failed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Clears all items lists
        /// </summary>
        internal void clearLists()
        {
            m_predators.Clear();
            m_flares.Clear();
            m_explosions.Clear();
        }

        /// <summary>
        /// Places flares on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="flares">List of flares</param>
        internal void setFlares(ContentManager content, List<FlareParser> flares)
        {
            foreach (FlareParser flare in flares)
            {
                Flare f = new Flare(content.Load<Texture2D>("Items/Flare")
                    , new Rectangle((int)(flare.X / GameBoard.BlockSize.Width), (int)(flare.Y / GameBoard.BlockSize.Height)
                        , GameBoard.BlockSize.Width, GameBoard.BlockSize.Height)
                    , new Point((int)flare.XE, (int)flare.YE));
                m_flares.Add(f);
            }
        }

        /// <summary>
        /// Places explosions on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="explosions">List of explosions</param>
        internal void setExplosions(ContentManager content, List<ExplosionParser> explosions)
        {
            foreach (ExplosionParser explosion in explosions)
            {
                Explosion e = new Explosion(content.Load<Texture2D>("Items/Explosion")
                    , new Rectangle(explosion.X, explosion.Y, explosion.Width, explosion.Height));
                e.ExplosionLevel = explosion.Level;
                m_explosions.Add(e);
            }
        }

        /// <summary>
        /// Places spits on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="spits">List of spits</param>
        internal void setSpits(ContentManager content, List<SpitParser> spits)
        {
            foreach (SpitParser spit in spits)
            {
                Texture2D texture;
                if (spit.Lifes == 3)
                    texture = content.Load<Texture2D>("Items/Spit");
                else
                    texture = content.Load<Texture2D>("Items/Flare");

                Spit s = new Spit(texture, new Rectangle((int)spit.X, (int)spit.Y, GameBoard.BlockSize.Width, GameBoard.BlockSize.Height)
                    , new Point((int)spit.XE, (int)spit.YE), this, m_player, spit.Lifes, spit.Boxes);
                m_spits.Add(s);
            }
        }

        /// <summary>
        /// Places MutantSharks on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="sharks">List of MutantSharks</param>
        internal void setMutantSharks(ContentManager content, List<SimpleParser> sharks)
        {
            Texture2D texture = content.Load<Texture2D>("Items/Shark");
            foreach (SimpleParser shark in sharks)
            {
                MutantShark s = new MutantShark(texture
                    , new Rectangle(shark.X, shark.Y, GameBoard.BlockSize.Width * 2, GameBoard.BlockSize.Height * 2), this, m_player);
                s.IsBlinded = shark.IsBlinded;
                s.BlindedSeconds = shark.BlindedTime;
                m_predators.Add(s);
            }
        }

        /// <summary>
        /// Places MutantOctopus on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="octopus">List of MutantOctopus</param>
        internal void setMutantOctopus(ContentManager content, List<SimpleParser> octopus)
        {
            Texture2D texture = content.Load<Texture2D>("Items/Octopus");
            foreach (SimpleParser o in octopus)
            {
                MutantOctopus mutantOctopus = new MutantOctopus(texture
                    , new Rectangle(o.X, o.Y, GameBoard.BlockSize.Width * 3, GameBoard.BlockSize.Height * 3), this, m_player);
                mutantOctopus.IsBlinded = o.IsBlinded;
                mutantOctopus.BlindedSeconds = o.BlindedTime;
                m_predators.Add(mutantOctopus);
            }
        }

        /// <summary>
        /// Places Sharks on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="sharks">List of Sharks</param>
        internal void setSharks(ContentManager content, List<KeyValuePair<SimpleParser, int>> sharks)
        {
            Texture2D texture = content.Load<Texture2D>("Items/Shark");
            foreach (KeyValuePair<SimpleParser, int> shark in sharks)
            {
                Shark s = new Shark(texture
                    , new Rectangle(shark.Key.X, shark.Key.Y, GameBoard.BlockSize.Width, GameBoard.BlockSize.Height), this, m_player);
                s.RemainingTiles = shark.Value;
                s.IsBlinded = shark.Key.IsBlinded;
                s.BlindedSeconds = shark.Key.BlindedTime;
                m_predators.Add(s);
                m_sharkNumber++;
            }
        }

        /// <summary>
        /// Places GreenPirates on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="pirates">List of GreenPirates</param>
        /// <param name="type">Type of pirate</param>
        internal void setPirates(ContentManager content, List<SimpleParser> pirates, string type)
        {
            Texture2D texture = content.Load<Texture2D>("Items/PirateShip");
            foreach (SimpleParser pirate in pirates)
            {
                Predator p = null;
                switch (type)
                {
                    case "Green":
                        p = new GreenPirate(texture
                            , new Rectangle(pirate.X, pirate.Y, GameBoard.BlockSize.Width, GameBoard.BlockSize.Height), this, m_player);
                        break;
                    case "Red":
                        p = new RedPirate(texture
                            , new Rectangle(pirate.X, pirate.Y, GameBoard.BlockSize.Width, GameBoard.BlockSize.Height), this, m_player);
                        break;
                    case "Black":
                        p = new BlackPirate(texture
                            , new Rectangle(pirate.X, pirate.Y, GameBoard.BlockSize.Width, GameBoard.BlockSize.Height), this, m_player);
                        break;
                }
                p.IsBlinded = pirate.IsBlinded;
                p.BlindedSeconds = pirate.BlindedTime;
                m_predators.Add(p);
            }
        }

        /// <summary>
        /// Sets MutantCrab
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="crabs">List of MutantCrabs</param>
        internal void setMutantCrab(ContentManager content, List<MutantCrabParser> crabs)
        {
            Texture2D texture = content.Load<Texture2D>("Items/Crab");
            foreach (MutantCrabParser crab in crabs)
            {
                MutantCrab c = new MutantCrab(texture
                    , new Rectangle(crab.X, crab.Y, GameBoard.BlockSize.Width * 2, GameBoard.BlockSize.Height * 2), this, m_player);
                c.IsBlinded = crab.IsBlinded;
                c.BlindedSeconds = crab.BlindedTime;
                c.Destination = new Vector2(crab.DestX, crab.DestY);
                c.Direction = new Point(crab.DirectX, crab.DirectY);
                c.Step = new Vector2(crab.StepX, crab.StepY);
                c.ChangedDirection = crab.ChangedDirection;
                LevelThreeBoss level = this as LevelThreeBoss;
                level.CrabsNumber++;
                m_predators.Add(c);
            }
        }

        /// <summary>
        /// Places Octopus on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="octopus">List of Octopus</param>
        internal void setOctopus(ContentManager content, List<SimpleParser> octopus)
        {
            Texture2D texture = content.Load<Texture2D>("Items/Octopus");
            foreach (SimpleParser o in octopus)
            {
                Octopus oct = new Octopus(texture
                    , new Rectangle(o.X, o.Y, GameBoard.BlockSize.Width * 2, GameBoard.BlockSize.Height * 2), this, m_player);
                oct.IsBlinded = o.IsBlinded;
                oct.BlindedSeconds = o.BlindedTime;
                m_predators.Add(oct);
            }
        }

        /// <summary>
        /// Places Crab on the board
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="crabs">List of Crabs</param>
        internal void setCrabs(ContentManager content, List<SimpleParser> crabs)
        {
            Texture2D texture = content.Load<Texture2D>("Items/Crab");
            foreach (SimpleParser crab in crabs)
            {
                Crab c = new Crab(texture
                    , new Rectangle(crab.X, crab.Y, GameBoard.BlockSize.Width, GameBoard.BlockSize.Height), this, m_player);
                c.IsBlinded = crab.IsBlinded;
                c.BlindedSeconds = crab.BlindedTime;
                m_predators.Add(c);
            }
        }

        /// <summary>
        /// Updates Content in predators
        /// </summary>
        /// <param name="content"></param>
        internal void updateContent(ContentManager content)
        {
            Content = content;
            foreach (Predator p in m_predators)
                p.Content = Content;
            foreach (Spit s in m_spits)
                s.Content = Content;
        }

        /// <summary>
        /// Updates blindness on Predators
        /// </summary>
        internal void updateBlindness()
        {
            foreach (Predator predator in m_predators)
                predator.setBlind();
        }
        #endregion

        public event EventHandler<EventArgs> Ended;
        public event EventHandler<EventArgs> Failed;

        public enum LevelNumber
        {
            One,
            OneBoss,
            Two,
            TwoBoss,
            Three,
            ThreeBoss
        }
    }
}
