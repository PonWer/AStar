using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    /// <summary>
    /// Settings for the pathfinder
    /// </summary>
    public class RunSettings
    {
        public bool DiagonalMovement = false;

        ///Cost
        public double DiagonalMovementCost = Math.Sqrt(1 + 1);

        public double MovementCost = 1.0f;

        ///Increase for faster search but less accurate
        public double PullWeightToTarget = 1.0f;

        ///Which algorithm to use
        public DistanceAlgorithm AlgorithmToDetermineDistance = DistanceAlgorithm.Euclidean;

        public enum DistanceAlgorithm
        {
            //Normal sqrt(pow(A.x - B.x) + pow(A.y - B.Y))
            Euclidean,

            //dX + dY
            Manhattan,

            // MAX(dX,dY)
            Chessboard
        }
    }
}