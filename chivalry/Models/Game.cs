using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chivalry.Models
{
    public class Game : INotifyPropertyChanged
    {
        private string againstUserName;
        private IDictionary<Tuple<int, int>, BoardSpaceState> pieceLocations = new Dictionary<Tuple<int, int>, BoardSpaceState>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string AgainstUserName
        {
            get
            {
                return againstUserName;
            }
            set
            {
                if (againstUserName != value)
                {
                    againstUserName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PieceLocationsChanged;

        public void SetPieceLocation(int row, int col, BoardSpaceState boardState)
        {
            pieceLocations[new Tuple<int, int>(row, col)] = boardState;
            if (PieceLocationsChanged != null)
            {
                PieceLocationsChanged(this, new PropertyChangedEventArgs("PieceLocations"));
            }
        }

        public IEnumerable<KeyValuePair<Tuple<int, int>, BoardSpaceState>> PieceLocations
        {
            get
            {
                return pieceLocations.AsEnumerable();
            }
        }
    }
}
