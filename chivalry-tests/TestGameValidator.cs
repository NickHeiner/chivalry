using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using chivalry.Models;
using chivalry.Controllers;

namespace chivalry_tests
{
    // For some reason I can't figure out how to run tests that aren't in this class. Ugh.
    [TestClass]
    public class TestGameValidator
    {
        [TestMethod]
        public void AddPiece()
        {
            int row = 0;
            int col = 0;
            var piece = BoardSpaceState.FriendlyPieceShort;

            Game game = new Game();
            game.SetPieceLocation(row, col, piece);
            Assert.AreEqual(piece, game.getPieceAt(row, col));
        }

        [TestMethod]
        public void ImplicitNone()
        {
            int row = 0;
            int col = 0;

            Game game = new Game();
            Assert.AreEqual(BoardSpaceState.None, game.getPieceAt(row, col));
        }

        [TestMethod]
        public void NoActiveMoves_SelectFriendlyPiece()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 0, BoardSpaceState.FriendlyPieceShort);
            Assert.IsTrue(GameValidator.isValidMove(game, 0, 0));
        }

        [TestMethod]
        public void NoActiveMoves_SelectNone()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 0, BoardSpaceState.None);
            Assert.IsFalse(GameValidator.isValidMove(game, 0, 0));
        }

        [TestMethod]
        public void NoActiveMoves_SelectOpponent()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 0, BoardSpaceState.OpponentPieceShort);
            Assert.IsFalse(GameValidator.isValidMove(game, 0, 0));
        }

        [TestMethod]
        public void MoveOneSpace()
        {
            Game game = new Game();
            game.AddActiveMove(0, 0);
            Assert.IsTrue(GameValidator.isValidMove(game, 0, 1));
        }

        [TestMethod]
        public void MoveOneSpace_Occupied()
        {
            int rowDest = 1;
            int colDest = 1;

            Game game = new Game();

            game.SetPieceLocation(rowDest, colDest, BoardSpaceState.OpponentPieceShort);

            game.AddActiveMove(0, 0);

            Assert.IsFalse(GameValidator.isValidMove(game, rowDest, colDest));
        }

        [TestMethod]
        public void MoveOneSpace_TooFarAway_NothingToJump()
        {
            int rowDest = 2;
            int colDest = 2;

            Game game = new Game();
            game.AddActiveMove(0, 0);

            Assert.IsFalse(GameValidator.isValidMove(game, rowDest, colDest));
        }

        [TestMethod]
        public void MoveOneSpace_TooFarAway()
        {
            int rowDest = 5;
            int colDest = 7;

            Game game = new Game();
            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceTall);

            Assert.IsFalse(GameValidator.isValidMove(game, rowDest, colDest));
        }

        [TestMethod]
        public void MoveOneSpace_HopFriendly()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceShort);

            Assert.IsTrue(GameValidator.isValidMove(game, 2, 2));
        }

        [TestMethod]
        public void MoveOneSpace_HopOpponent()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.OpponentPieceShort);

            Assert.IsTrue(GameValidator.isValidMove(game, 2, 2));
        }

        [TestMethod]
        public void MoveOneSpace_OnlyHopStraight()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameValidator.isValidMove(game, 2, 1));
        }

        [TestMethod]
        public void HopMultipleFriendly()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(2, 2);
            game.SetPieceLocation(3, 3, BoardSpaceState.FriendlyPieceShort);

            Assert.IsTrue(GameValidator.isValidMove(game, 4, 4));
        }

        [TestMethod]
        public void HopMultipleOpponents()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(2, 2);
            game.SetPieceLocation(3, 3, BoardSpaceState.OpponentPieceTall);

            Assert.IsTrue(GameValidator.isValidMove(game, 4, 4));
        }

        [TestMethod]
        public void HopMixed_WithShort()
        {
            Game game = new Game();

            game.SetPieceLocation(0, 0, BoardSpaceState.FriendlyPieceShort);
            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(2, 2);
            game.SetPieceLocation(3, 3, BoardSpaceState.OpponentPieceTall);

            Assert.IsFalse(GameValidator.isValidMove(game, 4, 4));
        }

        [TestMethod]
        public void HopMixed_WithTall()
        {
            Game game = new Game();

            game.SetPieceLocation(0, 0, BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(2, 2);
            game.SetPieceLocation(3, 3, BoardSpaceState.OpponentPieceTall);

            Assert.IsTrue(GameValidator.isValidMove(game, 4, 4));
        }

        [TestMethod]
        public void MoveIntoFriendlyOccupiedSpace()
        {
            Game game = new Game();
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(0, 0);

            Assert.IsFalse(GameValidator.isValidMove(game, 1, 1));
        }

        [TestMethod]
        public void OnlyMoveOneSpaceWithoutJumping()
        {
            Game game = new Game();
            game.AddActiveMove(0, 0);
            game.AddActiveMove(1, 0);

            Assert.IsFalse(GameValidator.isValidMove(game, 2, 0));
        }

        [TestMethod]
        public void JumpingAfterMovingNormally()
        {
            Game game = new Game();
            game.AddActiveMove(0, 0);
            game.AddActiveMove(1, 0);
            game.SetPieceLocation(2, 0, BoardSpaceState.FriendlyPieceTall);

            Assert.IsFalse(GameValidator.isValidMove(game, 3, 0));
        }

        [TestMethod]
        public void SpaceBetween()
        {
            Assert.AreEqual(new Tuple<int, int>(1, 1), 
                GameValidator.spaceBetween(new Tuple<int, int>(0, 0), new Tuple<int,int>(2, 2)));
        }
    }
}