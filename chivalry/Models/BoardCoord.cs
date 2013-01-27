using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chivalry.Models
{
    // Maybe this whole thing should be one function
    public class BoardCoord
    {
        public enum Transformation
        {
            FLIP,
            NO_FLIP
        }

        private Coord coord;
        private Transformation transformation;

        public BoardCoord(Coord coord, Transformation transformation)
        {
            this.coord = coord;
            this.transformation = transformation;
        }

        public int Row
        {
            get
            {
                return transformation == Transformation.NO_FLIP ? coord.Row : Game.BOARD_ROW_MAX - coord.Row;
            }
        }

        public int Col
        {
            get
            {
                return coord.Col;
            }
        }

        public Coord Coord
        {
            get
            {
                return Coord.Create(Row, Col);
            }
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", Row, Col);
        }

        
    }
}
