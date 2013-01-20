using chivalry.Models;
using chivalry.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace chivalry.Controllers
{
    public class GameValidator
    {
        // TODO check that move is within bounds
        public static bool isValidMove(Game game, int destRowIndex, int destColIndex)
        {
            var pieceAtMove = game.getPieceAt(destRowIndex, destColIndex);
            if (! game.NoActiveMovesExist() && pieceAtMove != BoardSpaceState.None)
            {
                return false;
            }
            if (game.NoActiveMovesExist())
            {
                return pieceAtMove == BoardSpaceState.FriendlyPieceShort || pieceAtMove == BoardSpaceState.FriendlyPieceTall;
            }
            if (areNeighbors(game.GetMostRecentMove(), destRowIndex, destColIndex))
            {
                return game.ActiveMoves.Count() == 1;
            }
            Tuple<int, int> locationToJump;
            var isJumpable = spaceBetween(destRowIndex, destColIndex, game.GetMostRecentMove(), out locationToJump);
            if (isJumpable)
            {
                if (game.ActiveMoves.Count() > 1 
                    && game.ActiveMoves.Pairwise().All(moves => areNeighbors(moves.Item1, moves.Item2)))
                {
                    return false;
                }

                var piecesJumped = 
                    Enumerable
                        .Concat(game.ActiveMoves, Enumerable.Repeat(new Tuple<int, int>(destRowIndex, destColIndex), 1))
                        .Pairwise()
                        .Select((moves, dest) => spaceBetween(moves.Item1, moves.Item2))
                        .Select(game.getPieceAt);

                return piecesJumped.All(isFriendly) 
                    || piecesJumped.All(isOpponent) 
                    || (game.getPieceAt(game.ActiveMoves.First()) == BoardSpaceState.FriendlyPieceTall
                        && game.getPieceAt(locationToJump) != BoardSpaceState.None);
            }
            return false;
        }

        // can these be defined on the enum itself?
        private static bool isFriendly(BoardSpaceState piece)
        {
            return piece == BoardSpaceState.FriendlyPieceTall || piece == BoardSpaceState.FriendlyPieceShort;
        }

        private static bool isOpponent(BoardSpaceState piece)
        {
            return piece == BoardSpaceState.OpponentPieceShort || piece == BoardSpaceState.OpponentPieceTall;
        }

        // visible for testing
        public static Tuple<int, int> spaceBetween(Tuple<int, int> src, Tuple<int, int> dest)
        {
            Tuple<int, int> result;
            if (!spaceBetween(src.Item1, src.Item2, dest, out result))
            {
                throw new ArgumentException("src");
            }
            return result;
        }

        private static bool spaceBetween(int rowIndex, int colIndex, Tuple<int, int> tuple, out Tuple<int, int> result)
        {
            int rowDist = Math.Abs(rowIndex - tuple.Item1);
            int colDist = Math.Abs(colIndex - tuple.Item2); 
            if (rowDist > 2 || rowDist == 1 || colDist > 2 || colDist == 1)
            {
                result = null;
                return false;
            }
            int rowResult = rowIndex;
            if (rowIndex == tuple.Item1 + 2)
            {
                rowResult = tuple.Item1 + 1;
            }
            else if (rowIndex == tuple.Item1 - 2)
            {
                rowResult = tuple.Item1 - 1;
            }

            int colResult = colIndex;
            if (colIndex == tuple.Item2 + 2)
            {
                colResult = tuple.Item2 + 1;
            }
            else if (colIndex == tuple.Item2 - 2)
            {
                colResult = tuple.Item2 - 1;
            }

            result = new Tuple<int, int>(rowResult, colResult);
            return true;
        }

        private static bool areNeighbors(Tuple<int, int> src, Tuple<int, int> dest)
        {
            return areNeighbors(src, dest.Item1, dest.Item2);
        }

        private static bool areNeighbors(Tuple<int, int> location, int row, int col)
        {
            return Math.Abs(location.Item1 - row) <= 1 && Math.Abs(location.Item2 - col) <= 1;
        }
    }
}
