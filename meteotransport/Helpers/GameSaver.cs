using Meteo.GameBoard;
using Meteo.Items;
using Meteo.Items.Predators;
using Meteo.Items.Predators.Animals;
using Meteo.Items.Predators.Pirates;
using Meteo.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Meteo.Helpers
{
    class GameSaver
    {
        #region variables
        /// <summary>
        /// Path to file with saved games
        /// </summary>
        private static string m_savesFile = @".\Conf\save.sav";
        /// <summary>
        /// High scores file path
        /// </summary>
        private static string m_highScoresFile = @".\Conf\highscores.xml";
        /// <summary>
        /// Max high scores entries
        /// </summary>
        private static int MAX_ENTRIES = 10;
        #endregion

        #region methods
        /// <summary>
        /// Saves High score
        /// </summary>
        /// <param name="player">Player</param>
        internal static void saveHighScores(Player player)
        {
            checkForFile(m_highScoresFile);
            List<BestUser> userEntries = new List<BestUser>();
            XmlDocument document = new XmlDocument();
            document.Load(m_highScoresFile);

            XmlNodeList usersListNode = document.SelectSingleNode("Users").SelectNodes("User");
            XmlNode users = document.SelectSingleNode("Users");

            foreach (XmlNode node in usersListNode)
            {
                BestUser user = new BestUser();
                user.Username = node.SelectSingleNode("Username").InnerText;
                user.Points = int.Parse(node.SelectSingleNode("Points").InnerText);
                userEntries.Add(user);
            }
            {
                BestUser user = new BestUser();
                user.Username = player.LoggedUser.Username;
                user.Points = player.TotalPoints;
                userEntries.Add(user);
            }

            userEntries.Sort((x, y) => x.Points > y.Points ? -1 : 1);
            document.SelectSingleNode("Users").RemoveAll();

            int index = 0;
            foreach (BestUser user in userEntries)
            {
                index++;
                XmlElement bestUser = document.CreateElement("User");
                XmlElement username = document.CreateElement("Username");
                username.InnerText = user.Username;
                bestUser.AppendChild(username);
                XmlElement points = document.CreateElement("Points");
                points.InnerText = user.Points.ToString();
                bestUser.AppendChild(points);
                users.AppendChild(bestUser);

                if (index >= MAX_ENTRIES)
                    break;
            }
            document.Save(m_highScoresFile);
        }

        /// <summary>
        /// Gets saved entries
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Returns list of saved entries</returns>
        internal static List<string> getSaveEntries(string user)
        {
            checkForFile(m_savesFile);
            List<string> userEntries = new List<string>();
            XmlDocument document = new XmlDocument();
            document.Load(m_savesFile);

            XmlNodeList usersListNode = document.SelectSingleNode("Users").SelectNodes("User");

            foreach (XmlNode node in usersListNode)
            {
                XmlNode username = node.SelectSingleNode("Username");
                if (user == username.InnerText)
                    userEntries.Add(node.SelectSingleNode("Save").InnerText);
            }
            return userEntries;
        }

        /// <summary>
        /// Saves the current game
        /// </summary>
        /// <param name="user">Logged User</param>
        /// <param name="level">Current level</param>
        /// <param name="player">Game player</param>
        internal static void saveGame(string user, string filename, Level level, Player player)
        {
            checkForFile(m_savesFile);

            XmlDocument doc = new XmlDocument();
            doc.Load(m_savesFile);

            XmlElement userNode = doc.CreateElement("User");
            XmlElement save = doc.CreateElement("Save");
            save.InnerText = filename;
            userNode.AppendChild(save);
            XmlElement username = doc.CreateElement("Username");
            username.InnerText = user;
            userNode.AppendChild(username);
            XmlElement levelNode = doc.CreateElement("Level");
            levelNode.InnerText = level.LevelId.ToString();
            userNode.AppendChild(levelNode);
            XmlElement difficultyNode = doc.CreateElement("Difficulty");
            difficultyNode.InnerText = level.Difficulty.ToString();
            userNode.AppendChild(difficultyNode);
            #region player
            XmlElement playerNode = doc.CreateElement("Player");
            XmlElement points = doc.CreateElement("Points");
            points.InnerText = player.Points.ToString();
            playerNode.AppendChild(points);
            XmlElement totalPoints = doc.CreateElement("TotalPoints");
            totalPoints.InnerText = player.TotalPoints.ToString();
            playerNode.AppendChild(totalPoints);
            XmlElement predators = doc.CreateElement("Predators");
            predators.InnerText = player.ShotPredators.ToString();
            playerNode.AppendChild(predators);
            XmlElement totalPredators = doc.CreateElement("TotalPredators");
            totalPredators.InnerText = player.TotalShotPredators.ToString();
            playerNode.AppendChild(totalPredators);
            XmlElement lifes = doc.CreateElement("Lifes");
            lifes.InnerText = player.Lifes.ToString();
            playerNode.AppendChild(lifes);
            XmlElement speed = doc.CreateElement("Speed");
            speed.InnerText = player.Speed.ToString();
            playerNode.AppendChild(speed);
            XmlElement boxes = doc.CreateElement("Boxes");
            boxes.InnerText = player.Boxes.ToString();
            playerNode.AppendChild(boxes);
            XmlElement flares = doc.CreateElement("Flares");
            flares.InnerText = player.FlaresNumber.ToString();
            playerNode.AppendChild(flares);
            XmlElement turbo = doc.CreateElement("Turbo");
            turbo.InnerText = player.TurboLevel.ToString();
            playerNode.AppendChild(turbo);
            XmlElement drunk = doc.CreateElement("Drunk");
            drunk.InnerText = player.IsDrunk.ToString();
            playerNode.AppendChild(drunk);
            XmlElement drunkTime = doc.CreateElement("DrunkTime");
            drunkTime.InnerText = player.ElapsedDrunkSeconds.ToString();
            playerNode.AppendChild(drunkTime);
            XmlElement position = doc.CreateElement("BoardPosition");
            XmlElement positionX = doc.CreateElement("X");
            positionX.InnerText = player.BoardPosition.X.ToString();
            position.AppendChild(positionX);
            XmlElement positionY = doc.CreateElement("Y");
            positionY.InnerText = player.BoardPosition.Y.ToString();
            position.AppendChild(positionY);
            playerNode.AppendChild(position);
            userNode.AppendChild(playerNode);
            #endregion
            #region board
            IceBlock[,] board = level.GameBoard.IceBlocks;

            XmlElement boardNode = doc.CreateElement("Board");
            XmlElement boxesNode = doc.CreateElement("Boxes");
            XmlElement predatorsNode = doc.CreateElement("Predators");
            XmlElement sharksNode = doc.CreateElement("Sharks");
            XmlElement octopusNode = doc.CreateElement("Octopus");
            XmlElement crabsNode = doc.CreateElement("Crabs");
            XmlElement mutantSharksNode = doc.CreateElement("MutantSharks");
            XmlElement mutantOctopusNode = doc.CreateElement("MutantOctopus");
            XmlElement mutantCrabsNode = doc.CreateElement("MutantCrabs");
            XmlElement greenPiratesNode = doc.CreateElement("GreenPirates");
            XmlElement redPiratesNode = doc.CreateElement("RedPirates");
            XmlElement blackPiratesNode = doc.CreateElement("BlackPirates");

            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    XmlElement x = doc.CreateElement("X");
                    x.InnerText = i.ToString();
                    XmlElement y = doc.CreateElement("Y");
                    y.InnerText = j.ToString();

                    if (board[i, j] != null)
                    {
                        XmlElement block = doc.CreateElement("IceBlock");
                        block.AppendChild(x);
                        block.AppendChild(y);
                        XmlElement frozen = doc.CreateElement("Frozen");
                        frozen.InnerText = board[i, j].FrozenLevel.ToString();
                        block.AppendChild(frozen);
                        boardNode.AppendChild(block);
                    }
                    List<Item> items = new List<Item>();
                    if ((items = level.GameBoard.isFree(i, j)).Count > 0)
                        foreach (Item item in items)
                        {
                            if (item.GetType() == typeof(Box))
                            {
                                Box currentBox = item as Box;

                                XmlElement box = doc.CreateElement("Box");
                                box.AppendChild(x);
                                box.AppendChild(y);

                                XmlElement boxType = doc.CreateElement("BoxType");
                                boxType.InnerText = currentBox.Type.ToString();
                                box.AppendChild(boxType);

                                boxesNode.AppendChild(box);
                            }
                            else if (item.GetType() == typeof(Shark))
                            {
                                Shark s = item as Shark;

                                XmlElement shark = doc.CreateElement("Shark");
                                shark.AppendChild(x);
                                shark.AppendChild(y);
                                XmlElement remainingTiles = doc.CreateElement("Tiles");
                                remainingTiles.InnerText = s.RemainingTiles.ToString();
                                shark.AppendChild(remainingTiles);
                                XmlElement blinded = doc.CreateElement("Blinded");
                                blinded.InnerText = s.IsBlinded.ToString();
                                shark.AppendChild(blinded);
                                XmlElement blindedTime = doc.CreateElement("BlindedTime");
                                blindedTime.InnerText = s.BlindedSeconds.ToString();
                                shark.AppendChild(blindedTime);
                                sharksNode.AppendChild(shark);
                            }
                            else if (item.GetType() == typeof(MutantShark))
                            {
                                MutantShark s = item as MutantShark;

                                XmlElement shark = doc.CreateElement("MutantShark");
                                shark.AppendChild(x);
                                shark.AppendChild(y);
                                XmlElement blinded = doc.CreateElement("Blinded");
                                blinded.InnerText = s.IsBlinded.ToString();
                                shark.AppendChild(blinded);
                                XmlElement blindedTime = doc.CreateElement("BlindedTime");
                                blindedTime.InnerText = s.BlindedSeconds.ToString();
                                shark.AppendChild(blindedTime);
                                mutantSharksNode.AppendChild(shark);
                            }
                            else if (item.GetType() == typeof(Octopus))
                            {
                                Octopus o = item as Octopus;

                                XmlElement octopus = doc.CreateElement("Octopus");
                                octopus.AppendChild(x);
                                octopus.AppendChild(y);
                                XmlElement blinded = doc.CreateElement("Blinded");
                                blinded.InnerText = o.IsBlinded.ToString();
                                octopus.AppendChild(blinded);
                                XmlElement blindedTime = doc.CreateElement("BlindedTime");
                                blindedTime.InnerText = o.BlindedSeconds.ToString();
                                octopus.AppendChild(blindedTime);
                                octopusNode.AppendChild(octopus);
                            }
                            else if (item.GetType() == typeof(MutantOctopus))
                            {
                                MutantOctopus o = item as MutantOctopus;

                                XmlElement octopus = doc.CreateElement("MutantOctopus");
                                octopus.AppendChild(x);
                                octopus.AppendChild(y);
                                XmlElement blinded = doc.CreateElement("Blinded");
                                blinded.InnerText = o.IsBlinded.ToString();
                                octopus.AppendChild(blinded);
                                XmlElement blindedTime = doc.CreateElement("BlindedTime");
                                blindedTime.InnerText = o.BlindedSeconds.ToString();
                                octopus.AppendChild(blindedTime);
                                mutantOctopusNode.AppendChild(octopus);
                            }
                            else if (item.GetType() == typeof(Crab))
                            {
                                Crab c = item as Crab;

                                XmlElement crab = doc.CreateElement("Crab");
                                crab.AppendChild(x);
                                crab.AppendChild(y);
                                XmlElement blinded = doc.CreateElement("Blinded");
                                blinded.InnerText = c.IsBlinded.ToString();
                                crab.AppendChild(blinded);
                                XmlElement blindedTime = doc.CreateElement("BlindedTime");
                                blindedTime.InnerText = c.BlindedSeconds.ToString();
                                crab.AppendChild(blindedTime);
                                crabsNode.AppendChild(crab);
                            }
                            else if (item.GetType() == typeof(MutantCrab))
                            {
                                MutantCrab c = item as MutantCrab;

                                XmlElement crab = doc.CreateElement("MutantCrab");
                                crab.AppendChild(x);
                                crab.AppendChild(y);
                                XmlElement blinded = doc.CreateElement("Blinded");
                                blinded.InnerText = c.IsBlinded.ToString();
                                crab.AppendChild(blinded);
                                XmlElement blindedTime = doc.CreateElement("BlindedTime");
                                blindedTime.InnerText = c.BlindedSeconds.ToString();
                                crab.AppendChild(blindedTime);
                                XmlElement step = doc.CreateElement("Step");
                                XmlElement stepX = doc.CreateElement("X");
                                stepX.InnerText = c.Step.X.ToString();
                                step.AppendChild(stepX);
                                XmlElement stepY = doc.CreateElement("Y");
                                stepY.InnerText = c.Step.Y.ToString();
                                step.AppendChild(stepY);
                                crab.AppendChild(step);
                                XmlElement destination = doc.CreateElement("Destination");
                                XmlElement destinationX = doc.CreateElement("X");
                                destinationX.InnerText = c.Destination.X.ToString();
                                destination.AppendChild(destinationX);
                                XmlElement destinationY = doc.CreateElement("Y");
                                destinationY.InnerText = c.Destination.Y.ToString();
                                destination.AppendChild(destinationY);
                                crab.AppendChild(destination);
                                XmlElement direction = doc.CreateElement("Direction");
                                XmlElement directionX = doc.CreateElement("X");
                                directionX.InnerText = c.Direction.X.ToString();
                                direction.AppendChild(directionX);
                                XmlElement directionY = doc.CreateElement("Y");
                                directionY.InnerText = c.Direction.Y.ToString();
                                direction.AppendChild(directionY);
                                crab.AppendChild(direction);
                                XmlElement changedDirection = doc.CreateElement("ChangedDirection");
                                changedDirection.InnerText = c.ChangedDirection.ToString();
                                crab.AppendChild(changedDirection);
                                mutantCrabsNode.AppendChild(crab);
                            }
                            else if (item.GetType() == typeof(GreenPirate))
                            {
                                GreenPirate p = item as GreenPirate;

                                XmlElement pirate = doc.CreateElement("GreenPirate");
                                pirate.AppendChild(x);
                                pirate.AppendChild(y);
                                XmlElement blinded = doc.CreateElement("Blinded");
                                blinded.InnerText = p.IsBlinded.ToString();
                                pirate.AppendChild(blinded);
                                XmlElement blindedTime = doc.CreateElement("BlindedTime");
                                blindedTime.InnerText = p.BlindedSeconds.ToString();
                                pirate.AppendChild(blindedTime);
                                greenPiratesNode.AppendChild(pirate);
                            }
                            else if (item.GetType() == typeof(RedPirate))
                            {
                                RedPirate p = item as RedPirate;

                                XmlElement pirate = doc.CreateElement("RedPirate");
                                pirate.AppendChild(x);
                                pirate.AppendChild(y);
                                XmlElement blinded = doc.CreateElement("Blinded");
                                blinded.InnerText = p.IsBlinded.ToString();
                                pirate.AppendChild(blinded);
                                XmlElement blindedTime = doc.CreateElement("BlindedTime");
                                blindedTime.InnerText = p.BlindedSeconds.ToString();
                                pirate.AppendChild(blindedTime);
                                redPiratesNode.AppendChild(pirate);
                            }
                            else if (item.GetType() == typeof(BlackPirate))
                            {
                                BlackPirate p = item as BlackPirate;

                                XmlElement pirate = doc.CreateElement("BlackPirate");
                                pirate.AppendChild(x);
                                pirate.AppendChild(y);
                                XmlElement blinded = doc.CreateElement("Blinded");
                                blinded.InnerText = p.IsBlinded.ToString();
                                pirate.AppendChild(blinded);
                                XmlElement blindedTime = doc.CreateElement("BlindedTime");
                                blindedTime.InnerText = p.BlindedSeconds.ToString();
                                pirate.AppendChild(blindedTime);
                                blackPiratesNode.AppendChild(pirate);
                            }
                        }
                }
            predatorsNode.AppendChild(sharksNode);
            predatorsNode.AppendChild(octopusNode);
            predatorsNode.AppendChild(crabsNode);
            predatorsNode.AppendChild(mutantSharksNode);
            predatorsNode.AppendChild(mutantOctopusNode);
            predatorsNode.AppendChild(mutantCrabsNode);
            predatorsNode.AppendChild(greenPiratesNode);
            predatorsNode.AppendChild(redPiratesNode);
            predatorsNode.AppendChild(blackPiratesNode);
            #endregion
            #region other items
            XmlElement flaresNode = doc.CreateElement("Flares");
            foreach (Flare f in level.m_flares)
            {
                XmlElement flare = doc.CreateElement("Flare");

                XmlElement x = doc.CreateElement("X");
                x.InnerText = f.Position.X.ToString();
                flare.AppendChild(x);

                XmlElement y = doc.CreateElement("Y");
                y.InnerText = f.Position.Y.ToString();
                flare.AppendChild(y);

                XmlElement end = doc.CreateElement("End");

                XmlElement xE = doc.CreateElement("X");
                xE.InnerText = f.EndPoint.X.ToString();
                end.AppendChild(xE);

                XmlElement yE = doc.CreateElement("Y");
                yE.InnerText = f.EndPoint.Y.ToString();
                end.AppendChild(yE);
                flare.AppendChild(end);

                flaresNode.AppendChild(flare);
            }

            XmlElement explosionsNode = doc.CreateElement("Explosions");
            foreach (Explosion e in level.m_explosions)
            {
                XmlElement explosion = doc.CreateElement("Explosion");

                XmlElement x = doc.CreateElement("X");
                x.InnerText = e.Position.X.ToString();
                explosion.AppendChild(x);

                XmlElement y = doc.CreateElement("Y");
                y.InnerText = e.Position.Y.ToString();
                explosion.AppendChild(y);

                XmlElement width = doc.CreateElement("Width");
                width.InnerText = e.ItemSize.Width.ToString();
                explosion.AppendChild(width);

                XmlElement height = doc.CreateElement("Height");
                height.InnerText = e.ItemSize.Height.ToString();
                explosion.AppendChild(height);

                XmlElement explosionLevel = doc.CreateElement("Level");
                explosionLevel.InnerText = e.ExplosionLevel.ToString();
                explosion.AppendChild(explosionLevel);

                explosionsNode.AppendChild(explosion);
            }

            XmlElement spitsNode = doc.CreateElement("Spits");
            foreach (Spit s in level.m_spits)
            {
                XmlElement spit = doc.CreateElement("Spit");

                XmlElement x = doc.CreateElement("X");
                x.InnerText = s.Position.X.ToString();
                spit.AppendChild(x);

                XmlElement y = doc.CreateElement("Y");
                y.InnerText = s.Position.Y.ToString();
                spit.AppendChild(y);

                XmlElement end = doc.CreateElement("End");

                XmlElement xE = doc.CreateElement("X");
                xE.InnerText = s.EndPoint.X.ToString();
                end.AppendChild(xE);

                XmlElement yE = doc.CreateElement("Y");
                yE.InnerText = s.EndPoint.Y.ToString();
                end.AppendChild(yE);
                spit.AppendChild(end);

                XmlElement boxesSpit = doc.CreateElement("Boxes");
                boxesSpit.InnerText = s.m_boxes.ToString();
                spit.AppendChild(boxesSpit);

                XmlElement lifesSpit = doc.CreateElement("Lifes");
                lifesSpit.InnerText = s.m_lifes.ToString();
                spit.AppendChild(lifesSpit);

                spitsNode.AppendChild(spit);
            }
            #endregion
            userNode.AppendChild(boardNode);
            userNode.AppendChild(boxesNode);
            userNode.AppendChild(predatorsNode);
            userNode.AppendChild(flaresNode);
            userNode.AppendChild(explosionsNode);
            userNode.AppendChild(spitsNode);
            doc.DocumentElement.AppendChild(userNode);
            doc.Save(m_savesFile);
        }

        /// <summary>
        /// Loads game
        /// </summary>
        public static void loadGame(ContentManager content, User loggedUser, ref Player player
            , ref Level level, ref int difficulty, int width, int height, string filename)
        {
            XmlDocument document = new XmlDocument();
            document.Load(m_savesFile);

            XmlNodeList usersListNode = document.SelectSingleNode("Users").SelectNodes("User");

            foreach (XmlNode node in usersListNode)
            {
                XmlNode username = node.SelectSingleNode("Username");
                XmlNode fileName = node.SelectSingleNode("Save");
                if (loggedUser.Username != username.InnerText || fileName.InnerText != filename)
                    continue;

                XmlNode difficultyNode = node.SelectSingleNode("Difficulty");
                difficulty = int.Parse(difficultyNode.InnerText);

                XmlNode playerNode = node.SelectSingleNode("Player");
                player.ShotPredators = int.Parse(playerNode.SelectSingleNode("Predators").InnerText);
                player.TotalShotPredators = int.Parse(playerNode.SelectSingleNode("TotalPredators").InnerText);
                player.Speed = int.Parse(playerNode.SelectSingleNode("Speed").InnerText);
                player.Boxes = int.Parse(playerNode.SelectSingleNode("Boxes").InnerText);
                player.Lifes = int.Parse(playerNode.SelectSingleNode("Lifes").InnerText);
                player.FlaresNumber = int.Parse(playerNode.SelectSingleNode("Flares").InnerText);
                player.TurboLevel = int.Parse(playerNode.SelectSingleNode("Turbo").InnerText);
                player.IsDrunk = bool.Parse(playerNode.SelectSingleNode("Drunk").InnerText);
                player.ElapsedDrunkSeconds = int.Parse(playerNode.SelectSingleNode("DrunkTime").InnerText);
                XmlNode playerPosition = playerNode.SelectSingleNode("BoardPosition");

                string levelId = node.SelectSingleNode("Level").InnerText;
                level = generateLevel(levelId, player, width, height, difficulty);

                XmlNodeList iceBlockNodes = node.SelectSingleNode("Board").SelectNodes("IceBlock");
                List<KeyValuePair<Point, int>> iceBlocks = new List<KeyValuePair<Point, int>>();
                foreach (XmlNode ice in iceBlockNodes)
                {
                    if (ice.ChildNodes.Count != 3)
                        continue;
                    int x = int.Parse(ice.SelectSingleNode("X").InnerText);
                    int y = int.Parse(ice.SelectSingleNode("Y").InnerText);
                    int frozen = int.Parse(ice.SelectSingleNode("Frozen").InnerText);
                    iceBlocks.Add(new KeyValuePair<Point, int>(new Point(x, y), frozen));
                }

                List<KeyValuePair<Point, string>> boxes = new List<KeyValuePair<Point, string>>();
                if (node.SelectSingleNode("Boxes").ChildNodes.Count > 0)
                {
                    XmlNodeList boxNodes = node.SelectSingleNode("Boxes").SelectNodes("Box");
                    foreach (XmlNode box in boxNodes)
                    {
                        if (box.ChildNodes.Count != 3)
                            continue;
                        int x = int.Parse(box.SelectSingleNode("X").InnerText);
                        int y = int.Parse(box.SelectSingleNode("Y").InnerText);
                        string type = box.SelectSingleNode("BoxType").InnerText;
                        boxes.Add(new KeyValuePair<Point, string>(new Point(x, y), type));
                    }
                }

                List<FlareParser> flares = new List<FlareParser>();
                if (node.SelectSingleNode("Flares").ChildNodes.Count > 0)
                {
                    XmlNodeList flareNodes = node.SelectSingleNode("Flares").SelectNodes("Flare");
                    foreach (XmlNode flare in flareNodes)
                    {
                        if (flare.ChildNodes.Count != 3)
                            continue;
                        float x = float.Parse(flare.SelectSingleNode("X").InnerText);
                        float y = float.Parse(flare.SelectSingleNode("Y").InnerText);
                        float xE = float.Parse(flare.SelectSingleNode("End").SelectSingleNode("X").InnerText);
                        float yE = float.Parse(flare.SelectSingleNode("End").SelectSingleNode("Y").InnerText);
                        flares.Add(new FlareParser(x, y, xE, yE));
                    }
                }

                List<ExplosionParser> explosions = new List<ExplosionParser>();
                if (node.SelectSingleNode("Explosions").ChildNodes.Count > 0)
                {
                    XmlNodeList explosionNodes = node.SelectSingleNode("Explosions").SelectNodes("Explosion");
                    foreach (XmlNode explosion in explosionNodes)
                    {
                        if (explosion.ChildNodes.Count != 5)
                            continue;
                        int x = int.Parse(explosion.SelectSingleNode("X").InnerText);
                        int y = int.Parse(explosion.SelectSingleNode("Y").InnerText);
                        int explosionWidth = int.Parse(explosion.SelectSingleNode("Width").InnerText);
                        int explosionHeight = int.Parse(explosion.SelectSingleNode("Height").InnerText);
                        int explosionLevel = int.Parse(explosion.SelectSingleNode("Level").InnerText);
                        explosions.Add(new ExplosionParser(x, y, explosionWidth, explosionHeight, explosionLevel));
                    }
                }

                List<SpitParser> spits = new List<SpitParser>();
                if (node.SelectSingleNode("Spits").ChildNodes.Count > 0)
                {
                    XmlNodeList spitNodes = node.SelectSingleNode("Spits").SelectNodes("Spit");
                    foreach (XmlNode spit in spitNodes)
                    {
                        if (spit.ChildNodes.Count != 5)
                            continue;
                        float x = float.Parse(spit.SelectSingleNode("X").InnerText);
                        float y = float.Parse(spit.SelectSingleNode("Y").InnerText);
                        float xE = float.Parse(spit.SelectSingleNode("End").SelectSingleNode("X").InnerText);
                        float yE = float.Parse(spit.SelectSingleNode("End").SelectSingleNode("Y").InnerText);
                        int spitBoxes = int.Parse(spit.SelectSingleNode("Boxes").InnerText);
                        int lifes = int.Parse(spit.SelectSingleNode("Lifes").InnerText);
                        spits.Add(new SpitParser(x, y, xE, yE, spitBoxes, lifes));
                    }
                }

                level.clearLists();
                level.GameBoard.setBoard(content, iceBlocks);
                level.GameBoard.setBoxes(content, boxes);
                if (player.Boxes == Player.MAX_BOXES)
                    level.GameBoard.IsTopEnabled = true;

                level.setFlares(content, flares);
                level.setExplosions(content, explosions);
                level.setSpits(content, spits);

                XmlNode predatorsNode = node.SelectSingleNode("Predators");
                switch (levelId)
                {
                    case "OneBoss":
                        {
                            List<SimpleParser> sharks = new List<SimpleParser>();
                            if (predatorsNode.SelectSingleNode("MutantSharks").ChildNodes.Count > 0)
                            {
                                XmlNodeList sharkNodes = predatorsNode.SelectSingleNode("MutantSharks").SelectNodes("MutantShark");
                                foreach (XmlNode shark in sharkNodes)
                                {
                                    if (shark.ChildNodes.Count != 4)
                                        continue;
                                    int x = int.Parse(shark.SelectSingleNode("X").InnerText);
                                    int y = int.Parse(shark.SelectSingleNode("Y").InnerText);
                                    bool isBlinded = bool.Parse(shark.SelectSingleNode("Blinded").InnerText);
                                    int blindedTime = int.Parse(shark.SelectSingleNode("BlindedTime").InnerText);
                                    sharks.Add(new SimpleParser(x, y, isBlinded, blindedTime));
                                }
                                level.setMutantSharks(content, sharks);
                            }
                        }
                        break;
                    case "TwoBoss":
                        {
                            List<SimpleParser> octopus = new List<SimpleParser>();
                            if (predatorsNode.SelectSingleNode("MutantOctopus").ChildNodes.Count > 0)
                            {
                                XmlNodeList octopusNode = predatorsNode.SelectSingleNode("MutantOctopus").SelectNodes("MutantOctopus");
                                foreach (XmlNode o in octopusNode)
                                {
                                    if (o.ChildNodes.Count != 4)
                                        continue;
                                    int x = int.Parse(o.SelectSingleNode("X").InnerText);
                                    int y = int.Parse(o.SelectSingleNode("Y").InnerText);
                                    bool isBlinded = bool.Parse(o.SelectSingleNode("Blinded").InnerText);
                                    int blindedTime = int.Parse(o.SelectSingleNode("BlindedTime").InnerText);
                                    octopus.Add(new SimpleParser(x, y, isBlinded, blindedTime));
                                }
                                level.setMutantOctopus(content, octopus);
                            }
                        }
                        break;
                    case "ThreeBoss":
                        {
                            List<MutantCrabParser> crabs = new List<MutantCrabParser>();
                            if (predatorsNode.SelectSingleNode("MutantCrabs").ChildNodes.Count > 0)
                            {
                                XmlNodeList crabsNode = predatorsNode.SelectSingleNode("MutantCrabs").SelectNodes("MutantCrab");
                                foreach (XmlNode c in crabsNode)
                                {
                                    if (c.ChildNodes.Count != 8)
                                        continue;
                                    int x = int.Parse(c.SelectSingleNode("X").InnerText);
                                    int y = int.Parse(c.SelectSingleNode("Y").InnerText);
                                    bool isBlinded = bool.Parse(c.SelectSingleNode("Blinded").InnerText);
                                    int blindedTime = int.Parse(c.SelectSingleNode("BlindedTime").InnerText);
                                    float stepX = float.Parse(c.SelectSingleNode("Step").SelectSingleNode("X").InnerText);
                                    float stepY = float.Parse(c.SelectSingleNode("Step").SelectSingleNode("Y").InnerText);
                                    float destX = float.Parse(c.SelectSingleNode("Destination").SelectSingleNode("X").InnerText);
                                    float destY = float.Parse(c.SelectSingleNode("Destination").SelectSingleNode("Y").InnerText);
                                    int directX = int.Parse(c.SelectSingleNode("Direction").SelectSingleNode("X").InnerText);
                                    int directY = int.Parse(c.SelectSingleNode("Direction").SelectSingleNode("Y").InnerText);
                                    bool changedDirection = bool.Parse(c.SelectSingleNode("ChangedDirection").InnerText);
                                    crabs.Add(new MutantCrabParser(x, y, isBlinded, blindedTime, stepX, stepY, destX, destY, directX, directY, changedDirection));
                                }
                                level.setMutantCrab(content, crabs);
                            }
                        }
                        break;
                }
                {
                    List<KeyValuePair<SimpleParser, int>> sharks = new List<KeyValuePair<SimpleParser, int>>();
                    if (predatorsNode.SelectSingleNode("Sharks").ChildNodes.Count > 0)
                    {
                        XmlNodeList sharkNodes = predatorsNode.SelectSingleNode("Sharks").SelectNodes("Shark");
                        foreach (XmlNode shark in sharkNodes)
                        {
                            if (shark.ChildNodes.Count != 5)
                                continue;
                            int x = int.Parse(shark.SelectSingleNode("X").InnerText);
                            int y = int.Parse(shark.SelectSingleNode("Y").InnerText);
                            bool isBlinded = bool.Parse(shark.SelectSingleNode("Blinded").InnerText);
                            int blindedTime = int.Parse(shark.SelectSingleNode("BlindedTime").InnerText);
                            int tiles = int.Parse(shark.SelectSingleNode("Tiles").InnerText);
                            sharks.Add(new KeyValuePair<SimpleParser, int>(new SimpleParser(x, y, isBlinded, blindedTime), tiles));
                        }
                        level.setSharks(content, sharks);
                    }
                }
                if (levelId == "Two" || levelId == "Three")
                {
                    List<SimpleParser> octopus = new List<SimpleParser>();
                    if (predatorsNode.SelectSingleNode("Octopus").ChildNodes.Count > 0)
                    {
                        XmlNodeList octopusNodes = predatorsNode.SelectSingleNode("Octopus").SelectNodes("Octopus");
                        foreach (XmlNode o in octopusNodes)
                        {
                            if (o.ChildNodes.Count != 4)
                                continue;
                            int x = int.Parse(o.SelectSingleNode("X").InnerText);
                            int y = int.Parse(o.SelectSingleNode("Y").InnerText);
                            bool isBlinded = bool.Parse(o.SelectSingleNode("Blinded").InnerText);
                            int blindedTime = int.Parse(o.SelectSingleNode("BlindedTime").InnerText);
                            octopus.Add(new SimpleParser(x, y, isBlinded, blindedTime));
                        }
                        level.setOctopus(content, octopus);
                    }

                    if (levelId == "Three")
                    {
                        List<SimpleParser> crabs = new List<SimpleParser>();
                        if (predatorsNode.SelectSingleNode("Crabs").ChildNodes.Count > 0)
                        {
                            XmlNodeList crabsNodes = predatorsNode.SelectSingleNode("Crabs").SelectNodes("Crab");
                            foreach (XmlNode c in crabsNodes)
                            {
                                if (c.ChildNodes.Count != 4)
                                    continue;
                                int x = int.Parse(c.SelectSingleNode("X").InnerText);
                                int y = int.Parse(c.SelectSingleNode("Y").InnerText);
                                bool isBlinded = bool.Parse(c.SelectSingleNode("Blinded").InnerText);
                                int blindedTime = int.Parse(c.SelectSingleNode("BlindedTime").InnerText);
                                crabs.Add(new SimpleParser(x, y, isBlinded, blindedTime));
                            }
                            level.setCrabs(content, crabs);
                        }
                    }
                }

                switch (levelId)
                {
                    case "One":
                        {
                            List<SimpleParser> pirates = new List<SimpleParser>();
                            if (predatorsNode.SelectSingleNode("GreenPirates").ChildNodes.Count > 0)
                            {
                                XmlNodeList piratesNodes = predatorsNode.SelectSingleNode("GreenPirates").SelectNodes("GreenPirate");
                                foreach (XmlNode pirate in piratesNodes)
                                {
                                    if (pirate.ChildNodes.Count != 4)
                                        continue;
                                    int x = int.Parse(pirate.SelectSingleNode("X").InnerText);
                                    int y = int.Parse(pirate.SelectSingleNode("Y").InnerText);
                                    bool isBlinded = bool.Parse(pirate.SelectSingleNode("Blinded").InnerText);
                                    int blindedTime = int.Parse(pirate.SelectSingleNode("BlindedTime").InnerText);
                                    pirates.Add(new SimpleParser(x, y, isBlinded, blindedTime));
                                }
                                level.setPirates(content, pirates, "Green");
                            }
                        }
                        break;
                    case "Two":
                        {
                            List<SimpleParser> pirates = new List<SimpleParser>();
                            if (predatorsNode.SelectSingleNode("RedPirates").ChildNodes.Count > 0)
                            {
                                XmlNodeList piratesNodes = predatorsNode.SelectSingleNode("RedPirates").SelectNodes("RedPirate");
                                foreach (XmlNode pirate in piratesNodes)
                                {
                                    if (pirate.ChildNodes.Count != 4)
                                        continue;
                                    int x = int.Parse(pirate.SelectSingleNode("X").InnerText);
                                    int y = int.Parse(pirate.SelectSingleNode("Y").InnerText);
                                    bool isBlinded = bool.Parse(pirate.SelectSingleNode("Blinded").InnerText);
                                    int blindedTime = int.Parse(pirate.SelectSingleNode("BlindedTime").InnerText);
                                    pirates.Add(new SimpleParser(x, y, isBlinded, blindedTime));
                                }
                                level.setPirates(content, pirates, "Red");
                            }
                        }
                        break;
                    case "Three":
                        {
                            List<SimpleParser> pirates = new List<SimpleParser>();
                            if (predatorsNode.SelectSingleNode("BlackPirates").ChildNodes.Count > 0)
                            {
                                XmlNodeList piratesNodes = predatorsNode.SelectSingleNode("BlackPirates").SelectNodes("BlackPirate");
                                foreach (XmlNode pirate in piratesNodes)
                                {
                                    if (pirate.ChildNodes.Count != 4)
                                        continue;
                                    int x = int.Parse(pirate.SelectSingleNode("X").InnerText);
                                    int y = int.Parse(pirate.SelectSingleNode("Y").InnerText);
                                    bool isBlinded = bool.Parse(pirate.SelectSingleNode("Blinded").InnerText);
                                    int blindedTime = int.Parse(pirate.SelectSingleNode("BlindedTime").InnerText);
                                    pirates.Add(new SimpleParser(x, y, isBlinded, blindedTime));
                                }
                                level.setPirates(content, pirates, "Black");
                            }
                        }
                        break;
                }

                player.BoardPosition = new Microsoft.Xna.Framework.Point(
                    int.Parse(playerPosition.SelectSingleNode("X").InnerText), int.Parse(playerPosition.SelectSingleNode("Y").InnerText));
                player.Position = new Vector2(player.BoardPosition.X * player.ItemSize.Width, player.BoardPosition.Y * player.ItemSize.Height);
                player.setLevel(level);
                player.Points = int.Parse(playerNode.SelectSingleNode("Points").InnerText);
                level.updateContent(content);
                level.updateBlindness();
                return;
            }
        }

        private static Level generateLevel(string levelId, Player player, int width, int height, int difficulty)
        {
            switch (levelId)
            {
                case "One":
                    player.CurrentLevel = 1;
                    return new LevelOne(player, width, height, difficulty);
                    break;
                case "OneBoss":
                    player.CurrentLevel = 1;
                    return new LevelOneBoss(player, width, height, difficulty);
                    break;
                case "Two":
                    player.CurrentLevel = 2;
                    return new LevelTwo(player, width, height, difficulty);
                    break;
                case "TwoBoss":
                    player.CurrentLevel = 2;
                    return new LevelTwoBoss(player, width, height, difficulty);
                    break;
                case "Three":
                    player.CurrentLevel = 3;
                    return new LevelThree(player, width, height, difficulty);
                    break;
                case "ThreeBoss":
                    player.CurrentLevel = 3;
                    return new LevelThreeBoss(player, width, height, difficulty);
                    break;
            }
            return null;
        }

        private static void checkForFile(string file)
        {
            if (!File.Exists(file))
            {
                File.Create(file).Close();
                StreamWriter writer = new StreamWriter(file);
                writer.WriteLine("<Users>\n</Users>");
                writer.Close();
            }
        }
        #endregion

        /// <summary>
        /// Represents best user
        /// </summary>
        private class BestUser
        {
            /// <summary>
            /// Username
            /// </summary>
            public string Username { get; set; }
            /// <summary>
            /// Points
            /// </summary>
            public int Points { get; set; }
        }
    }
}
