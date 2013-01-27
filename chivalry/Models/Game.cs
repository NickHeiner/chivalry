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
        private static readonly int[] ROWS_END = new int[] { 2, 8, 10, 12 };
        [IgnoreDataMember]
        public static readonly IEnumerable<int> ALL_ROWS = Enumerable.Concat(Enumerable.Concat(ROWS_END, Enumerable.Repeat(12, 8)), ROWS_END.Reverse());

        [IgnoreDataMember]
        public static readonly int BOARD_ROW_MAX = 15;
        [IgnoreDataMember]
        public static readonly int BOARD_COL_MAX = 15;

        [IgnoreDataMember]
        public static readonly int ENDZONE_COL_1 = 6;
        [IgnoreDataMember]
        public static readonly int ENDZONE_COL_2 = 7;

        [IgnoreDataMember]
        public BoardCoord.Transformation Transformation { get; set; }

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
            public object ConvertFromJson(IJsonValue val)
            {
                //var dict = JsonConvert.DeserializeObject<Dictionary<Coord, string>>(val.GetString());
                //var deserialized = new Dictionary<Coord, BoardSpaceState>();
                //foreach (var pieceLoc in dict)
                //{
                //    deserialized[pieceLoc.Key] = (BoardSpaceState) Enum.Parse(typeof(BoardSpaceState), pieceLoc.Value);
                //}
                //return deserialized;
                return new Dictionary<Coord, BoardSpaceState>();
            }

            public IJsonValue ConvertToJson(object instance)
            {
                var dict = (IDictionary<Coord, BoardSpaceState>)instance;
                IDictionary<Coord, string> toSerialize = new Dictionary<Coord, string>();
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
        [IgnoreDataMember]
        //[DataMember(Name = "BoardPieceLocations")]
        [DataMemberJsonConverter(ConverterType = typeof(DictionaryJsonConverter))]
        public IDictionary<Coord, BoardSpaceState> pieceLocations { get; set; }

        private List<Coord> activeMoveChain = new List<Coord>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Game()
        {
            pieceLocations = new Dictionary<Coord, BoardSpaceState>();
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

        public void SetPieceLocation(Coord coord, BoardSpaceState boardState)
        {
            pieceLocations[coord] = boardState;
            if (PieceLocationsChanged != null)
            {
                PieceLocationsChanged(this, new PropertyChangedEventArgs("QueryPieceLocations"));
            }
        }

        [IgnoreDataMember]
        public IEnumerable<KeyValuePair<Coord, BoardSpaceState>> QueryPieceLocations
        {
            get
            {
                return pieceLocations.AsEnumerable();
            }
        }

        public BoardSpaceState GetPieceAt(Coord coord)
        {
            BoardSpaceState boardSpaceState;
            return pieceLocations.TryGetValue(coord, out boardSpaceState) ? boardSpaceState : BoardSpaceState.None ;
        }

        public void ClearActiveMoves()
        {
            activeMoveChain.Clear();
            NotifyPropertyChanged("ActiveMoves");
            NotifyPropertyChanged("ActiveMovesExist");
            NotifyPropertyChanged("NoActiveMovesExist");
        }

        //public void AddActiveMove(int row, int col)
        //{
        //    AddActiveMove(new Tuple<int, int>(row, col));
        //}

        public void AddActiveMove(Coord coord)
        {
            activeMoveChain.Add(coord);
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

        internal Coord GetMostRecentMove()
        {
            return activeMoveChain.Last();
        }

        public IEnumerable<Coord> ActiveMoves
        {
            get
            {
                return activeMoveChain.AsEnumerable();
            }
        }

        public int RowMax
        {
            get
            {
                return pieceLocations.Select(kv => kv.Key.Row).Max();
            }
        }
    }
}
