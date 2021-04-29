using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("AStar.Test")]

namespace AStar
{
    public class PathFinder
    {

        private readonly World _world;
        private readonly RunSettings _runSettings;

        private WorldNode _start, _target;

        public List<WorldNode> OpenList = new List<WorldNode>();
        public List<WorldNode> ClosedList = new List<WorldNode>();

        public PathFinder(World inWorld, RunSettings inRunSettings)
        {
            _world = inWorld;
            _runSettings = inRunSettings;
        }

        /// <summary>
        /// Set start and target positions
        /// </summary>
        /// <param name="inStartX"></param>
        /// <param name="inStartY"></param>
        /// <param name="inTargetX"></param>
        /// <param name="inTargetY"></param>
        /// <returns>True if valid locations, false if locations are inside a wall or outside dimensions</returns>
        public bool SetStartAndTarget(int inStartX, int inStartY, int inTargetX, int inTargetY)
        {
            CleanLists();

            //Verify X axis
            if (inStartX < 0 || inStartX >= _world.Width) return false;
            if (inTargetX < 0 || inTargetX >= _world.Width) return false;

            //Verify Y axis
            if (inStartY < 0 || inStartY >= _world.Height) return false;
            if (inTargetY < 0 || inTargetY >= _world.Height) return false;

            if (!_world.Map[inStartY, inStartX].IsWall && !_world.Map[inTargetY, inTargetX].IsWall)
            {
                _start = _world.Map[inStartY, inStartX];
                _start.G = 0;
                _target = _world.Map[inTargetY, inTargetX];

                OpenList.Add(_start);

                return true;
            }

            return false;
        }

        private void CleanLists()
        {
            OpenList.Clear();
            ClosedList.Clear();
        }

        public bool RunSingleFrame()
        {
            if (!OpenList.Any())
                return true;
            OpenList = OpenList.OrderBy(x => x.H).ToList();

            var current = OpenList.First();
            OpenList.RemoveAt(0);
            ClosedList.Add(current);
            current.HasBeenVisited = true;

            if (current == _target)
                return true;

            AddNeighborToOpenList(current);

            if (_runSettings.DiagonalMovement)
            {
                AddNeighborToOpenList(current,true);
            }

            return false;
        }

        private void AddNeighborToOpenList(WorldNode current, bool inDiagonal = false)
        {
            var neighbors = inDiagonal ?
                _world.GetDiagonalNeighbors(current) :
                _world.GetNeighBoors(current);

            foreach (var neighbor in neighbors.Where(x => !x.IsWall))
            {
                if (current.G + _runSettings.MovementCost < neighbor.G)
                {
                    UpdateWorldNodeWithNewParent(neighbor, current, inDiagonal);

                    if (!OpenList.Contains(neighbor) && !ClosedList.Contains(neighbor))
                        OpenList.Add(neighbor);
                }
            }
        }


        private void UpdateWorldNodeWithNewParent(WorldNode inNode, WorldNode inParent, bool inDiagonal = false)
        {
            inNode.G = inDiagonal
                ? inParent.G + _runSettings.DiagonalMovementCost
                : inParent.G + _runSettings.MovementCost;

            inNode.H = inNode.GetDistance_Sqrt(_target);
            inNode.Parent = inParent;
        }

        public void RunUntilEnd()
        {
            while (!RunSingleFrame())
            {

            }
        }

        public void PrintMapToConsole()
        {
            var topAndBottom = new StringBuilder();
            for (var x = 0; x < _world.Width; x++)
            {
                topAndBottom.Append('-');
            }
            Console.WriteLine(topAndBottom);

            for (var y = _world.Height - 1; y >= 0; y--)
            {
                var row = new StringBuilder();
                row.Append('|');
                for (var x = 0; x < _world.Width; x++)
                {
                    if (x == _start.X && y == _start.Y)
                        row.Append('S');
                    else if (x == _target.X && y == _target.Y)
                        row.Append('T');
                    else if (_world.Map[y, x].IsWall)
                        row.Append('X');
                    else if (_world.Map[y, x].HasBeenVisited)
                        row.Append('*');
                    else if (_world.Map[y, x].G < double.MaxValue)
                        row.Append('?');
                    else
                    {
                        row.Append(' ');
                    }
                }
                row.Append('|');
                Console.WriteLine(row);
            }

            Console.WriteLine(topAndBottom);
        }
    }
}
