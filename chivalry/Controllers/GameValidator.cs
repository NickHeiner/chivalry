using chivalry.Models;
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
        public static bool isValidMove(Game game, int rowIndex, int colIndex)
        {
            var pieceAtMove = game.getPieceAt(rowIndex, colIndex);
            if (pieceAtMove == BoardSpaceState.OpponentPieceShort || pieceAtMove == BoardSpaceState.OpponentPieceTall)
            {
                return false;
            }
            if (game.NoActiveMovesExist())
            {
                return pieceAtMove == BoardSpaceState.FriendlyPieceShort || pieceAtMove == BoardSpaceState.FriendlyPieceTall;
            }
            if (areNeighbors(game.GetMostRecentMove(), rowIndex, colIndex))
            {
                return true;
            }
            return false;
        }

        private static bool areNeighbors(Tuple<int, int> location, int row, int col)
        {
            return Math.Abs(location.Item1 - row) <= 1 && Math.Abs(location.Item2 - col) <= 1;
        }
    }
}
