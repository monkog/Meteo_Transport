#region Using Statements
using Meteo.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
#endregion

namespace Meteo
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class MeteoTransport : Microsoft.Xna.Framework.Game
    {
        #region variables
        /// <summary>
        /// Represents Graphics Device Manager 
        /// </summary>
        GraphicsDeviceManager m_graphics;
        /// <summary>
        /// Screen manager for current game
        /// </summary>
        ScreenManager m_screenManager;
        /// <summary>
        /// User logged to the game
        /// </summary>
        public User LoggedUser { get; private set; }
        /// <summary>
        /// High scores file path
        /// </summary>
        public static string ConfigFile = @".\Conf\passwords.xml";


        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "Backgrounds/ToxicBackground",
        };
        #endregion

        #region constructors
        /// <summary>
        /// The main game constructor.
        /// </summary>
        public MeteoTransport()
        {
            Content.RootDirectory = "Content";

            var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            m_graphics = new GraphicsDeviceManager(this);
            
            IsMouseVisible = true;

            // Create the screen manager component.
            m_screenManager = new ScreenManager(this);

            Components.Add(m_screenManager);

            // Activate the first screens.
            m_screenManager.AddScreen(new BackgroundScreen());
            m_screenManager.AddScreen(new LoggingScreen(""));
        }
        #endregion

        #region methods
        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }

            if (!Directory.Exists(@".\Conf"))
                Directory.CreateDirectory(@".\Conf");

            if (!File.Exists(ConfigFile))
            {
                File.Create(ConfigFile).Close();
                StreamWriter writer = new StreamWriter(ConfigFile);
                writer.WriteLine("<Users>\n</Users>");
                writer.Close();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            m_graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (MeteoTransport game = new MeteoTransport())
            {
                game.Run();
            }
        }
    }

    #endregion
}
