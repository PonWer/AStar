using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    /// <summary>
    /// To create and contain the world setting
    /// </summary>
    public class World
    {
        internal WorldNode[,] Map;
        internal int Width, Height;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inHeight"></param>
        /// <param name="inWidth"></param>
        /// <param name="inMap"></param>
        public World(int inHeight, int inWidth, string inMap)
        {
            inMap = inMap.Replace("[", string.Empty).Replace("]", string.Empty).Replace(",", string.Empty);

            Width = inWidth;
            Height = inHeight;
            Map = new WorldNode[inHeight, inWidth];

            if (inHeight * inWidth != inMap.Length)
            {
                throw new InvalidOperationException("Map string length does NOT match provided height and width");
            }

            if (!inMap.All(x => x == '0' || x == '1'))
            {
                throw new InvalidOperationException(
                    "Map string contains invalid characters, can only contain 0 or 1, however '[', ']', ',' are allowed but will be filtered out");
            }

            for (var y = 0; y < inHeight; y++)
            {
                for (var x = 0; x < inWidth; x++)
                {
                    Map[y, x] = new WorldNode(x, y)
                    {
                        IsWall = inMap.ElementAt(y * inWidth + x) == '1'
                    };
                }
            }
        }

        /// <summary>
        /// Get all non wall nodes neighboring provided node
        /// </summary>
        /// <param name="inNode"></param>
        /// <returns></returns>
        public IEnumerable<WorldNode> GetNonWallNeighBoors(WorldNode inNode)
        {
            var returnList = new List<WorldNode>();

            //Left
            if (inNode.X - 1 >= 0 && !Map[inNode.Y, inNode.X - 1].IsWall)
                returnList.Add(Map[inNode.Y, inNode.X - 1]);

            //Right
            if (inNode.X + 1 < Width && !Map[inNode.Y, inNode.X + 1].IsWall)
                returnList.Add(Map[inNode.Y, inNode.X + 1]);

            //Below
            if (inNode.Y - 1 >= 0 && !Map[inNode.Y - 1, inNode.X].IsWall)
                returnList.Add(Map[inNode.Y - 1, inNode.X]);

            //Above
            if (inNode.Y + 1 < Height && !Map[inNode.Y + 1, inNode.X].IsWall)
                returnList.Add(Map[inNode.Y + 1, inNode.X]);

            return returnList;
        }

        /// <summary>
        /// Get all diagonal non wall nodes neighboring provided node
        /// </summary>
        /// <param name="inNode"></param>
        /// <returns></returns>
        public IEnumerable<WorldNode> GetNonWallDiagonalNeighBoors(WorldNode inNode)
        {
            var returnList = new List<WorldNode>();

            //Bottom Left
            if (inNode.Y - 1 >= 0 && inNode.X - 1 >= 0 &&
                !Map[inNode.Y - 1, inNode.X - 1].IsWall)
                returnList.Add(Map[inNode.Y - 1, inNode.X - 1]);

            //Bottom Right
            if (inNode.Y - 1 >= 0 && inNode.X + 1 < Width &&
                !Map[inNode.Y - 1, inNode.X + 1].IsWall)
                returnList.Add(Map[inNode.Y - 1, inNode.X + 1]);

            //Top Right
            if (inNode.Y + 1 < Height && inNode.X + 1 < Width &&
                !Map[inNode.Y + 1, inNode.X + 1].IsWall)
                returnList.Add(Map[inNode.Y + 1, inNode.X + 1]);

            //Top Left
            if (inNode.Y + 1 < Height && inNode.X - 1 >= 0 &&
                !Map[inNode.Y + 1, inNode.X - 1].IsWall)
                returnList.Add(Map[inNode.Y + 1, inNode.X - 1]);


            return returnList;
        }
    }
}