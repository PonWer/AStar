using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AStar.Test
{
    [TestClass]
    public class PathFinderTest
    {
        private World _world;
        private RunSettings _runSettings;
        private PathFinder _pathfinder;

        [TestInitialize]
        public void Init()
        {
            _world = new World(25, 25, SampleMaps.Map25x25);
            _world.PrintMapToConsole();

            _runSettings = new RunSettings();

            _pathfinder = new PathFinder(_world, _runSettings);
        }


        [TestMethod]
        public void SetStartAndTarget_SetPositionsOutsideMap()
        {
            Assert.IsFalse(_pathfinder.SetStartAndTarget(-1, 0, 0, 0));
        }

        [TestMethod]
        public void SetStartAndTarget_SetPositionsOnWall()
        {
            Assert.IsFalse(_pathfinder.SetStartAndTarget(0, 0, 0, 0));
        }

        [TestMethod]
        public void SetStartAndTarget_SetPositions()
        {
            Assert.IsTrue(_pathfinder.SetStartAndTarget(3, 3, 4, 4));
        }

    }
}
