﻿using chivalry.Models;
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
                boardGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });
            }
            foreach (var col in Enumerable.Range(0, allRows.Max()))
            {
                boardGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });
            }

            foreach (var rowInfo in allRows.Select((colCount, index) => new { ColCount = colCount, Index = index }))
            {
                foreach (var col in Enumerable.Range(0, rowInfo.ColCount))
                {
                    var backgroundColorKey = rowInfo.Index % 2 == 0 ^ col % 2 == 0 ? "PrimaryTileColor" : "SecondaryTileColor";
                    var boardSpace = new BoardSpace() { Background = (Brush)App.Current.Resources[backgroundColorKey] };
                    //boardSpace.SetBinding(BoardSpace.SpaceStateProperty,
                    //    new Binding { Path = new PropertyPath(String.Format("[{0},{1}]", row, col)) });
                    //boardSpace.SetBinding(BoardSpace.CommandProperty,
                    //    new Binding { Path = new PropertyPath("MoveCommand") });
                    //boardSpace.CommandParameter = new Space(row, col);
                    Grid.SetRow(boardSpace, rowInfo.Index);
                    Grid.SetColumn(boardSpace, col);
                    boardGrid.Children.Add(boardSpace);
                }
            }
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
