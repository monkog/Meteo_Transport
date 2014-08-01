using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Meteo.Items
{
    class KeyHelpers
    {
        /// <summary>
        /// Parses key read from given XmlNode
        /// </summary>
        /// <param name="xmlNode">XmlNode</param>
        /// <param name="keyNode">Node to select</param>
        /// <param name="defaultKey">Default Key value</param>
        /// <returns></returns>
        public static Keys parseKey(XmlNode xmlNode, string keyNode, Keys defaultKey)
        {
            return parseKeyFromString(xmlNode.SelectSingleNode(keyNode).InnerText, defaultKey);
        }

        /// <summary>
        /// Parses string to Key
        /// </summary>
        /// <param name="value">String</param>
        /// <param name="defaultKey">Default Key value</param>
        /// <returns></returns>
        public static Keys parseKeyFromString(string value, Keys defaultKey)
        {
            Keys key;
            Keys.TryParse(value, true, out key);
            if (key == Keys.P || key == Keys.Enter)
                return defaultKey;
            return key;
        }
    }
}
