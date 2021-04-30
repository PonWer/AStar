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
    /// <summary>
    /// Find the path from start to end using World object
    /// </summary>
    public class PathFinder
    {
        private readonly World _world;
        private readonly RunSettings _runSettings;

        private WorldNode _start, _target;
        private List<WorldNode> _openList = new List<WorldNode>();
        public List<WorldNode> ResultingPath { get; private set; }

        public enum RunStatus
        {
            FailedToFindPath = -1,
            InProgress,
            Done
        }

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
        public RunStatus RunSingleFrame()
        {
            if (!_openList.Any())
                return RunStatus.FailedToFindPath;
            _openList = _openList.OrderBy(x => x.F).ToList();

            var current = _openList.First();
            _openList.RemoveAt(0);
            current.HasBeenVisited = true;

            if (current == _target)
            {
                ResultingPath = TraverseBackToStart();
                return RunStatus.Done;
            }

            AddNeighborToOpenList(current);
            if (_runSettings.DiagonalMovement)
                AddNeighborToOpenList(current,true);
            
            return RunStatus.InProgress;
        }

        /// <summary>
        /// Loop until there are no more nodes to traverse or target reached
        /// </summary>
        public void RunUntilEnd()
        {
            while (RunSingleFrame() == RunStatus.InProgress)
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
            var outputBuilder = new StringBuilder();
            for (var y = _world.Height - 1; y >= 0; y--)
            {
                for (var x = 0; x < _world.Width; x++)
                {
                    if (x == _start?.X && y == _start?.Y)
                        outputBuilder.Append('S');
                    else if (x == _target?.X && y == _target?.Y)
                        outputBuilder.Append('T');
                    else if (_world.Map[y, x].IsWall)
                        outputBuilder.Append('X');
                    else if (inOnlyPath && ResultingPath.Contains(_world.Map[y, x]))
                        outputBuilder.Append('P');
                    else if (!inOnlyPath && _world.Map[y, x].HasBeenVisited)
                        outputBuilder.Append('.');
                    else if (!inOnlyPath && _world.Map[y, x].G < double.MaxValue)
                        outputBuilder.Append('?');
                    else
                    {
                        outputBuilder.Append(' ');
                    }
                    //2 chars for each node
                    outputBuilder.Append(' ');
                }
                //New row
                outputBuilder.Append('\n');
            }

            Console.WriteLine(outputBuilder);
        }

        #region Private Methods

        private void AddNeighborToOpenList(WorldNode inCurrent, bool inDiagonal = false)
        {
            var neighbors = inDiagonal ?
                _world.GetNonWallDiagonalNeighBoors(inCurrent) :
                _world.GetNonWallNeighBoors(inCurrent);

            foreach (var neighbor in neighbors)
            {
                var tempG = inCurrent.G + (inDiagonal
                    ? _runSettings.DiagonalMovementCost
                    : _runSettings.MovementCost);

                //Only work on if the G score is better than before
                if (neighbor.G < tempG)
                    continue;


                //Get the distance based on runsettings
                neighbor.H = _runSettings.AlgorithmToDetermineDistance switch
                {
                    RunSettings.DistanceAlgorithm.Euclidean => neighbor.GetDistance_Euclidean(_target),
                    RunSettings.DistanceAlgorithm.Manhattan => neighbor.GetDistance_Manhattan(_target),
                    RunSettings.DistanceAlgorithm.Chessboard => neighbor.GetDistance_Chessboard(_target),
                    _ => throw new ArgumentOutOfRangeException(nameof(_runSettings.AlgorithmToDetermineDistance))
                };

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
            var returnList = new List<WorldNode>();

            //Start end end
            var current = _target;
            while (current != _start)
            {
                returnList.Add(current);
                //Move to its parent
                current = current.Parent;
            }
            
            //Add start
            returnList.Add(current);

            //Reverse so start is first
            returnList.Reverse();

            return returnList;
        }
        #endregion
    }
}
