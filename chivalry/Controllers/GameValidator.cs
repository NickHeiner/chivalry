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
        public static bool IsValidMove(Game game, int destRowIndex, int destColIndex)
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
            if (GameUtils.AreNeighbors(game.GetMostRecentMove(), destRowIndex, destColIndex))
            {
                return game.ActiveMoves.Count() == 1;
            }
            Tuple<int, int> locationToJump;
            var isJumpable = GameUtils.SpaceBetween(destRowIndex, destColIndex, game.GetMostRecentMove(), out locationToJump);
            if (isJumpable)
            {
                if (game.ActiveMoves.Count() > 1
                    && game.ActiveMoves.Pairwise().All(moves => GameUtils.AreNeighbors(moves.Item1, moves.Item2)))
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

        public static bool IsCompleteMove(Game game)
        {
            return 
                GameUtils.PiecesJumped(game, game.ActiveMoves).Count() == 0 ||
                GameUtils.NeighborsOf(game, game.ActiveMoves.Last())
                         .Where(neighbor => 
                                GameUtils.IsJumpableFrom(game, game.ActiveMoves.Last(), neighbor) 
                             && GameUtils.IsOpponent(game.getPieceAt(neighbor))
                             && ! new HashSet<Tuple<int, int>>(GameUtils.SpacesJumped(game, game.ActiveMoves)).Contains(neighbor))
                         .Count() == 0;
        }

        public static Player GameWinner(Game game)
        {
            return 
                GameUtils.IsOpponent(game.getPieceAt(new Tuple<int, int>(0, Game.ENDZONE_COL_1))) &&
                GameUtils.IsOpponent(game.getPieceAt(new Tuple<int, int>(0, Game.ENDZONE_COL_2))) 
                    ? Player.Opponent :
                GameUtils.IsFriendly(game.getPieceAt(new Tuple<int, int>(game.RowMax, Game.ENDZONE_COL_1))) &&
                GameUtils.IsFriendly(game.getPieceAt(new Tuple<int, int>(game.RowMax, Game.ENDZONE_COL_2))) 
                    ? Player.Friendly : Player.None;
        }
    }
}
