using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteo
{
    /// <summary>
    /// Represents a logged user
    /// </summary>
    public class User
    {
        #region variables
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Left Key value
        /// </summary>
        public Keys LeftKey { get; set; }
        /// <summary>
        /// Right Key value
        /// </summary>
        public Keys RightKey { get; set; }
        /// <summary>
        /// Up Key value
        /// </summary>
        public Keys UpKey { get; set; }
        /// <summary>
        /// Down Key value
        /// </summary>
        public Keys DownKey { get; set; }
        /// <summary>
        /// Turbo speed Key value
        /// </summary>
        public Keys TurboKey { get; set; }
        /// <summary>
        /// Is music on
        /// </summary>
        public bool MusicOn { get; set; }
        /// <summary>
        /// Are sounds on
        /// </summary>
        public bool SoundsOn { get; set; }
        #endregion

        /// <summary>
        /// Checks if username and password match
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>True if both username and password match</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(User))
                return false;

            User user = obj as User;
            return user.Username == Username && user.Password == Password;
        }
    }
}
