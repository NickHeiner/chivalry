using chivalry.Common;
using chivalry.Controllers;
using chivalry.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public sealed partial class PlayGame : LayoutAwarePage
    {
        private const int BOARD_GRID_SIDE_LENGTH = 45;
        // TODO this needs to be BoardCoord, not just Coord
        private IDictionary<Coord, BoardSpace> boardSpaces = new Dictionary<Coord, BoardSpace>();
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
            foreach (var row in Game.ALL_ROWS)
            {
                boardGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(BOARD_GRID_SIDE_LENGTH) });
            }
            var maxColIndex = Game.ALL_ROWS.Max();
            foreach (var col in Enumerable.Range(0, maxColIndex))
            {
                boardGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(BOARD_GRID_SIDE_LENGTH) });
            }

            foreach (var rowInfo in Game.ALL_ROWS.Select((colCount, index) => new { ColCount = colCount, Index = index }))
            {
                foreach (var col in Enumerable.Range(0, rowInfo.ColCount))
                {
                    var colOffset = (maxColIndex - rowInfo.ColCount) / 2;
                    var colIndex = col + colOffset;

                    var backgroundColorKey = rowInfo.Index % 2 == 0 ^ colIndex % 2 == 0 ? "PrimaryTileColor" : "SecondaryTileColor";
                    var boardSpace = new BoardSpace() { Background = (Brush)App.Current.Resources[backgroundColorKey] };
                    Grid.SetRow(boardSpace, rowInfo.Index);
                    Grid.SetColumn(boardSpace, colIndex);
                    boardGrid.Children.Add(boardSpace);
                    boardSpaces[Coord.Create(rowInfo.Index, colIndex)] = boardSpace;

                    // TODO this needs to translate from screen space to board space
                    boardSpace.Click += (_, __) => GameController.OnBoardSpaceClick(game, Coord.Create(rowInfo.Index, colIndex)) ;
                }
            }

            updateBoardFromGame();
            updateCapturedPiecesFromGame();
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
            game.PropertyChanged += game_PropertyChanged;
        }

        void game_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ActiveMoves"))
            {
                foreach (var boardSpaceLocation in boardSpaces)
                {
                    if (game.ActiveMoves.Contains(boardSpaceLocation.Key))
                    {
                        boardSpaceLocation.Value.Select();
                    }
                    else
                    {
                        boardSpaceLocation.Value.Unselect();
                    }
                }
            }
            if (e.PropertyName.Equals("CapturedPieces"))
            {
                updateCapturedPiecesFromGame();
            }
        }

        private void updateCapturedPiecesFromGame()
        {
            capturedFriendlyPieces.Children.Clear();
            capturedOpponentPieces.Children.Clear();

            updateCapturedPieces(capturedFriendlyPieces, BoardSpaceState.FriendlyPieceShort);
            updateCapturedPieces(capturedFriendlyPieces, BoardSpaceState.FriendlyPieceTall);
            updateCapturedPieces(capturedOpponentPieces, BoardSpaceState.OpponentPieceShort);
            updateCapturedPieces(capturedOpponentPieces, BoardSpaceState.OpponentPieceTall);
        }

        private void updateCapturedPieces(StackPanel container, BoardSpaceState piece)
        {
            foreach (var _ in Enumerable.Range(0, game.GetCapturedCount(piece)))
            {
                container.Children.Add(new BoardSpace()
                {
                    SpaceState = piece,
                    Width = BOARD_GRID_SIDE_LENGTH,
                    Height = BOARD_GRID_SIDE_LENGTH
                });
            }
        }

        void updateBoardFromGame()
        {
            foreach (var boardSpace in boardSpaces)
            {
                boardSpace.Value.SpaceState = BoardSpaceState.None;
            }
            foreach (var pieceLoc in game.QueryPieceLocations)
            {
                boardSpaces[new BoardCoord(pieceLoc.Key, game.Transformation).Coord].SpaceState = pieceLoc.Value;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            game.ClearActiveMoves();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GameController.ExecuteMoves(game);
        }   
    }
}
