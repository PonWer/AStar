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
        private List<WorldNode> _openList = new List<WorldNode>();
        public List<WorldNode> ResultingPath { get; private set; }

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

                _openList.Add(_start);

                return true;
            }

            return false;
        }

        /// <summary>
        ///  A single update, for debug purposes
        /// </summary>
        /// <returns></returns>
        public bool RunSingleFrame()
        {
            if (!_openList.Any())
                return true;
            _openList = _openList.OrderBy(x => x.F).ToList();

            var current = _openList.First();
            _openList.RemoveAt(0);
            current.HasBeenVisited = true;

            if (current == _target)
            {
                ResultingPath = TraverseBackToStart();
                return true;
            }

            AddNeighborToOpenList(current);

            if (_runSettings.DiagonalMovement)
                AddNeighborToOpenList(current,true);
            
            return false;
        }

        /// <summary>
        /// Loop until there are no more nodes to traverse or target reached
        /// </summary>
        public void RunUntilEnd()
        {
            while (!RunSingleFrame())
            {
                //do stuff?
            }
        }

        /// <summary>
        /// Print map, got lazy and did it in console instead of a fancy drawing tool
        /// </summary>
        /// <param name="inOnlyPath"></param>
        public void PrintMapToConsole(bool inOnlyPath)
        {
            var topAndBottom = new StringBuilder();
            for (var x = 0; x < _world.Width; x++)
            {
                topAndBottom.Append('-');
                topAndBottom.Append('-');
            }
            Console.WriteLine(topAndBottom);

            for (var y = _world.Height - 1; y >= 0; y--)
            {
                var row = new StringBuilder();
                row.Append('|');
                for (var x = 0; x < _world.Width; x++)
                {
                    if (x == _start?.X && y == _start?.Y)
                        row.Append('S');
                    else if (x == _target?.X && y == _target?.Y)
                        row.Append('T');
                    else if (_world.Map[y, x].IsWall)
                        row.Append('X');
                    else if (inOnlyPath && ResultingPath.Contains(_world.Map[y, x]))
                        row.Append('P');
                    else if (!inOnlyPath && _world.Map[y, x].HasBeenVisited)
                        row.Append('.');
                    else if (!inOnlyPath && _world.Map[y, x].G < double.MaxValue)
                        row.Append('?');
                    else
                    {
                        row.Append(' ');
                    }
                    row.Append(' ');
                }
                row.Append('|');
                Console.WriteLine(row);
            }

            Console.WriteLine(topAndBottom);
        }

        #region Private Methods

        private void AddNeighborToOpenList(WorldNode inCurrent, bool inDiagonal = false)
        {
            var neighbors = inDiagonal ?
                _world.GetNonWallDiagonalNeighBoors(inCurrent) :
                _world.GetNonWallNeighBoors(inCurrent);

            foreach (var neighbor in neighbors)
            {
                var tempG = inCurrent.G = inDiagonal
                    ? inCurrent.G + _runSettings.DiagonalMovementCost
                    : inCurrent.G + _runSettings.MovementCost;

                //Only work on if the G score is better than before
                if (neighbor.G < tempG)
                    continue;

                switch (_runSettings.AlgorithmToDetermineDistance)
                {
                    case RunSettings.DistanceAlgorithm.Euclidean:
                        neighbor.H = neighbor.GetDistance_Euclidean(_target);
                        break;
                    case RunSettings.DistanceAlgorithm.Manhattan:
                        neighbor.H = neighbor.GetDistance_Manhattan(_target);
                        break;
                    case RunSettings.DistanceAlgorithm.Chessboard:
                        neighbor.H = neighbor.GetDistance_Chessboard(_target);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_runSettings.AlgorithmToDetermineDistance));
                }

                //Update scores
                neighbor.G = tempG;
                neighbor.F = neighbor.H * _runSettings.PullWeightToTarget + neighbor.G;

                //Set the (new) parent
                neighbor.Parent = inCurrent;

                //Add to open list only if not already present and not closed
                if (!_openList.Contains(neighbor) && !neighbor.HasBeenVisited)
                    _openList.Add(neighbor);
            }
        }

        private List<WorldNode> TraverseBackToStart()
        {
            var current = _target;
            var returnList = new List<WorldNode>();

            while (current != _start)
            {
                returnList.Add(current);
                current = current.Parent;
            }
            returnList.Add(current);

            return returnList;
        }
        #endregion
    }
}
