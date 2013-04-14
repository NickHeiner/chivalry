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
        public static IEnumerable<BoardSpaceState> PiecesJumped(Game game, IEnumerable<Coord> activeMoves)
        {
            return SpacesJumped(game, activeMoves).Select(game.GetPieceAt);
        }

        public static IEnumerable<Coord> SpacesJumped(Game game, IEnumerable<Coord> activeMoves)
        {
            if (game.ActiveMoves.Count() == 2 && AreNeighbors(game.ActiveMoves.First(), game.ActiveMoves.Last()))
            {
                return Enumerable.Empty<Coord>();
            }

            return activeMoves
                .Pairwise()
                .Select((moves, dest) => SpaceBetween(moves.Item1, moves.Item2));
        }

        public static Coord SpaceBetween(Coord src, Coord dest)
        {
            Coord result;
            if (!SpaceBetween(src, dest, out result))
            {
                throw new ArgumentException("src and dest are not within one space of each other");
            }
            return result;
        }

        public static void MovePiece(Game game, Coord src, Coord dest)
        {
            game.SetPieceLocation(dest, game.GetPieceAt(src));
            game.SetPieceLocation(src, BoardSpaceState.None);
        }

        public static bool SpaceBetween(Coord coord1, Coord coord2, out Coord result)
        {
            int rowDist = Math.Abs(coord1.Row - coord2.Row);
            int colDist = Math.Abs(coord1.Col - coord2.Col);
            if (rowDist > 2 || rowDist == 1 || colDist > 2 || colDist == 1)
            {
                result = null;
                return false;
            }
            int rowResult = coord1.Row;
            if (coord1.Row == coord2.Row + 2)
            {
                rowResult = coord2.Row + 1;
            }
            else if (coord1.Row == coord2.Row - 2)
            {
                rowResult = coord2.Row - 1;
            }

            int colResult = coord1.Col;
            if (coord1.Col == coord2.Col + 2)
            {
                colResult = coord2.Col + 1;
            }
            else if (coord1.Col == coord2.Col - 2)
            {
                colResult = coord2.Col - 1;
            }

            result = new Coord() { Row = rowResult, Col = colResult };
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

        public static bool AreNeighbors(Coord src, Coord dest)
        {
            return AreNeighbors(src, dest.Row, dest.Col);
        }

        public static bool AreNeighbors(Coord location, int row, int col)
        {
            return Math.Abs(location.Row - row) <= 1 && Math.Abs(location.Col - col) <= 1;
        }

        public static IEnumerable<Coord> NeighborsOf(Game game, Coord loc)
        {
            // TODO add out of bounds checking
            return new Coord[] 
            {
                new Coord() { Row = loc.Row + 1, Col = loc.Col},
                new Coord() { Row = loc.Row - 1, Col = loc.Col},
                new Coord() { Row = loc.Row, Col = loc.Col + 1},
                new Coord() { Row = loc.Row, Col = loc.Col - 1},

                new Coord() { Row = loc.Row + 1, Col = loc.Col + 1},
                new Coord() { Row = loc.Row - 1, Col = loc.Col + 1},
                new Coord() { Row = loc.Row + 1, Col = loc.Col - 1},
                new Coord() { Row = loc.Row - 1, Col = loc.Col - 1},
            };
        }

        public static bool IsJumpableFrom(Game game, Coord src, Coord toJump)
        {
            Coord diff = new Coord() { Row = toJump.Row - src.Row, Col = toJump.Col - src.Col };
            Coord toLand = new Coord() { Row = src.Row + diff.Row * 2, Col = src.Col + diff.Col * 2 };
            // TODO add out of bounds checking
            return game.GetPieceAt(toLand) == BoardSpaceState.None && game.GetPieceAt(toJump) != BoardSpaceState.None;
        }

        public static BoardSpace.ArrowDirection ArrowDirectionOfCoords(Coord from, Coord to)
        {
            Coord diff = to - from;

            if (diff.Row < 0 && diff.Col == 0)
            {
                return BoardSpace.ArrowDirection.NORTH;
            }
            if (diff.Row < 0 && diff.Col < 0)
            {
                return BoardSpace.ArrowDirection.NORTHWEST;
            }
            if (diff.Row == 0 && diff.Col < 0)
            {
                return BoardSpace.ArrowDirection.WEST;
            }
            if (diff.Row > 0 && diff.Col < 0)
            {
                return BoardSpace.ArrowDirection.SOUTHWEST;
            }
            if (diff.Row > 0 && diff.Col == 0)
            {
                return BoardSpace.ArrowDirection.SOUTH;
            }
            if (diff.Row > 0 && diff.Col > 0)
            {
                return BoardSpace.ArrowDirection.SOUTHEAST;
            }
            if (diff.Row == 0 && diff.Col > 0)
            {
                return BoardSpace.ArrowDirection.EAST;
            }

            return BoardSpace.ArrowDirection.NORTHEAST;
        }
    }
}
