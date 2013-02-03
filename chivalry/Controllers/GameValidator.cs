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
        public static bool IsValidMove(Game game, Coord move)
        {
            if (GameWinner(game) != Player.None)
            {
                return false;
            }

            var pieceAtMove = game.GetPieceAt(move);
            if (! game.NoActiveMovesExist && pieceAtMove != BoardSpaceState.None)
            {
                return false;
            }
            if (game.NoActiveMovesExist)
            {
                return pieceAtMove == BoardSpaceState.FriendlyPieceShort || pieceAtMove == BoardSpaceState.FriendlyPieceTall;
            }
            if (GameUtils.AreNeighbors(game.GetMostRecentMove(), move))
            {
                return game.ActiveMoves.Count() == 1;
            }
            Coord locationToJump;
            var isJumpable = GameUtils.SpaceBetween(move, game.GetMostRecentMove(), out locationToJump);
            if (isJumpable)
            {
                if (game.ActiveMoves.Count() > 1
                    && game.ActiveMoves.Pairwise().All(moves => GameUtils.AreNeighbors(moves.Item1, moves.Item2)))
                {
                    return false;
                }

                var piecesJumped = GameUtils.PiecesJumped(game,
                    Enumerable.Concat(game.ActiveMoves,
                                      Enumerable.Repeat(move, 1)));

                return piecesJumped.All(GameUtils.IsFriendly)
                    || piecesJumped.All(GameUtils.IsOpponent) 
                    || (game.GetPieceAt(game.ActiveMoves.First()) == BoardSpaceState.FriendlyPieceTall
                        && game.GetPieceAt(locationToJump) != BoardSpaceState.None);
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
                             && GameUtils.IsOpponent(game.GetPieceAt(neighbor))
                             && ! new HashSet<Coord>(GameUtils.SpacesJumped(game, game.ActiveMoves)).Contains(neighbor))
                         .Count() == 0;
        }

        public static Player GameWinner(Game game)
        {
            var opponentInFriendlyEndzone =
                GameUtils.IsOpponent(game.GetPieceAt(new Coord() { Row = Game.BOARD_ROW_MAX, Col = Game.ENDZONE_COL_1 })) &&
                GameUtils.IsOpponent(game.GetPieceAt(new Coord() { Row = Game.BOARD_ROW_MAX, Col = Game.ENDZONE_COL_2 }));

            var friendlyInOpponentEndzone =
                GameUtils.IsFriendly(game.GetPieceAt(new Coord() { Row = 0, Col = Game.ENDZONE_COL_1 })) &&
                GameUtils.IsFriendly(game.GetPieceAt(new Coord() { Row = 0, Col = Game.ENDZONE_COL_2 }));

            return opponentInFriendlyEndzone ? Player.Opponent :
                   friendlyInOpponentEndzone ? Player.Friendly : Player.None;
        }
    }
}
