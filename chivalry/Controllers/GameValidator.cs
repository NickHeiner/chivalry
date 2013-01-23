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
            if (! game.NoActiveMovesExist && pieceAtMove != BoardSpaceState.None)
            {
                return false;
            }
            if (game.NoActiveMovesExist)
            {
                return pieceAtMove == BoardSpaceState.FriendlyPieceShort || pieceAtMove == BoardSpaceState.FriendlyPieceTall;
            }
            if (areNeighbors(game.GetMostRecentMove(), destRowIndex, destColIndex))
            {
                return game.ActiveMoves.Count() == 1;
            }
            Tuple<int, int> locationToJump;
            var isJumpable = GameUtils.SpaceBetween(destRowIndex, destColIndex, game.GetMostRecentMove(), out locationToJump);
            if (isJumpable)
            {
                if (game.ActiveMoves.Count() > 1 
                    && game.ActiveMoves.Pairwise().All(moves => areNeighbors(moves.Item1, moves.Item2)))
                {
                    return false;
                }

                var piecesJumped = GameUtils.PiecesJumped(game,
                    Enumerable.Concat(game.ActiveMoves, 
                                      Enumerable.Repeat(new Tuple<int, int>(destRowIndex, destColIndex), 1)));

                return piecesJumped.All(GameUtils.IsFriendly)
                    || piecesJumped.All(GameUtils.IsOpponent) 
                    || (game.getPieceAt(game.ActiveMoves.First()) == BoardSpaceState.FriendlyPieceTall
                        && game.getPieceAt(locationToJump) != BoardSpaceState.None);
            }
            return false;
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
