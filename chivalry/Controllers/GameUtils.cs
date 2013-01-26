using chivalry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chivalry.Utils;

namespace chivalry.Controllers
{
    public static class GameUtils
    {
        public static IEnumerable<BoardSpaceState> PiecesJumped(Game game, IEnumerable<Tuple<int, int>> activeMoves)
        {
            return SpacesJumped(game, activeMoves).Select(game.getPieceAt);
        }

        public static IEnumerable<Tuple<int, int>> SpacesJumped(Game game, IEnumerable<Tuple<int, int>> activeMoves)
        {
            if (game.ActiveMoves.Count() == 2 && AreNeighbors(game.ActiveMoves.First(), game.ActiveMoves.Last()))
            {
                return Enumerable.Empty<Tuple<int, int>>();
            }

            return activeMoves
                .Pairwise()
                .Select((moves, dest) => SpaceBetween(moves.Item1, moves.Item2));
        }

        public static Tuple<int, int> SpaceBetween(Tuple<int, int> src, Tuple<int, int> dest)
        {
            Tuple<int, int> result;
            if (!SpaceBetween(src.Item1, src.Item2, dest, out result))
            {
                throw new ArgumentException("src and dest are not within one space of each other");
            }
            return result;
        }

        public static void MovePiece(Game game, Tuple<int, int> src, Tuple<int, int> dest)
        {
            game.SetPieceLocation(dest, game.getPieceAt(src));
            game.SetPieceLocation(src, BoardSpaceState.None);
        }

        public static bool SpaceBetween(int rowIndex, int colIndex, Tuple<int, int> tuple, out Tuple<int, int> result)
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

        // can these be defined on the enum itself?
        public static bool IsFriendly(BoardSpaceState piece)
        {
            return piece == BoardSpaceState.FriendlyPieceTall || piece == BoardSpaceState.FriendlyPieceShort;
        }

        public static bool IsOpponent(BoardSpaceState piece)
        {
            return piece == BoardSpaceState.OpponentPieceShort || piece == BoardSpaceState.OpponentPieceTall;
        }

        public static bool AreNeighbors(Tuple<int, int> src, Tuple<int, int> dest)
        {
            return AreNeighbors(src, dest.Item1, dest.Item2);
        }

        public static bool AreNeighbors(Tuple<int, int> location, int row, int col)
        {
            return Math.Abs(location.Item1 - row) <= 1 && Math.Abs(location.Item2 - col) <= 1;
        }

        public static IEnumerable<Tuple<int, int>> NeighborsOf(Game game, Tuple<int, int> loc)
        {
            // TODO add out of bounds checking
            return new Tuple<int, int>[] 
            {
                new Tuple<int, int>(loc.Item1 + 1, loc.Item2),
                new Tuple<int, int>(loc.Item1 - 1, loc.Item2),
                new Tuple<int, int>(loc.Item1, loc.Item2 + 1),
                new Tuple<int, int>(loc.Item1, loc.Item2 - 1),

                new Tuple<int, int>(loc.Item1 + 1, loc.Item2 + 1),
                new Tuple<int, int>(loc.Item1 - 1, loc.Item2 + 1),
                new Tuple<int, int>(loc.Item1 + 1, loc.Item2 - 1),
                new Tuple<int, int>(loc.Item1 - 1, loc.Item2 - 1),
            };
        }

        public static bool IsJumpableFrom(Game game, Tuple<int, int> src, Tuple<int, int> toJump)
        {
            Tuple<int, int> diff = new Tuple<int, int>(toJump.Item1 - src.Item1, toJump.Item2 - src.Item2);
            Tuple<int, int> toLand = new Tuple<int, int>(src.Item1 + diff.Item1 * 2, src.Item2 + diff.Item2 * 2);
            // TODO add out of bounds checking
            return game.getPieceAt(toLand) == BoardSpaceState.None && game.getPieceAt(toJump) != BoardSpaceState.None;
        }
    }
}
