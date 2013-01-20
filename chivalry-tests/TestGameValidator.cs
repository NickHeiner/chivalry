using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using chivalry.Models;
using chivalry.Controllers;

namespace chivalry_tests
{
    [TestClass]
    public class TestGameValidator
    {
        [TestMethod]
        public void NoActiveMoves_SelectFriendlyPiece()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 0, BoardSpaceState.FriendlyPieceShort);
            Assert.IsTrue(GameValidator.isValidMove(game, 0, 0));
        }
    }
}