using System;
using System.Threading;
using AStar;

namespace Launcher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var world = new World(50, 50, SampleMaps.Map50x50);

            var runSettings = new RunSettings
            {
                DiagonalMovement = true,
                AlgorithmToDetermineDistance = RunSettings.DistanceAlgorithm.Euclidean,
                PullWeightToTarget = 1.0f
            };
            var pathfinder = new PathFinder(world, runSettings);

            bool ok;
            do
            {
                Console.Clear();
                pathfinder.PrintMapToConsole(false);
                Console.WriteLine("Input a Start location");
                Console.WriteLine("Enter the X coordinate of START (unsure? use 10)");
                ok = int.TryParse(Console.ReadLine(), out var startX);
                Console.WriteLine("Enter the Y coordinate of START (unsure? use 3)");
                ok &= int.TryParse(Console.ReadLine(), out var startY);

                Console.WriteLine("Enter the X coordinate of TARGET (unsure? use 45)");
                ok &= int.TryParse(Console.ReadLine(), out var targetX);
                Console.WriteLine("Enter the Y coordinate of TARGET (unsure? use 35)");
                ok &= int.TryParse(Console.ReadLine(), out var targetY);

                if (!ok)
                {
                    Console.WriteLine("Invalid chars in entered values, Press any key to retry");
                    _ = Console.ReadKey();
                    continue;
                }

                ok = pathfinder.SetStartAndTarget(startX, startY, targetX, targetY);
            } while (!ok);

            Console.WriteLine("Debug mode to view the process? [Y]/[N])");
            var debug = Console.ReadLine()?.ToUpper() == "Y";
            if (debug)
            {
                Console.WriteLine("Do you want the iteration to be [A]utomatic or [M]anuel?");
                var automatic = Console.ReadLine()?.ToUpper() == "A";
                if (automatic)
                {
                    Console.Clear();
                    while (pathfinder.RunSingleFrame() == PathFinder.RunStatus.InProgress)
                    {
                        Console.SetCursorPosition(0, 0);
                        pathfinder.PrintMapToConsole(false);
                        Thread.Sleep(10);
                    }

                    var result = pathfinder.ResultingPath;
                }
                else
                {
                    Console.WriteLine("Press any key to step once, until done");
                    Console.ReadKey();
                    Console.Clear();
                    while (pathfinder.RunSingleFrame() == PathFinder.RunStatus.InProgress)
                    {
                        Console.SetCursorPosition(0, 0);
                        pathfinder.PrintMapToConsole(false);
                        _ = Console.ReadKey();
                    }

                    var result = pathfinder.ResultingPath;
                }
            }
            else
            {
                pathfinder.RunUntilEnd();
            }

            Console.Clear();
            pathfinder.PrintMapToConsole(true);
        }
    }
}