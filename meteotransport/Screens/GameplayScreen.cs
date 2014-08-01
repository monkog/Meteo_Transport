#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Meteo.Levels;
using Meteo.Items;
using Meteo.GameBoard;
using Meteo.Screens;
using Meteo.Helpers;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Meteo
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : MenuScreen
    {
        #region Fields
        /// <summary>
        /// Content Manager
        /// </summary>
        ContentManager m_content;
        /// <summary>
        /// Game font
        /// </summary>
        SpriteFont m_gameFont;
        /// <summary>
        /// Changes alpha enabling fading out
        /// </summary>
        float m_pauseAlpha;

        /// <summary>
        /// Background image
        /// </summary>
        Texture2D m_backgroundImage;
        /// <summary>
        /// Background rectangle
        /// </summary>
        Rectangle m_backgroundRectangle;

        /// <summary>
        /// Current instance of level
        /// </summary>
        Level m_currentLevel;
        /// <summary>
        /// Game difficulty
        /// </summary>
        private int m_difficulty;
        /// <summary>
        /// Logged user
        /// </summary>
        private User LoggedUser;
        /// <summary>
        /// Logged player
        /// </summary>
        Player m_player;

        /// <summary>
        /// Game music
        /// </summary>
        protected Song Music;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(User user, int difficulty, string title)
            : base(title)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            LoggedUser = user;
            m_difficulty = difficulty;
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// Loads chosen game
        /// </summary>
        public void loadGame(string fileName)
        {
            GameSaver.loadGame(m_content, LoggedUser, ref m_player, ref m_currentLevel, ref m_difficulty
                , ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, fileName);
            m_currentLevel.Content = m_content;
            m_currentLevel.Status.loadContent(m_content);
            m_currentLevel.Ended += (sender, e) => viewStatistics(sender, e, m_currentLevel.LevelId);
            m_currentLevel.Failed += (sender, e) => restartLevel(sender, e, m_currentLevel.LevelId);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (m_content == null)
                m_content = new ContentManager(ScreenManager.Game.Services, "Content");

            Music = m_content.Load<Song>("Sounds/Music");
            MediaPlayer.Volume = 0;
            MediaPlayer.Play(Music);
            m_gameFont = m_content.Load<SpriteFont>("gamefont");
            m_backgroundImage = m_content.Load<Texture2D>("Backgrounds/GreenWater");

            m_backgroundRectangle = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

            Thread.Sleep(1000);
            ScreenManager.Game.ResetElapsedTime();

            Texture2D playerTexture = m_content.Load<Texture2D>("Items/Player");

            m_player = new Player(playerTexture, new Rectangle(0, 0, 3 * ScreenManager.GraphicsDevice.Viewport.Width / (4 * Board.WIDTH)
                , ScreenManager.GraphicsDevice.Viewport.Height / Board.HEIGHT), LoggedUser);

            prepareLevel(Level.LevelNumber.One);
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            m_content.Unload();
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the state of the game. 
        /// </summary>
        /// <remarks>
        /// The game will stop updating when the pause menu is active,
        /// or if you switch to a different application.
        /// </remarks>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            MediaPlayer.Volume = LoggedUser.MusicOn == true ? 1 : 0;
            m_currentLevel.SoundsOn = LoggedUser.SoundsOn;

            if (IsActive)
                m_currentLevel.update();

            if (coveredByOtherScreen)
                m_pauseAlpha = Math.Min(m_pauseAlpha + 1f / 32, 1);
            else
                m_pauseAlpha = Math.Max(m_pauseAlpha - 1f / 32, 0);
        }


        /// <summary>
        /// Handles the input
        /// </summary>
        public override void HandleInput(InputState input)
        {
            KeyboardState keyboardState = input.CurrentKeyboardState;

            if (input.IsPauseGame())
                ScreenManager.AddScreen(new PauseMenuScreen(LoggedUser, m_currentLevel, m_player, ""));
        }

        /// <summary>
        /// Draws the screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(m_backgroundImage, m_backgroundRectangle, Color.White);
            m_currentLevel.draw(spriteBatch);

            spriteBatch.End();

            if (TransitionPosition > 0 || m_pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, m_pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Changes level number and updates level
        /// </summary>
        /// <param name="level"></param>
        void prepareLevel(Level.LevelNumber level)
        {
            m_currentLevel = null;
            switch (level)
            {
                case Level.LevelNumber.One:
                    m_player.CurrentLevel = 1;
                    m_currentLevel = new LevelOne(m_player, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, m_difficulty);
                    break;
                case Level.LevelNumber.OneBoss:
                    m_currentLevel = new LevelOneBoss(m_player, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, m_difficulty);
                    break;
                case Level.LevelNumber.Two:
                    m_player.CurrentLevel = 2;
                    m_currentLevel = new LevelTwo(m_player, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, m_difficulty);
                    break;
                case Level.LevelNumber.TwoBoss:
                    m_currentLevel = new LevelTwoBoss(m_player, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, m_difficulty);
                    break;
                case Level.LevelNumber.Three:
                    m_player.CurrentLevel = 3;
                    m_currentLevel = new LevelThree(m_player, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, m_difficulty);
                    break;
                case Level.LevelNumber.ThreeBoss:
                    m_currentLevel = new LevelThreeBoss(m_player, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, m_difficulty);
                    break;
            }

            m_currentLevel.loadContent(m_content);
            m_player.setLevel(m_currentLevel);
            if (m_currentLevel.LevelId != Level.LevelNumber.ThreeBoss)
                m_currentLevel.Ended += (sender, e) => viewStatistics(sender, e, level);
            else
                m_currentLevel.Ended += endGame;
            m_currentLevel.Failed += (sender, e) => restartLevel(sender, e, level);
        }

        /// <summary>
        /// Views the statistics after level
        /// </summary>
        void viewStatistics(object _sender, EventArgs _e, Level.LevelNumber level)
        {
            StatisticsScreen statistics = new StatisticsScreen(LoggedUser, m_player, m_currentLevel, "Next Level", "");
            statistics.Ended += (sender, e) => m_currentLevel_Ended(sender, e, level);
            ScreenManager.AddScreen(statistics);
        }

        /// <summary>
        /// Restarts current level
        /// </summary>
        void restartLevel(object _sender, EventArgs _e, Level.LevelNumber level)
        {
            StatisticsScreen statistics = new StatisticsScreen(LoggedUser, m_player, m_currentLevel, "Restart Level", "");
            ScreenManager.AddScreen(statistics);
            statistics.Ended += (sender, e) => restartLevelScreen(level);
        }

        /// <summary>
        /// Determines which level to restart
        /// </summary>
        /// <param name="level">Failed level</param>
        private void restartLevelScreen(Level.LevelNumber level)
        {
            switch (level)
            {
                case Level.LevelNumber.One:
                case Level.LevelNumber.OneBoss:
                    prepareLevel(Level.LevelNumber.One);
                    break;
                case Level.LevelNumber.Two:
                case Level.LevelNumber.TwoBoss:
                    prepareLevel(Level.LevelNumber.Two);
                    break;
                case Level.LevelNumber.Three:
                case Level.LevelNumber.ThreeBoss:
                    prepareLevel(Level.LevelNumber.Three);
                    break;
            }
        }

        /// <summary>
        /// Ends the game
        /// </summary>
        void endGame(object _sender, EventArgs _e)
        {
            StatisticsScreen statistics = new StatisticsScreen(LoggedUser, m_player, m_currentLevel, "Main Menu", "");
            ScreenManager.AddScreen(statistics);
            statistics.Ended += gameEnded;
        }

        /// <summary>
        /// Ends the game and saves high score
        /// </summary>
        void gameEnded(object sender, EventArgs e)
        {
            GameSaver.saveHighScores(m_player);
            ExitScreen();
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen(LoggedUser, "Main Menu"));
        }

        /// <summary>
        /// Switches game to next level
        /// </summary>
        void m_currentLevel_Ended(object sender, EventArgs e, Level.LevelNumber level)
        {
            switch (level)
            {
                case Level.LevelNumber.One:
                    prepareLevel(Level.LevelNumber.OneBoss);
                    break;
                case Level.LevelNumber.OneBoss:
                    prepareLevel(Level.LevelNumber.Two);
                    break;
                case Level.LevelNumber.Two:
                    prepareLevel(Level.LevelNumber.TwoBoss);
                    break;
                case Level.LevelNumber.TwoBoss:
                    prepareLevel(Level.LevelNumber.Three);
                    break;
                case Level.LevelNumber.Three:
                    prepareLevel(Level.LevelNumber.ThreeBoss);
                    break;
                case Level.LevelNumber.ThreeBoss:
                    endGame(sender, e);
                    break;
            }
        }
        #endregion
    }
}
