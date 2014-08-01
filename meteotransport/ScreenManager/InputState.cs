#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
#endregion

namespace Meteo
{
    /// <summary>
    /// Helper for reading input 
    /// </summary>
    public class InputState
    {
        #region Fields
        /// <summary>
        /// Current Keyboard state
        /// </summary>
        public KeyboardState CurrentKeyboardState;
        /// <summary>
        /// Current mouse state
        /// </summary>
        public MouseState CurrentMouseState;
        /// <summary>
        /// Last Keyboard state
        /// </summary>
        public KeyboardState LastKeyboardState;
        /// <summary>
        /// Last mouse state
        /// </summary>
        public MouseState LastMouseState;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardState = Keyboard.GetState();
            LastKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();
            LastMouseState = CurrentMouseState;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reads the latest state of the keyboard
        /// </summary>
        public void Update()
        {
            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
        }


        /// <summary>
        /// Checking if a key was pressed during this update
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        /// Checks for a menu selection
        /// </summary>
        public bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Enter)
                || (CurrentMouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released);
        }


        /// <summary>
        /// Checks for a menu cancel
        /// </summary>
        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape);
        }

        /// <summary>
        /// Checks for a menu up
        /// </summary>
        public bool IsMenuUp()
        {
            return IsNewKeyPress(Keys.Up);
        }


        /// <summary>
        /// Checks for a menu down
        /// </summary>
        public bool IsMenuDown()
        {
            return IsNewKeyPress(Keys.Down);
        }


        /// <summary>
        /// Checks for a pause 
        /// </summary>
        public bool IsPauseGame()
        {
            return IsNewKeyPress(Keys.Escape);
        }
        #endregion
    }
}
