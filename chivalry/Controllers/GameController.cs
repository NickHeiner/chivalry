using chivalry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chivalry.Utils;

namespace chivalry.Controllers
{
    class GameController
    {
        internal static void onBoardSpaceClick(Game game, int rowIndex, int colIndex)
        {
            if (GameValidator.isValidMove(game, rowIndex, colIndex))
            {
                game.AddActiveMove(new Tuple<int, int>(rowIndex, colIndex));
            }
        }

        internal static void ExecuteMoves(Game game)
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

            GameUtils.MovePiece(game, game.ActiveMoves.First(), game.ActiveMoves.Last());

            game.ClearActiveMoves();
        }
    }
}
