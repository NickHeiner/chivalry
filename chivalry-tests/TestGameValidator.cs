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
            Assert.IsTrue(GameValidator.IsValidMove(game, 0, 0));
        }

        [TestMethod]
        public void NoActiveMoves_SelectNone()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 0, BoardSpaceState.None);
            Assert.IsFalse(GameValidator.IsValidMove(game, 0, 0));
        }

        [TestMethod]
        public void NoActiveMoves_SelectOpponent()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 0, BoardSpaceState.OpponentPieceShort);
            Assert.IsFalse(GameValidator.IsValidMove(game, 0, 0));
        }

        [TestMethod]
        public void MoveOneSpace()
        {
            Game game = new Game();
            game.AddActiveMove(0, 0);
            Assert.IsTrue(GameValidator.IsValidMove(game, 0, 1));
        }

        [TestMethod]
        public void MoveOneSpace_Occupied()
        {
            int rowDest = 1;
            int colDest = 1;

            Game game = new Game();

            game.SetPieceLocation(rowDest, colDest, BoardSpaceState.OpponentPieceShort);

            game.AddActiveMove(0, 0);

            Assert.IsFalse(GameValidator.IsValidMove(game, rowDest, colDest));
        }

        [TestMethod]
        public void MoveOneSpace_TooFarAway_NothingToJump()
        {
            int rowDest = 2;
            int colDest = 2;

            Game game = new Game();
            game.AddActiveMove(0, 0);

            Assert.IsFalse(GameValidator.IsValidMove(game, rowDest, colDest));
        }

        [TestMethod]
        public void MoveOneSpace_TooFarAway()
        {
            int rowDest = 5;
            int colDest = 7;

            Game game = new Game();
            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceTall);

            Assert.IsFalse(GameValidator.IsValidMove(game, rowDest, colDest));
        }

        [TestMethod]
        public void MoveOneSpace_HopFriendly()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceShort);

            Assert.IsTrue(GameValidator.IsValidMove(game, 2, 2));
        }

        [TestMethod]
        public void MoveOneSpace_HopOpponent()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.OpponentPieceShort);

            Assert.IsTrue(GameValidator.IsValidMove(game, 2, 2));
        }

        [TestMethod]
        public void MoveOneSpace_OnlyHopStraight()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameValidator.IsValidMove(game, 2, 1));
        }

        [TestMethod]
        public void HopMultipleFriendly()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(2, 2);
            game.SetPieceLocation(3, 3, BoardSpaceState.FriendlyPieceShort);

            Assert.IsTrue(GameValidator.IsValidMove(game, 4, 4));
        }

        [TestMethod]
        public void HopMultipleOpponents()
        {
            Game game = new Game();

            game.AddActiveMove(0, 0);
            game.SetPieceLocation(1, 1, BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(2, 2);
            game.SetPieceLocation(3, 3, BoardSpaceState.OpponentPieceTall);

            Assert.IsTrue(GameValidator.IsValidMove(game, 4, 4));
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

            Assert.IsFalse(GameValidator.IsValidMove(game, 4, 4));
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

            Assert.IsTrue(GameValidator.IsValidMove(game, 4, 4));
        }

        [TestMethod]
        public void MoveIntoFriendlyOccupiedSpace()
        {
            Game game = new Game();
            game.SetPieceLocation(1, 1, BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(0, 0);

            Assert.IsFalse(GameValidator.IsValidMove(game, 1, 1));
        }

        [TestMethod]
        public void OnlyMoveOneSpaceWithoutJumping()
        {
            Game game = new Game();
            game.AddActiveMove(0, 0);
            game.AddActiveMove(1, 0);

            Assert.IsFalse(GameValidator.IsValidMove(game, 2, 0));
        }

        [TestMethod]
        public void JumpingAfterMovingNormally()
        {
            Game game = new Game();
            game.AddActiveMove(0, 0);
            game.AddActiveMove(1, 0);
            game.SetPieceLocation(2, 0, BoardSpaceState.FriendlyPieceTall);

            Assert.IsFalse(GameValidator.IsValidMove(game, 3, 0));
        }

        [TestMethod]
        public void SpaceBetween()
        {
            Assert.AreEqual(new Tuple<int, int>(1, 1), 
                GameUtils.SpaceBetween(new Tuple<int, int>(0, 0), new Tuple<int,int>(2, 2)));
        }

        // TODO the order of `expected` and `actual` are messed up on several of these

        [TestMethod]
        public void ExecuteMoves_Capture_MovesPieceAwayFromStart()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 0, BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(0, 1, BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(0, 0);
            game.AddActiveMove(0, 2);

            GameController.ExecuteMoves(game);

            Assert.AreEqual(game.getPieceAt(0, 0), BoardSpaceState.None);
        }

        [TestMethod]
        public void ExecuteMoves_Capture_RemovesCapturedPiece()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 0, BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(0, 1, BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(0, 0);
            game.AddActiveMove(0, 2);

            GameController.ExecuteMoves(game);

            Assert.AreEqual(BoardSpaceState.None, game.getPieceAt(0, 1));
        }

        [TestMethod]
        public void ExecuteMoves_Capture_MovesPieceToDest()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 0, BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(0, 1, BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(0, 0);
            game.AddActiveMove(0, 2);

            GameController.ExecuteMoves(game);

            Assert.AreEqual(game.getPieceAt(0, 2), BoardSpaceState.FriendlyPieceShort);
        }

        [TestMethod]
        public void ExecuteMoves_HandleSingleMove()
        {
            Game game = new Game();
            game.SetPieceLocation(0, 10, BoardSpaceState.FriendlyPieceShort);
            game.AddActiveMove(0, 10);
            game.AddActiveMove(0, 11);

            GameController.ExecuteMoves(game);

            Assert.AreEqual(BoardSpaceState.None, game.getPieceAt(0, 10));
            Assert.AreEqual(BoardSpaceState.FriendlyPieceShort, game.getPieceAt(0, 11));
        }

        [TestMethod]
        public void IsCompleteCapture_False()
        {
            Game game = new Game();
            game.AddActiveMove(0    , 2);
            game.SetPieceLocation(0, 3, BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(0, 4);
            game.SetPieceLocation(0, 5, BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameValidator.IsCompleteMove(game));
        }

        [TestMethod]
        public void IsCompleteCapture_True()
        {
            Game game = new Game();
            game.AddActiveMove(0, 2);
            game.SetPieceLocation(0, 3, BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(0, 4);
            game.SetPieceLocation(0, 5, BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(0, 6);

            Assert.IsTrue(GameValidator.IsCompleteMove(game));
        }

        [TestMethod]
        public void IsCompleteCapture_SimpleMove()
        {
            Game game = new Game();
            game.AddActiveMove(0, 12);
            game.AddActiveMove(0, 13);

            Assert.IsTrue(GameValidator.IsCompleteMove(game));
        }

        [TestMethod]
        public void NeighborsOf()
        {
            var expected = new Tuple<int, int>[] 
                { 
                    // I know that the unary + and the extra 0 is unnecessary, but the code is easier to read when it lines up
                    new Tuple<int, int>(-1, 00),
                    new Tuple<int, int>(+1, 00),
                    new Tuple<int, int>(00, -1), 
                    new Tuple<int, int>(00, +1),
                    new Tuple<int, int>(-1, -1),
                    new Tuple<int, int>(+1, +1),
                    new Tuple<int, int>(-1, +1),
                    new Tuple<int, int>(+1, -1),
                }.AsEnumerable();

            var actual = GameUtils.NeighborsOf(new Game(), new Tuple<int, int>(0, 0));

            // ugh is this really necessary?
            foreach (var loc in expected)
            {
                Assert.IsTrue(new HashSet<Tuple<int, int>>(actual).Contains(loc));
            }

            foreach (var loc in actual)
            {
                Assert.IsTrue(new HashSet<Tuple<int, int>>(expected).Contains(loc));
            }
        }

        [TestMethod]
        public void IsJumpable()
        {
            Game game = new Game();
            game.SetPieceLocation(10, 10, BoardSpaceState.OpponentPieceShort);
            
            Assert.IsTrue(GameUtils.IsJumpableFrom(game, new Tuple<int, int>(10, 9), new Tuple<int, int>(10, 10)));
        }

        [TestMethod]
        public void IsJumpable_NoPieceToJump()
        {
            Assert.IsFalse(GameUtils.IsJumpableFrom(new Game(), new Tuple<int, int>(10, 9), new Tuple<int, int>(10, 10)));
        }

        [TestMethod]
        public void IsJumpable_LandingSpotBlocked()
        {
            Game game = new Game();
            game.SetPieceLocation(10, 10, BoardSpaceState.OpponentPieceShort);
            game.SetPieceLocation(10, 11, BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameUtils.IsJumpableFrom(game, new Tuple<int, int>(10, 9), new Tuple<int, int>(10, 10)));
        }

        [TestMethod]
        public void IsJumpable_Diagonal_Not()
        {
            Game game = new Game();
            game.SetPieceLocation(10, 10, BoardSpaceState.OpponentPieceShort);
            game.SetPieceLocation(9, 9, BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameUtils.IsJumpableFrom(game, new Tuple<int, int>(11, 11), new Tuple<int, int>(10, 10)));
        }

        [TestMethod]
        // TODO add FLAWLESS VICTORY easter egg
        public void Victory_Opponent()
        {
            Game game = new Game();
            game.SetPieceLocation(10, 0, BoardSpaceState.FriendlyPieceShort); // for game.RowMax
            game.SetPieceLocation(0, Game.ENDZONE_COL_1, BoardSpaceState.OpponentPieceShort);
            game.SetPieceLocation(0, Game.ENDZONE_COL_2, BoardSpaceState.OpponentPieceShort);

            Assert.AreEqual(Player.Opponent, GameValidator.GameWinner(game));
        }

        [TestMethod]
        // TODO add FLAWLESS VICTORY easter egg
        public void Victory_NotTautological()
        {
            Game game = new Game();
            game.SetPieceLocation(16, Game.ENDZONE_COL_1, BoardSpaceState.None);

            Assert.AreEqual(Player.None, GameValidator.GameWinner(game));
        }

        [TestMethod]
        // TODO add FLAWLESS VICTORY easter egg
        public void Victory_Friendly()
        {
            Game game = new Game();
            // is this the right row?
            game.SetPieceLocation(16, Game.ENDZONE_COL_1, BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(16, Game.ENDZONE_COL_2, BoardSpaceState.FriendlyPieceShort);

            Assert.AreEqual(Player.Friendly, GameValidator.GameWinner(game));
        }

        [TestMethod]
        public void ExecuteMoves_SetWinner_Friendly()
        {
            Game game = new Game();
            // is this the right row?
            game.SetPieceLocation(16, Game.ENDZONE_COL_1, BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(15, Game.ENDZONE_COL_2, BoardSpaceState.FriendlyPieceShort);
            game.AddActiveMove(new Tuple<int, int>(15, Game.ENDZONE_COL_2));
            game.AddActiveMove(new Tuple<int, int>(16, Game.ENDZONE_COL_2));

            GameController.ExecuteMoves(game);

            Assert.AreEqual(Player.Friendly, game.Winner);
        }

        [TestMethod]
        public void ExecuteMoves_SetWinner_None()
        {
            Game game = new Game();

            game.SetPieceLocation(2, 2, BoardSpaceState.FriendlyPieceShort);
            game.AddActiveMove(new Tuple<int, int>(2, 2));
            game.AddActiveMove(new Tuple<int, int>(2, 3));

            GameController.ExecuteMoves(game);

            Assert.AreEqual(Player.None, game.Winner);
        }

        [TestMethod]
        public void ExecuteMoves_NoMoves()
        {
            // don't crash
            GameController.ExecuteMoves(new Game());
        }

        [TestMethod]
        public void TupleOfString()
        {
            Assert.AreEqual(Tuple.Create(2, 3), Game.DictionaryJsonConverter.tupleOfString("(2, 3)"));
        }

    }
}