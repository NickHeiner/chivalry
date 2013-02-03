using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using chivalry.Models;
using chivalry.Controllers;
using chivalry;

namespace chivalry_tests
{
    // For some reason I can't figure out how to run tests that aren't in this class. Ugh.
    [TestClass]
    public class TestGameValidator
    {
        [TestMethod]
        public void AddPiece()
        {
            Coord coord = new Coord() { Row = 1, Col = 0 };
            var piece = BoardSpaceState.FriendlyPieceShort;

            Game game = new Game();
            game.SetPieceLocation(coord, piece);
            Assert.AreEqual(piece, game.GetPieceAt(coord));
        }

        [TestMethod]
        public void ImplicitNone()
        {
            Game game = new Game();
            Assert.AreEqual(BoardSpaceState.None, game.GetPieceAt(new Coord() { Row = 0, Col = 0 }));
        }

        [TestMethod]
        public void NoActiveMoves_SelectFriendlyPiece()
        {
            Coord validMove = new Coord() { Row = 0, Col = 0};

            Game game = new Game();
            game.SetPieceLocation(validMove, BoardSpaceState.FriendlyPieceShort);
            Assert.IsTrue(GameValidator.IsValidMove(game, validMove));
        }

        [TestMethod]
        public void NoActiveMoves_SelectNone()
        {
            Coord coord = new Coord() { Row = 0, Col = -1 };
            Game game = new Game();
            game.SetPieceLocation(coord, BoardSpaceState.None);
            Assert.IsFalse(GameValidator.IsValidMove(game, coord));
        }

        [TestMethod]
        public void NoActiveMoves_SelectOpponent()
        {
            Coord coord = new Coord() { Row = 0, Col = 1 };
            Game game = new Game();
            game.SetPieceLocation(coord, BoardSpaceState.OpponentPieceShort);
            Assert.IsFalse(GameValidator.IsValidMove(game, coord));
        }

        [TestMethod]
        public void MoveOneSpace()
        {
            Coord coord = new Coord() { Row = 1, Col = -1 };
            Game game = new Game();
            game.AddActiveMove(coord);
            Assert.IsTrue(GameValidator.IsValidMove(game, coord + new Coord() { Row = 0, Col = 1 }));
        }

        [TestMethod]
        public void MoveOneSpace_Occupied()
        {
            Game game = new Game();
            
            Coord start = new Coord() { Row = 1, Col = 1 };
            game.AddActiveMove(start);

            Coord occupied = new Coord() { Row = 0, Col = 1 };
            game.SetPieceLocation(occupied, BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameValidator.IsValidMove(game, occupied));
        }

        [TestMethod]
        public void MoveOneSpace_TooFarAway_NothingToJump()
        {
            Coord nothingToJump = new Coord() { Row = 2, Col = 2 };

            Game game = new Game();
            game.AddActiveMove(new Coord() { Row = 0, Col = 0 });

            Assert.IsFalse(GameValidator.IsValidMove(game, nothingToJump));
        }

        [TestMethod]
        public void MoveOneSpace_TooFarAway()
        {
            Coord tooFar = new Coord() { Row = 5, Col = 7 };

            Game game = new Game();
            game.AddActiveMove(new Coord() { Row = 0, Col = 0 });
            game.SetPieceLocation(new Coord() { Row = 0, Col = 0 }, BoardSpaceState.FriendlyPieceTall);

            Assert.IsFalse(GameValidator.IsValidMove(game, tooFar));
        }

        [TestMethod]
        public void MoveOneSpace_HopFriendly()
        {
            Game game = new Game();

            Coord origin = new Coord() { Row = 0, Col = 0 };
            Coord plusOne = new Coord() { Row = 1, Col = 1 };

            game.AddActiveMove(origin);
            game.SetPieceLocation(origin + plusOne, BoardSpaceState.FriendlyPieceShort);

            Assert.IsTrue(GameValidator.IsValidMove(game, origin + plusOne + plusOne));
        }

