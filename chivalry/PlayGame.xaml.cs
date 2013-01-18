using chivalry.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace chivalry
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PlayGame : chivalry.Common.LayoutAwarePage
    {
        private const int BOARD_GRID_SIDE_LENGTH = 45;
        private IDictionary<Tuple<int, int>, BoardSpace> boardSpaces = new Dictionary<Tuple<int, int>, BoardSpace>();
        private Game game
        {
            get
            {
                return (Game)DataContext;
            }
        }

        public PlayGame()
        {
            this.InitializeComponent();
            Loaded += PlayGame_Loaded;
        }

        // adapted from http://code.msdn.microsoft.com/windowsapps/Reversi-XAMLC-sample-board-816140fa/sourcecode?fileId=69011&pathId=706708707
        // This is shit code.
        void PlayGame_Loaded(object sender, RoutedEventArgs e)
        {
            var rowsEnd = new int[] { 2, 8, 10, 12 };
            var allRows = Enumerable.Concat(Enumerable.Concat(rowsEnd, Enumerable.Repeat(12, 8)), rowsEnd.Reverse());

            foreach (var row in allRows)
            {
                boardGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(BOARD_GRID_SIDE_LENGTH) });
            }
            var maxColIndex = allRows.Max();
            foreach (var col in Enumerable.Range(0, maxColIndex))
            {
                boardGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(BOARD_GRID_SIDE_LENGTH) });
            }

            foreach (var rowInfo in allRows.Select((colCount, index) => new { ColCount = colCount, Index = index }))
            {
                foreach (var col in Enumerable.Range(0, rowInfo.ColCount))
                {
                    var colOffset = (maxColIndex - rowInfo.ColCount) / 2;
                    var colIndex = col + colOffset;

                    var backgroundColorKey = rowInfo.Index % 2 == 0 ^ colIndex % 2 == 0 ? "PrimaryTileColor" : "SecondaryTileColor";
                    var boardSpace = new BoardSpace(rowInfo.Index, colIndex) { Background = (Brush)App.Current.Resources[backgroundColorKey] };
                    //boardSpace.SetBinding(BoardSpace.SpaceStateProperty,
                    //    new Binding { Path = new PropertyPath(String.Format("[{0},{1}]", row, col)) });
                    //boardSpace.SetBinding(BoardSpace.CommandProperty,
                    //    new Binding { Path = new PropertyPath("MoveCommand") });
                    //boardSpace.CommandParameter = new Space(row, col);
                    Grid.SetRow(boardSpace, rowInfo.Index);
                    Grid.SetColumn(boardSpace, colIndex);
                    boardGrid.Children.Add(boardSpace);
                    boardSpaces[new Tuple<int, int>(rowInfo.Index, colIndex)] = boardSpace;
                }
            }

            updateBoardFromGame();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            DataContext = navigationParameter;
            game.PieceLocationsChanged += PlayGame_PieceLocationsChanged;
        }

        void updateBoardFromGame()
        {
            foreach (var pieceLoc in game.PieceLocations)
            {
                boardSpaces[pieceLoc.Key].SpaceState = pieceLoc.Value;
            }
        }

        void PlayGame_PieceLocationsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            updateBoardFromGame();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
