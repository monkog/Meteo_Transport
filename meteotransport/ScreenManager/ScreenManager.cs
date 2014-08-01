#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace Meteo
{
    /// <summary>
    /// Manages one or more GameScreen instances
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields
        /// <summary>
        /// List of screens
        /// </summary>
        List<GameScreen> m_screens = new List<GameScreen>();
        /// <summary>
        /// Screens to update
        /// </summary>
        List<GameScreen> m_screensToUpdate = new List<GameScreen>();

        /// <summary>
        /// Input state
        /// </summary>
        InputState m_input = new InputState();
        /// <summary>
        /// Sprite batch
        /// </summary>
        SpriteBatch m_spriteBatch;
        /// <summary>
        /// Font
        /// </summary>
        SpriteFont m_font;
        /// <summary>
        /// Texture for drawing black transparent background
        /// </summary>
        Texture2D m_blankTexture;

        bool isInitialized;

        bool traceEnabled;

        #endregion

        #region Properties

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return m_spriteBatch; }
        }

        /// <summary>
        /// A default font shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font
        {
            get { return m_font; }
        }
        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        { }


        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            m_font = content.Load<SpriteFont>("menufont");
            m_blankTexture = content.Load<Texture2D>("blank");

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in m_screens)
            {
                screen.LoadContent();
            }
        }

        /// <summary>
        /// Unloads graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (GameScreen screen in m_screens)
                screen.UnloadContent();
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Allows to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            m_input.Update();
            m_screensToUpdate.Clear();

            foreach (GameScreen screen in m_screens)
                m_screensToUpdate.Add(screen);

            bool coveredByOtherScreen = false;
            bool otherScreenHasFocus = !Game.IsActive;

            while (m_screensToUpdate.Count > 0)
            {
                GameScreen screen = m_screensToUpdate[m_screensToUpdate.Count - 1];
                m_screensToUpdate.RemoveAt(m_screensToUpdate.Count - 1);
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(m_input);
                        otherScreenHasFocus = true;
                    }

                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }


        /// <summary>
        /// Draws all screens
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in m_screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }


        #endregion

        #region Methods
        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen)
        {
            screen.IsExiting = false;
            screen.ScreenManager = this;

            if (isInitialized)
                screen.LoadContent();

            m_screens.Add(screen);
        }


        /// <summary>
        /// Removes a screen from the screen manager
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            if (isInitialized)
                screen.UnloadContent();

            m_screensToUpdate.Remove(screen);
            m_screens.Remove(screen);
        }


        /// <summary>
        /// Provides array holding all the screens
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return m_screens.ToArray();
        }


        /// <summary>
        /// Draws transparent black screen
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);
            m_spriteBatch.End();
        }
        #endregion
    }
}
