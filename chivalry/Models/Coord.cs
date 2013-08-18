using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camelot;

namespace chivalry.Models
{
    public class Coord
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Camelot.GameState.coord asCamelotCoord()
        {
            return new GameState.coord(GameState.row.NewRow(Row), GameState.col.NewCol(Col));
        }

        public static Coord operator +(Coord c1, Coord c2) 
        {
            return new Coord() { Row = c1.Row + c2.Row, Col = c1.Col + c2.Col };
        }

        public static Coord operator -(Coord c1, Coord c2)
        {
            return new Coord() { Row = c1.Row - c2.Row, Col = c1.Col - c2.Col };
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.Col == c2.Col && c1.Row == c2.Row;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }

        // Is this a great shortcut or inviting error?
        public static Coord Create(int row, int col)
        {
            return new Coord() { Row = row, Col = col };
        }

        public override bool Equals(object obj)
        {
            return obj != null 
                && obj.GetType() == GetType() 
                && ((Coord)obj).Row == Row 
                && ((Coord)obj).Col == Col;
        }

        public override int GetHashCode()
        {
            return 13 + Row * Col ;
        }
    }
}
