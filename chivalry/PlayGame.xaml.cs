using chivalry.Common;
using chivalry.Controllers;
using chivalry.Models;
using chivalry.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace chivalry
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PlayGame : LayoutAwarePage
    {
        private const int BOARD_GRID_SIDE_LENGTH = 55;
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

            ((App)App.Current).DataManager.UserUpdate += async (s, e) => await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var user = await getUser();
                LoadState(user.Games.Single(g => g.Id == game.Id), null);
                updateFromGame();
            });
        }

        // adapted from http://code.msdn.microsoft.com/windowsapps/Reversi-XAMLC-sample-board-816140fa/sourcecode?fileId=69011&pathId=706708707
        // This is shit code.
        async void PlayGame_Loaded(object sender, RoutedEventArgs e)
        {
            boardViewBox.Height = Window.Current.Bounds.Height * (85d / 90d);
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
                    var user = await getUser();
                    boardSpace.Click += (_, __) => GameController.OnBoardSpaceClick(user, game, new BoardCoord(Coord.Create(rowInfo.Index, colIndex), game.Transformation).Coord);
                }
            }

            updateFromGame();
        }

        // TODO Shouldn't it be possible to do this declaratively,
        // and not have to manually call these update methods?
        private void updateFromGame()
        {
            updateBoardFromGame();
            updateCapturedPiecesFromGame();
            updateGameStatusFromGame();
        }

        private static async System.Threading.Tasks.Task<User> getUser()
        {
            return await ((App)Application.Current).getUser();
        }

        private async void updateGameStatusFromGame()
        {
            gameStatusMessage.Text = GameController.StatusMessageOf(await getUser(), game);
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
                    if (game.ActiveMoves.Select(coord => new BoardCoord(coord, game.Transformation).Coord).Contains(boardSpaceLocation.Key))
                    {   
                        boardSpaceLocation.Value.Select();
                    }
                    else
                    {
                        boardSpaceLocation.Value.Unselect();
                    }
                }
                foreach (var boardSpacePair in game.ActiveMoves.Pairwise().Skip(1))
                {
                    Coord from = boardSpacePair.Item1;

                    BoardSpace fromBoardSpace = boardSpaces.Single(spaceCoord => new BoardCoord(spaceCoord.Key, game.Transformation).Coord == from).Value;

                    fromBoardSpace.SetArrowDirection(GameUtils.ArrowDirectionOfCoords(from, boardSpacePair.Item2));
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

        private async void updateCapturedPieces(StackPanel container, BoardSpaceState piece)
        {
            foreach (var _ in Enumerable.Range(0, game.GetCapturedCount(piece)))
            {
                container.Children.Add(new BoardSpace()
                {
                    SpaceState = GameController.BoardSpaceStateFor(await getUser(), game, piece),
                    Width = BOARD_GRID_SIDE_LENGTH,
                    Height = BOARD_GRID_SIDE_LENGTH
                });
            }
        }

        private async void updateBoardFromGame()
        {
            foreach (var boardSpace in boardSpaces)
            {
                boardSpace.Value.SpaceState = BoardSpaceState.None;
            }
            foreach (var pieceLoc in game.QueryPieceLocations)
            {
                boardSpaces[new BoardCoord(pieceLoc.Key, game.Transformation).Coord].SpaceState = GameController.BoardSpaceStateFor(await getUser(), game, pieceLoc.Value);
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

        private void ResetMoves_Click(object sender, RoutedEventArgs e)
        {
            game.ClearActiveMoves();
        }

        private async void MakeMoves_Click(object sender, RoutedEventArgs e)
        {
            GameController.ExecuteMovesFor(game, await getUser());

            ((App)Application.Current).DataManager.SaveGame(game, await getUser());
        }
    }
}
