using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AStar.Test
{
    [TestClass]
    public class WorldTest
    {
        [TestMethod]
        public void Constructor_InvalidDimensions()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new World(1, 1, "1,0,1,0,1"));
        }

        [TestMethod]
        public void Constructor_InvalidCharacters()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new World(2, 2, "1,2,1,0"));
        }

        [TestMethod]
        public void Constructor_LoadMap50x50()
        {
            var world = new World(50, 50, SampleMaps.Map50x50);

            Assert.AreEqual(50 * 50, world.Map.Length);
            Assert.AreEqual(50, world.Height);
            Assert.AreEqual(50, world.Width);
        }

        [TestMethod]
        public void Constructor_LoadMap25x25()
        {
            var world = new World(25, 25, SampleMaps.Map25x25);

            Assert.AreEqual(25 * 25, world.Map.Length);
            Assert.AreEqual(25, world.Height);
            Assert.AreEqual(25, world.Width);
        }

        [TestMethod]
        public void Constructor_LoadAsymmetricMap()
        {
            var world = new World(2, 3, "1,1,1,1,1,1");

            Assert.AreEqual(2 * 3, world.Map.Length);
            Assert.AreEqual(2, world.Height);
            Assert.AreEqual(3, world.Width);
        }
    }
}