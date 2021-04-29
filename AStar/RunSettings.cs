using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    public class RunSettings
    {
        public bool DiagonalMovement = false;

        //Cost
        public double DiagonalMovementCost = 1.0f;
        public double MovementCost = 0.5f;

        //
        public double PullWeightToTarget = 2.0f;

        //Which algorithm to use
        public DistanceAlgorithm AlgorithmToDetermineDistance;

        public enum DistanceAlgorithm
        {
            //Normal sqrt(pow(A.x - B.x) + pow(A.y - B.Y))
            Euclidean,

            //nodeDistance
            Manhattan,

            // MAX(dX,dY)
            Chessboard
        }


    }
}
