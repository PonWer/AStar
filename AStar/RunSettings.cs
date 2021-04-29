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
        public double DiagonalMovementCost = 1.5f;
        public double MovementCost = 1.0f;
    }
}
