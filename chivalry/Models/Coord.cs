using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chivalry.Models
{
    public class Coord
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public static Coord operator +(Coord c1, Coord c2) 
        {
            return new Coord() { Row = c1.Row + c2.Row, Col = c1.Col + c2.Col };
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