        [TestMethod]
        public void MoveOneSpace_HopOpponent()
        {
            Game game = new Game();

            Coord origin = new Coord() { Row = 0, Col = 0 };
            Coord plusOne = new Coord() { Row = 1, Col = 1 };

            game.AddActiveMove(origin);
            game.SetPieceLocation(origin + plusOne, BoardSpaceState.OpponentPieceShort);

            Assert.IsTrue(GameValidator.IsValidMove(game, origin + plusOne + plusOne));
        }

        [TestMethod]
        public void MoveOneSpace_OnlyHopStraight()
        {
            Game game = new Game();

            game.AddActiveMove(new Coord() { Row = 0, Col = 0 });
            game.SetPieceLocation(new Coord() { Row = 1, Col = 1 }, BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameValidator.IsValidMove(game, new Coord() { Row = 2, Col = 1 }));
        }

        [TestMethod]
        public void HopMultipleFriendly()
        {
            Game game = new Game();

            game.AddActiveMove(new Coord() { Row = 0, Col = 0 });
            game.SetPieceLocation(new Coord() { Row = 1, Col = 1 }, BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(new Coord() { Row = 2, Col = 2 });
            game.SetPieceLocation(new Coord() { Row = 3, Col = 3 }, BoardSpaceState.FriendlyPieceShort);

            Assert.IsTrue(GameValidator.IsValidMove(game, new Coord() { Row = 4, Col = 4 }));
        }

        [TestMethod]
        public void HopMultipleOpponents()
        {
            Game game = new Game();

            game.AddActiveMove(Coord.Create(0, 0));
            game.SetPieceLocation(Coord.Create(1, 1), BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(Coord.Create(2, 2));
            game.SetPieceLocation(Coord.Create(3, 3), BoardSpaceState.OpponentPieceTall);

            Assert.IsTrue(GameValidator.IsValidMove(game, Coord.Create(4, 4)));
        }

        [TestMethod]
        public void HopMixed_WithShort()
        {
            Game game = new Game();

            game.SetPieceLocation(Coord.Create(0, 0), BoardSpaceState.FriendlyPieceShort);
            game.AddActiveMove(Coord.Create(0, 0));
            game.SetPieceLocation(Coord.Create(1, 1), BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(Coord.Create(2, 2));
            game.SetPieceLocation(Coord.Create(3, 3), BoardSpaceState.OpponentPieceTall);

            Assert.IsFalse(GameValidator.IsValidMove(game, Coord.Create(4, 4)));
        }

        [TestMethod]
        public void HopMixed_WithTall()
        {
            Game game = new Game();

            game.SetPieceLocation(Coord.Create(0, 0), BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(Coord.Create(0, 0));
            game.SetPieceLocation(Coord.Create(1, 1), BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(Coord.Create(2, 2));
            game.SetPieceLocation(Coord.Create(3, 3), BoardSpaceState.OpponentPieceTall);

            Assert.IsTrue(GameValidator.IsValidMove(game, Coord.Create(4, 4)));
        }

        [TestMethod]
        public void MoveIntoFriendlyOccupiedSpace()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(1, 1), BoardSpaceState.FriendlyPieceTall);
            game.AddActiveMove(Coord.Create(0, 0));

            Assert.IsFalse(GameValidator.IsValidMove(game, Coord.Create(1, 1)));
        }

        [TestMethod]
        public void OnlyMoveOneSpaceWithoutJumping()
        {
            Game game = new Game();
            game.AddActiveMove(Coord.Create(0, 0));
            game.AddActiveMove(Coord.Create(1, 0));

            Assert.IsFalse(GameValidator.IsValidMove(game, Coord.Create(2, 0)));
        }

        [TestMethod]
        public void JumpingAfterMovingNormally()
        {
            Game game = new Game();
            game.AddActiveMove(Coord.Create(0, 0));
            game.AddActiveMove(Coord.Create(1, 0));
            game.SetPieceLocation(Coord.Create(2, 0), BoardSpaceState.FriendlyPieceTall);

            Assert.IsFalse(GameValidator.IsValidMove(game, Coord.Create(3, 0)));
        }

        [TestMethod]
        public void SpaceBetween()
        {
            var spaceBetween = GameUtils.SpaceBetween(Coord.Create(0, 0), Coord.Create(2, 2));

            Assert.AreEqual(Coord.Create(1, 1), spaceBetween);
        }

        // TODO the order of `expected` and `actual` are messed up on several of these

        [TestMethod]
        public void ExecuteMoves_Capture_MovesPieceAwayFromStart()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(0, 0), BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(Coord.Create(0, 1), BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(Coord.Create(0, 0));
            game.AddActiveMove(Coord.Create(0, 2));

            GameController.ExecuteMoves(game);

            Assert.AreEqual(game.GetPieceAt(Coord.Create(0, 0)), BoardSpaceState.None);
        }

        [TestMethod]
        public void ExecuteMoves_Capture_RemovesCapturedPiece()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(0, 0), BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(Coord.Create(0, 1), BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(Coord.Create(0, 0));
            game.AddActiveMove(Coord.Create(0, 2));

            GameController.ExecuteMoves(game);

            Assert.AreEqual(BoardSpaceState.None, game.GetPieceAt(Coord.Create(0, 1)));
            Assert.AreEqual(1, game.GetCapturedCount(BoardSpaceState.OpponentPieceShort));
        }

        [TestMethod]
        public void Game_NothingCapturedToStart()
        {
            Game game = new Game();

            Assert.AreEqual(0, game.GetCapturedCount(BoardSpaceState.OpponentPieceShort));
            Assert.AreEqual(0, game.GetCapturedCount(BoardSpaceState.FriendlyPieceShort));
            Assert.AreEqual(0, game.GetCapturedCount(BoardSpaceState.OpponentPieceTall));
            Assert.AreEqual(0, game.GetCapturedCount(BoardSpaceState.FriendlyPieceTall));
        }

        [TestMethod]
        public void ExecuteMoves_Capture_MovesPieceToDest()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(0, 0), BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(Coord.Create(0, 1), BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(Coord.Create(0, 0));
            game.AddActiveMove(Coord.Create(0, 2));

            GameController.ExecuteMoves(game);

            Assert.AreEqual(game.GetPieceAt(Coord.Create(0, 2)), BoardSpaceState.FriendlyPieceShort);
        }

        [TestMethod]
        public void ExecuteMoves_HandleSingleMove()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(0, 10), BoardSpaceState.FriendlyPieceShort);
            game.AddActiveMove(Coord.Create(0, 10));
            game.AddActiveMove(Coord.Create(0, 11));

            GameController.ExecuteMoves(game);

            Assert.AreEqual(BoardSpaceState.None, game.GetPieceAt(Coord.Create(0, 10)));
            Assert.AreEqual(BoardSpaceState.FriendlyPieceShort, game.GetPieceAt(Coord.Create(0, 11)));
        }

        [TestMethod]
        public void IsCompleteCapture_False()
        {
            Game game = new Game();
            game.AddActiveMove(Coord.Create(0, 2));
            game.SetPieceLocation(Coord.Create(0, 3), BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(Coord.Create(0, 4));
            game.SetPieceLocation(Coord.Create(0, 5), BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameValidator.IsCompleteMove(game));
        }

        [TestMethod]
        public void IsCompleteCapture_True()
        {
            Game game = new Game();
            game.AddActiveMove(Coord.Create(0, 2));
            game.SetPieceLocation(Coord.Create(0, 3), BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(Coord.Create(0, 4));
            game.SetPieceLocation(Coord.Create(0, 5), BoardSpaceState.OpponentPieceShort);
            game.AddActiveMove(Coord.Create(0, 6));

            Assert.IsTrue(GameValidator.IsCompleteMove(game));
        }

        [TestMethod]
        public void IsCompleteCapture_SimpleMove()
        {
            Game game = new Game();
            game.AddActiveMove(Coord.Create(0, 12));
            game.AddActiveMove(Coord.Create(0, 13));

            Assert.IsTrue(GameValidator.IsCompleteMove(game));
        }

        [TestMethod]
        public void NeighborsOf()
        {
            var expected = new Coord[] 
                { 
                    // I know that the unary + and the extra 0 is unnecessary, but the code is easier to read when it lines up
                    Coord.Create(-1, 00),
                    Coord.Create(+1, 00),
                    Coord.Create(00, -1), 
                    Coord.Create(00, +1),
                    Coord.Create(-1, -1),
                    Coord.Create(+1, +1),
                    Coord.Create(-1, +1),
                    Coord.Create(+1, -1),
                }.AsEnumerable();

            var actual = GameUtils.NeighborsOf(new Game(), Coord.Create(0, 0));

            // ugh is this really necessary?
            foreach (var loc in expected)
            {
                Assert.IsTrue(new HashSet<Coord>(actual).Contains(loc));
            }

            foreach (var loc in actual)
            {
                Assert.IsTrue(new HashSet<Coord>(expected).Contains(loc));
            }
        }

        [TestMethod]
        public void IsJumpable()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(10, 10), BoardSpaceState.OpponentPieceShort);
            
            Assert.IsTrue(GameUtils.IsJumpableFrom(game, Coord.Create(10, 9), Coord.Create(10, 10)));
        }

        [TestMethod]
        public void IsJumpable_NoPieceToJump()
        {
            Assert.IsFalse(GameUtils.IsJumpableFrom(new Game(), Coord.Create(10, 9), Coord.Create(10, 10)));
        }

        [TestMethod]
        public void IsJumpable_LandingSpotBlocked()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(10, 10), BoardSpaceState.OpponentPieceShort);
            game.SetPieceLocation(Coord.Create(10, 11), BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameUtils.IsJumpableFrom(game, Coord.Create(10, 9), Coord.Create(10, 10)));
        }

