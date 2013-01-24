using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace chivalry.Models
{
    public class Game : INotifyPropertyChanged
    {
        public class IDictionaryJsonConverter : IDataMemberJsonConverter 
        {
            public object ConvertFromJson(IJsonValue val)
            {
                //var stringified = val.Stringify();
                //var deserialized = JsonConvert.DeserializeObject(stringified);
                //return deserialized;

                //var dict = new Dictionary<Tuple<int, int>, BoardSpaceState>();

                //foreach (var loc in val.GetObject())
                //{
                //    dict[(Tuple<int, int>)JsonConvert.DeserializeObject(loc.Key)] = (BoardSpaceState)JsonConvert.DeserializeObject(loc.Value.GetString());
                //}

                //return val.GetObject();

                return JsonConvert.DeserializeObject(val.GetString());
            }

            public IJsonValue ConvertToJson(object instance)
            {
                return JsonValue.CreateStringValue(JsonConvert.SerializeObject(instance));

                //var dict = (IDictionary<Tuple<int, int>, BoardSpaceState>)instance;

                //var serialized = JsonConvert.SerializeObject(dict);
                //var jval = new JsonObject(serialized);
                //return jval;

                //var converted = new JsonObject();
                //foreach (var loc in dict)
                //{
                //    converted[JsonConvert.SerializeObject(loc.Key)] = JsonValue.CreateStringValue(JsonConvert.SerializeObject(loc.Value));
                //}
                //return converted;

                //JsonValue.
            }
        }

        // required by Azure
        public int Id { get; set; }

        private string recepientPlayerName;
        public string RecepientPlayerEmail { get; set; }

        public string InitiatingPlayerName { get; set; }
        public string InitiatingPlayerEmail { get; set; }
        

        // public with get; set; for Azure
        //[DataMember(Name = "BoardPieceLocations")]
        //[DataMemberJsonConverterAttribute(ConverterType = typeof(IDictionaryJsonConverter))]
        private IDictionary<Tuple<int, int>, BoardSpaceState> pieceLocations { get; set; }

        private List<Tuple<int, int>> activeMoveChain = new List<Tuple<int, int>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Game()
        {
            pieceLocations = new Dictionary<Tuple<int, int>, BoardSpaceState>();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [DataMember(Name = "RecepientPlayerName")]
        public string RecepientPlayerName
        {
            get
            {
                return recepientPlayerName;
            }
            set
            {
                if (recepientPlayerName != value)
                {
                    recepientPlayerName = value;
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
                PieceLocationsChanged(this, new PropertyChangedEventArgs("QueryPieceLocations"));
            }
        }

        [IgnoreDataMember]
        public IEnumerable<KeyValuePair<Tuple<int, int>, BoardSpaceState>> QueryPieceLocations
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

        internal void SetPieceLocation(Tuple<int, int> tuple, BoardSpaceState boardSpaceState)
        {
            SetPieceLocation(tuple.Item1, tuple.Item2, boardSpaceState);
        }
    }
}
