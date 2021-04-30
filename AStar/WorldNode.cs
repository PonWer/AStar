using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    public class WorldNode
    {
        public int X, Y;
        public double G, H, F;
        public WorldNode Parent;
        public bool IsWall, HasBeenVisited;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inX"></param>
        /// <param name="inY"></param>
        public WorldNode(int inX, int inY)
        {
            X = inX;
            Y = inY;
            G = double.MaxValue;
        }

        public double GetDistance_Euclidean(WorldNode inTargetNode)
        {
            return Math.Sqrt(Math.Pow(inTargetNode.X - X, 2) + Math.Pow(inTargetNode.Y - Y, 2));
        }

        public double GetDistance_Manhattan(WorldNode inTargetNode)
        {
            return Math.Abs(inTargetNode.X - X) + Math.Abs(inTargetNode.Y - Y);
        }

        public double GetDistance_Chessboard(WorldNode inTargetNode)
        {
            return Math.Max(Math.Abs(inTargetNode.X - X),Math.Abs(inTargetNode.Y - Y));
        }
    }
}
