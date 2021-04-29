using System;
using System.Threading;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var world = new AStar.World(50, 50, AStar.SampleMaps.Map50x50);

            var runSettings = new AStar.RunSettings()
            {
                DiagonalMovement = true
            };

            var pathfinder = new AStar.PathFinder(world, runSettings);
            var ok = pathfinder.SetStartAndTarget(3, 3, 48, 35);

            //pathfinder.RunUntilEnd();
            //pathfinder.PrintMapToConsole();

            while (!pathfinder.RunSingleFrame())
            {
                Console.Clear();
                pathfinder.PrintMapToConsole();

                Console.ReadKey();
            }


        }
    }
}