        [TestMethod]
        public void IsJumpable_Diagonal_Not()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(10, 10), BoardSpaceState.OpponentPieceShort);
            game.SetPieceLocation(Coord.Create(9, 9), BoardSpaceState.OpponentPieceShort);

            Assert.IsFalse(GameUtils.IsJumpableFrom(game, Coord.Create(11, 11), Coord.Create(10, 10)));
        }

        [TestMethod]
        // TODO add FLAWLESS VICTORY easter egg
        public void Victory_Opponent()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(10, 0), BoardSpaceState.FriendlyPieceShort); // for game.RowMax
            game.SetPieceLocation(Coord.Create(Game.BOARD_ROW_MAX, Game.ENDZONE_COL_1), BoardSpaceState.OpponentPieceShort);
            game.SetPieceLocation(Coord.Create(Game.BOARD_ROW_MAX, Game.ENDZONE_COL_2), BoardSpaceState.OpponentPieceShort);

            Assert.AreEqual(Player.Opponent, GameValidator.GameWinner(game));
        }

        [TestMethod]
        // TODO add FLAWLESS VICTORY easter egg
        public void Victory_NotTautological()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(16, Game.ENDZONE_COL_1), BoardSpaceState.None);

            Assert.AreEqual(Player.None, GameValidator.GameWinner(game));
        }

        [TestMethod]
        // TODO add FLAWLESS VICTORY easter egg
        public void Victory_Friendly()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(0, Game.ENDZONE_COL_1), BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(Coord.Create(0, Game.ENDZONE_COL_2), BoardSpaceState.FriendlyPieceShort);

            Assert.AreEqual(Player.Friendly, GameValidator.GameWinner(game));
        }

        [TestMethod]
        public void ExecuteMoves_SetWinner_Friendly()
        {
            Game game = new Game();
            // is this the right row?
            game.SetPieceLocation(Coord.Create(0, Game.ENDZONE_COL_1), BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(Coord.Create(1, Game.ENDZONE_COL_2), BoardSpaceState.FriendlyPieceShort);

            game.AddActiveMove(Coord.Create(1, Game.ENDZONE_COL_2));
            game.AddActiveMove(Coord.Create(0, Game.ENDZONE_COL_2));

            GameController.ExecuteMoves(game);

            Assert.AreEqual(Player.Friendly, game.Winner);
        }

        [TestMethod]
        public void ExecuteMoves_SetWinner_None()
        {
            Game game = new Game();

            game.SetPieceLocation(Coord.Create(2, 2), BoardSpaceState.FriendlyPieceShort);
            game.AddActiveMove(Coord.Create(2, 2));
            game.AddActiveMove(Coord.Create(2, 3));

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
        public void BoardCoord_NoFlip()
        {
            Coord coord = Coord.Create(6, 3);
            BoardCoord boardCoord = new BoardCoord(coord, BoardCoord.Transformation.NO_FLIP);
            Assert.AreEqual(coord, Coord.Create(boardCoord.Row, boardCoord.Col));
        }

        [TestMethod]
        public void BoardCoord_Flip()
        {
            Coord coord = Coord.Create(5, 3);
            BoardCoord boardCoord = new BoardCoord(coord, BoardCoord.Transformation.FLIP);
            Assert.AreEqual(Coord.Create(10, 3), Coord.Create(boardCoord.Row, boardCoord.Col));
        }

        [TestMethod]
        public void DataManager_WithUserData_Flip()
        {
            Game game = new Game() { InitiatingPlayerEmail = "nth23@cornell.edu", RecepientPlayerEmail = "ket29@cornell.edu" };
            User user = new User() { Email = "ket29@cornell.edu" };
            new DataManager().updateWithUserData(game, user);

            Assert.AreEqual(BoardCoord.Transformation.FLIP, game.Transformation);
        }

        [TestMethod]
        public void DataManager_WithUserData_NoFlip()
        {
            Game game = new Game() { InitiatingPlayerEmail = "nth23@cornell.edu", RecepientPlayerEmail = "ket29@cornell.edu" };
            User user = new User() { Email = "nth23@cornell.edu" };
            new DataManager().updateWithUserData(game, user);

            Assert.AreEqual(BoardCoord.Transformation.NO_FLIP, game.Transformation);
        }

        [TestMethod]
        public void GameValidator_GameWinner_RowMax()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(4, Game.ENDZONE_COL_1), BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(Coord.Create(4, Game.ENDZONE_COL_2), BoardSpaceState.FriendlyPieceShort);
            game.SetPieceLocation(Coord.Create(6, 5), BoardSpaceState.OpponentPieceShort);
            game.SetPieceLocation(Coord.Create(7, 5), BoardSpaceState.OpponentPieceShort);

            Assert.AreEqual(Player.None, GameValidator.GameWinner(game));
        }

        [TestMethod]
        public void Winner_DisablesMoving()
        {
            Game game = new Game();
            game.SetPieceLocation(Coord.Create(Game.BOARD_ROW_MAX, Game.ENDZONE_COL_1), BoardSpaceState.OpponentPieceShort);
            game.SetPieceLocation(Coord.Create(Game.BOARD_ROW_MAX, Game.ENDZONE_COL_2), BoardSpaceState.OpponentPieceTall);
            game.SetPieceLocation(Coord.Create(4, 5), BoardSpaceState.OpponentPieceTall);

            game.AddActiveMove(Coord.Create(4, 5));

            Assert.IsFalse(GameValidator.IsValidMove(game, Coord.Create(4, 6)));
        }

        [TestMethod]
        public void TupleOfString()
        {
            Assert.AreEqual(Tuple.Create(2, 3), Game.DictionaryJsonConverter.tupleOfString("(2, 3)"));
        }

    }
}