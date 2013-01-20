﻿using System;
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

        private List<Tuple<int, int>> activeMoveChain = new List<Tuple<int, int>>();

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

        // TODO why does this need to be a separate event?
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

        public BoardSpaceState getPieceAt(int rowIndex, int colIndex)
        {
            BoardSpaceState boardSpaceState;
            return pieceLocations.TryGetValue(new Tuple<int, int>(rowIndex, colIndex), out boardSpaceState) ? boardSpaceState : BoardSpaceState.None ;
        }

        public void ClearActiveMoves()
        {
            activeMoveChain.Clear();
            NotifyPropertyChanged("ActiveMoves");
            NotifyPropertyChanged("ActiveMovesExist");
            NotifyPropertyChanged("NoActiveMovesExist");
        }

        public void AddActiveMove(int row, int col)
        {
            AddActiveMove(new Tuple<int, int>(row, col));
        }

        public void AddActiveMove(Tuple<int, int> tuple)
        {
            activeMoveChain.Add(tuple);
            NotifyPropertyChanged("ActiveMoves");
            NotifyPropertyChanged("ActiveMovesExist");
            NotifyPropertyChanged("NoActiveMovesExist");
        }

        public bool ActiveMovesExist
        {
            get
            {
                return !NoActiveMovesExist;
            }
        }

        public bool NoActiveMovesExist
        {
            get 
            {
                return activeMoveChain.Count == 0;
            }
        }

        internal Tuple<int, int> GetMostRecentMove()
        {
            return activeMoveChain.Last();
        }

        public IEnumerable<Tuple<int, int>> ActiveMoves
        {
            get
            {
                return activeMoveChain.AsEnumerable();
            }
        }

        internal BoardSpaceState getPieceAt(Tuple<int, int> tuple)
        {
            // TODO consider cleaning up how coords are handled
            return getPieceAt(tuple.Item1, tuple.Item2);
        }
    }
}
