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
        public double G, H;
        public WorldNode Parent;
        public bool IsWall, HasBeenVisited;

        public WorldNode(int inX, int inY)
        {
            X = inX;
            Y = inY;
            G = double.MaxValue;
        }


        public double GetDistance_Sqrt(WorldNode inTargetNode)
        {
            return Math.Sqrt(Math.Pow((inTargetNode.X - X), 2) + Math.Pow((inTargetNode.Y - Y), 2));
        }
    }
}
