using chivalry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chivalry.Utils;

namespace chivalry.Controllers
{
    public static class GameController
    {
        public static void OnBoardSpaceClick(Game game, Coord coordClicked)
        {
            if (GameValidator.IsValidMove(game, coordClicked))
            {
                game.AddActiveMove(coordClicked);
            }
        }

        public static void ExecuteMoves(Game game)
        {
            if (game.ActiveMoves.Count() == 0)
            {
                return;
            }

            if (!(game.ActiveMoves.Count() == 2 && GameUtils.AreNeighbors(game.ActiveMoves.First(), game.ActiveMoves.Last())))
            {
                var opponentsLoc =
                    game.ActiveMoves
                        .Pairwise()
                        .Select((moves) => GameUtils.SpaceBetween(moves.Item1, moves.Item2))
                        .Where(loc => GameUtils.IsOpponent(game.GetPieceAt(loc)));

                foreach (var opponentLoc in opponentsLoc)
                {
                    game.CapturePiece(game.GetPieceAt(opponentLoc));
                    game.SetPieceLocation(opponentLoc, BoardSpaceState.None);
                }
            }

            GameUtils.MovePiece(game, game.ActiveMoves.First(), game.ActiveMoves.Last());

            game.ClearActiveMoves();

            game.Winner = GameValidator.GameWinner(game);
        }
    }
}
