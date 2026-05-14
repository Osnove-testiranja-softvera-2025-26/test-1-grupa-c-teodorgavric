using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OTS2026_GrupaC.Exceptions;
using OTS2026_GrupaC.Models;

namespace OTS2026_GrupaC.Test
{
    [TestFixture]
    internal class GameTest
    {
        private Game game;

        [SetUp]
        public void SetUp()
        {
            game = new Game(new Location(1, 2, 0), new Location(1, 15, 0));
        }

        // playerLocation: validna, nevalidna
        // beeLocation: validna, nevalidna
        [Test]
        public void Game_InvalidPlayerLocation_ThrowsException()
        {
            Exception ex = Assert.Throws<LocationOutsideOfMapException>((TestDelegate)(() => new Game(new Location(-1, 0, 0), new Location(1, 15, 0))));
            Assert.That(ex.Message, Is.EqualTo("Locations must be valid!"));
        }

        // playerLocation: validna, nevalidna
        // beeLocation: validna, nevalidna
        [Test]
        public void Game_InvalidBeeLocation_ThrowsException()
        {
            Exception ex = Assert.Throws<LocationOutsideOfMapException>((TestDelegate)(() => new Game(new Location(1, 2, 0), new Location(-1, 0, 0))));
            Assert.That(ex.Message, Is.EqualTo("Locations must be valid!"));
        }


        [TestCase(Move.Up, 1, 1, 2, 1, 0, 2)]
        [TestCase(Move.Down, 1, 1, 2, 1, 2, 2)]
        [TestCase(Move.Left, 1, 1, 2, 0, 1, 2)]
        [TestCase(Move.Right, 1, 1, 2, 2, 1, 2)]
        [TestCase(Move.Back, 1, 1, 2, 1, 1, 1)]
        [TestCase(Move.Forward, 1, 1, 2, 1, 1, 3)]
        public void MovePlayer_ValidInput_PlayerMoves(Move move, int x, int y, int z, int ex, int ey, int ez)
        {
            game.Player.Location = new Location(x, y, z); 
            game.MovePlayer(move);
            Assert.That(game.Player.Location, Is.EqualTo(new Location(ex, ey, ez)));
        }


        // location: validna, van mape, MapBarrier, Hive bez pčele, Hive sa pčelom
        [Test]
        public void ValidateLocation_ValidLocation_ReturnsTrue()
        {
            bool result = game.ValidateLocation(new Location(5, 5, 0));
            Assert.That(result, Is.True);
        }

        // location: validna, van mape, MapBarrier, Hive bez pčele, Hive sa pčelom
        [Test]
        public void ValidateLocation_LocationOutsideMap_ReturnsFalse()
        {
            bool result = game.ValidateLocation(new Location(-1, 0, 0));
            Assert.That(result, Is.False);
        }

        // location: validna, van mape, MapBarrier, Hive bez pčele, Hive sa pčelom
        [Test]
        public void ValidateLocation_HiveTileWithoutBee_ReturnsFalse()
        {
            game.Map.AddTile(TileType.Hive, TileContent.Empty, 5, 5, 0);
            game.Player.BeeCollected = false;
            bool result = game.ValidateLocation(new Location(5, 5, 0));
            Assert.That(result, Is.False);
        }

        // location: validna, van mape, MapBarrier, Hive bez pčele, Hive sa pčelom
        [Test]
        public void ValidateLocation_HiveTileWithBee_ReturnsTrue()
        {
            game.Map.AddTile(TileType.Hive, TileContent.Empty, 5, 5, 0);
            game.Player.BeeCollected = true;
            bool result = game.ValidateLocation(new Location(5, 5, 0));
            Assert.That(result, Is.True);
        }

        // location: validna, van mape, MapBarrier, Hive bez pčele, Hive sa pčelom
        [Test]
        public void ValidateLocation_MapBarrier_ReturnsFalse()
        {
            bool result = game.ValidateLocation(new Location(10, 5, 0));
            Assert.That(result, Is.False);
        }


        [Test]
        public void UpdatePlayer_NectarTile_NectarIncreased()
        {
            game.Player.Location = new Location(5, 5, 0);
            game.Player.AmountOfNectar = 0;
            game.Map.Tiles[5, 5, 0].Content = TileContent.Nectar;
            game.UpdatePlayer();
            Assert.That(game.Player.AmountOfNectar, Is.EqualTo(1));
        }

        [Test]
        public void UpdatePlayer_BeeTile_BeeCollectedIsTrue()
        {
            game.Player.Location = new Location(5, 5, 0);
            game.Map.Tiles[5, 5, 0].Content = TileContent.Bee;
            game.UpdatePlayer();
            Assert.That(game.Player.BeeCollected, Is.True);
        }

        [Test]
        public void UpdatePlayer_HiveTile_HoneyJarsIncreased()
        {
            game.Player.Location = new Location(5, 5, 0);
            game.Player.AmountOfNectar = 5;
            game.Map.AddTile(TileType.Hive, TileContent.Empty, 5, 5, 0);
            game.UpdatePlayer();
            Assert.That(game.Player.AmountOfHoneyJars, Is.EqualTo(5));
        }

        [Test]
        public void UpdatePlayer_HiveTile_NectarResetToZero()
        {
            game.Player.Location = new Location(5, 5, 0);
            game.Player.AmountOfNectar = 5;
            game.Map.AddTile(TileType.Hive, TileContent.Empty, 5, 5, 0);
            game.UpdatePlayer();
            Assert.That(game.Player.AmountOfNectar, Is.EqualTo(0));
        }

        [Test]
        public void UpdatePlayer_EmptyTile_NothingChanges()
        {
            game.Player.Location = new Location(5, 5, 0);
            game.Player.AmountOfNectar = 3;
            game.Map.Tiles[5, 5, 0].Content = TileContent.Empty;
            game.UpdatePlayer();
            Assert.That(game.Player.AmountOfNectar, Is.EqualTo(3));
        }

        // AmountOfHoneyJars: [0,5], [6,11], [12+]
        // AmountOfNectar: [0,9], [10+]
        // BeeCollected: yes, no
        [TestCaseSource(typeof(GameTestDataFromFile),"Get_CalculateAchievement_OKInput_SuccessfulCalculation_TestData",
        new object[] { "data_calculate_achievement.txt" })]
            public void CalculateAchievement_OKInput_SuccessfulCalculation(int amountOfHoneyJars, int amountOfNectar, bool beeCollected, Achievement? expectedAchievement)
            {
                game.Player.AmountOfHoneyJars = amountOfHoneyJars;
                game.Player.AmountOfNectar = amountOfNectar;
                game.Player.BeeCollected = beeCollected;
                Achievement actualAchievement = game.CalculateAchievement();
                Assert.That(actualAchievement, Is.EqualTo(expectedAchievement));
            }
    }
}
