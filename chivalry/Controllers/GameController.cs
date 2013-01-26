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
        public static void OnBoardSpaceClick(Game game, int rowIndex, int colIndex)
        {
            if (GameValidator.IsValidMove(game, rowIndex, colIndex))
            {
                game.AddActiveMove(new Tuple<int, int>(rowIndex, colIndex));
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
                var opponents =
                    game.ActiveMoves
                        .Pairwise()
                        .Select((moves, dest) => GameUtils.SpaceBetween(moves.Item1, moves.Item2))
                        .Where(loc => GameUtils.IsOpponent(game.getPieceAt(loc)));

                foreach (var opponent in opponents)
                {
                    game.SetPieceLocation(opponent.Item1, opponent.Item2, BoardSpaceState.None);
                }
            }

            GameUtils.MovePiece(game, game.ActiveMoves.First(), game.ActiveMoves.Last());

            game.ClearActiveMoves();

            game.Winner = GameValidator.GameWinner(game);
        }
    }
}
