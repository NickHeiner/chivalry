using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace chivalry.Models
{
    public class Game : INotifyPropertyChanged
    {
        [IgnoreDataMember]
        public static readonly int ENDZONE_COL_1 = 6;
        [IgnoreDataMember]
        public static readonly int ENDZONE_COL_2 = 7;

        public class PlayerJsonConverter : IDataMemberJsonConverter
        {
            public object ConvertFromJson(IJsonValue value)
            {
                return Enum.Parse(typeof(Player), value.GetString());
            }

            public IJsonValue ConvertToJson(object instance)
            {
                return JsonValue.CreateStringValue(instance.ToString());
            }
        }

        /**
         * All of this serialization could probably be done better,
         * but fuck it I've spent enough time trying to make it work already.
         */
        public class DictionaryJsonConverter : IDataMemberJsonConverter 
        {
            public static Tuple<int, int> tupleOfString(string str)
            {
                var match = Regex.Match(str, @"\((\d+), (\d+)\)");
                // need to grab indexes 1 and 2 because 0 is the entire match
                return Tuple.Create(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            }

            public object ConvertFromJson(IJsonValue val)
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(val.GetString());
                var deserialized = new Dictionary<Tuple<int, int>, BoardSpaceState>();
                foreach (var pieceLoc in dict)
                {
                    deserialized[tupleOfString(pieceLoc.Key)] = (BoardSpaceState) Enum.Parse(typeof(BoardSpaceState), pieceLoc.Value);
                }
                return deserialized;
            }

            public IJsonValue ConvertToJson(object instance)
            {
                var dict = (IDictionary<Tuple<int, int>, BoardSpaceState>)instance;
                IDictionary<Tuple<int, int>, string> toSerialize = new Dictionary<Tuple<int, int>, string>();
                foreach (var pieceLoc in dict)
                {
                    /** There may be an easier way to convert the enums to strings
                     * http://stackoverflow.com/questions/2441290/json-serialization-of-c-sharp-enum-as-string
                     * By default, Json.NET just converts the enum to its numeric value, which is not helpful.
                     * There could also be a way to do these dictionary conversions in a more functional way.
                     */
                    toSerialize[pieceLoc.Key] = pieceLoc.Value.ToString();
                }
                
                var serialized = JsonConvert.SerializeObject(toSerialize);
                return JsonValue.CreateStringValue(serialized);
            }
        }

        // required by Azure
        public int Id { get; set; }

        private string recepientPlayerName;
        public string RecepientPlayerEmail { get; set; }

        public string InitiatingPlayerName { get; set; }
        public string InitiatingPlayerEmail { get; set; }

        private Player winner;

        [DataMemberJsonConverter(ConverterType = typeof(PlayerJsonConverter))]
        public Player Winner 
        {
            get
            {
                return winner;
            }
            set
            {
                if (winner != value)
                {
                    winner = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // public with get; set; for Azure
        //[DataMember(Name = "BoardPieceLocations")]
        [DataMemberJsonConverter(ConverterType = typeof(DictionaryJsonConverter))]
        public IDictionary<Tuple<int, int>, BoardSpaceState> pieceLocations { get; set; }

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

        public int RowMax
        {
            get
            {
                return pieceLocations.Select(kv => kv.Key.Item1).Max();
            }
        }
    }
}
