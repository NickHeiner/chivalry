using chivalry.Controllers;
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
using Windows.Globalization.DateTimeFormatting;
using Camelot;

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
        public static readonly int ENDZONE_COL_1 = 5;
        [IgnoreDataMember]
        public static readonly int ENDZONE_COL_2 = 6;

        [IgnoreDataMember]
        public BoardCoord.Transformation Transformation { get; set; }

        // TODO there are a couple of the Initiator* and Recepient* fields. Perhaps there should be a GamePlayer object to factor those out.
        public string InitiatorChannelId { get; set; }
        public string RecepientChannelId { get; set; }

        // public for Azure
        // TODO this is broken because the DictionaryJsonConverter expects a different type for the dict
        [DataMemberJsonConverter(ConverterType = typeof(BoardSpaceStateIntDictConverter))]
        public Dictionary<BoardSpaceState, int> capturedPieces = new Dictionary<BoardSpaceState, int>();

        public void CapturePiece(BoardSpaceState piece)
        {
            if (!capturedPieces.ContainsKey(piece))
            {
                capturedPieces[piece] = 0;
            }
            capturedPieces[piece] += 1;
            NotifyPropertyChanged("CapturedPieces");
        }

        public class BoardSpaceStateIntDictConverter : IDataMemberJsonConverter
        {
            public object ConvertFromJson(IJsonValue val)
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(val.GetString());
                var deserialized = new Dictionary<BoardSpaceState, int>();
                foreach (var captureCount in dict)
                {
                    deserialized[(BoardSpaceState)Enum.Parse(typeof(BoardSpaceState), captureCount.Key)] = captureCount.Value;
                }
                return deserialized;
            }

            public IJsonValue ConvertToJson(object instance)
            {
                var dict = (IDictionary<BoardSpaceState, int>)instance;
                var toSerialize = new Dictionary<string, int>();
                foreach (var captureCount in dict)
                {
                    toSerialize[captureCount.Key.ToString()] = captureCount.Value;
                }

                var serialized = JsonConvert.SerializeObject(toSerialize);
                return JsonValue.CreateStringValue(serialized);
            }
        }


        public int GetCapturedCount(BoardSpaceState piece)
        {
            // ugh I wish I had a DefaultDict
            return capturedPieces.ContainsKey(piece) ? capturedPieces[piece] : 0;
        }

        /**
         * All of this serialization could probably be done better,
         * but fuck it I've spent enough time trying to make it work already.
         */
        public class CoordBoardSpaceStateDictConverter : IDataMemberJsonConverter 
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
                var deserialized = new Dictionary<Coord, BoardSpaceState>();
                foreach (var pieceLoc in dict)
                {
                    var tuple = tupleOfString(pieceLoc.Key);
                    deserialized[Coord.Create(tuple.Item1, tuple.Item2)] = (BoardSpaceState)Enum.Parse(typeof(BoardSpaceState), pieceLoc.Value);
                }
                return deserialized;
            }
                
            public IJsonValue ConvertToJson(object instance)
            {
                var dict = (IDictionary<Coord, BoardSpaceState>)instance;
                var toSerialize = new Dictionary<Tuple<int, int>, string>();
                foreach (var pieceLoc in dict)
                {
                    /** There may be an easier way to convert the enums to strings
                     * http://stackoverflow.com/questions/2441290/json-serialization-of-c-sharp-enum-as-string
                     * By default, Json.NET just converts the enum to its numeric value, which is not helpful.
                     * There could also be a way to do these dictionary conversions in a more functional way.
                     */
                    toSerialize[Tuple.Create(pieceLoc.Key.Row, pieceLoc.Key.Col)] = pieceLoc.Value.ToString();
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

        public string InitiaitingPlayerPicSource { get; set; }
        public string RecepientPlayerPicSource { get; set; }

        public class EnumStringConverter<T> : IDataMemberJsonConverter
        {
            public object ConvertFromJson(IJsonValue value)
            {
 	            return (T) Enum.Parse(typeof(T), value.GetString());
            }

            public IJsonValue ConvertToJson(object instance)
            {
 	            return JsonValue.CreateStringValue(instance.ToString());
            }
        }

        [DataMemberJsonConverter(ConverterType = typeof(EnumStringConverter<RelativePlayer>))]
        private RelativePlayer winner;

        [DataMemberJsonConverter(ConverterType = typeof(EnumStringConverter<AbsolutePlayer>))]
        public AbsolutePlayer WaitingOn { get; set; }

        [DataMemberJsonConverter(ConverterType = typeof(EnumStringConverter<RelativePlayer>))]
        public RelativePlayer Winner 
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

        public DateTime LastMoveSubmittedAt { get; set; }

        // TODO refactor this into a ViewModel
        public string LastMoveSubmittedAtLabel
        {
            get
            {
                return GameController.LastMoveSubmittedAtLabel(LastMoveSubmittedAt);
            }
        }

        // TODO shameful
        [IgnoreDataMember]
        public string OtherPlayerName { get; set; }
        [IgnoreDataMember]
        public string OtherPlayerPicSource { get; set; }
        [IgnoreDataMember]
        public string ThisPlayerPicSource { get; set; }

        // public with get; set; for Azure
        [DataMemberJsonConverter(ConverterType = typeof(CoordBoardSpaceStateDictConverter))]
        public Dictionary<Coord, BoardSpaceState> pieceLocations { get; set; }

        private List<Coord> activeMoveChain = new List<Coord>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Game()
        {
            pieceLocations = new Dictionary<Coord, BoardSpaceState>();
            winner = RelativePlayer.None;
            InitiatorChannelId = RecepientChannelId = "";
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

        public GameState.game asCamelotGame()
        {
            return new GameState.game();
        }
    }
}
