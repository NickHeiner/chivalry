using chivalry.Controllers;
using chivalry.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace chivalry
{
    // flagrantly copied from http://code.msdn.microsoft.com/windowsapps/Reversi-XAMLC-sample-board-816140fa/sourcecode?fileId=69011&pathId=430921794

    /// <summary> 
    /// Represents the UI for a single space on the game board.  
    /// </summary> 
    public class BoardSpace : Button
    {
        private int row;
        private int col;

        /// <summary> 
        /// Initializes a new instance of the BoardSpace class. 
        /// </summary> 
        public BoardSpace(int row, int col)
        {
            this.row = row;
            this.col = col;

            DefaultStyleKey = typeof(BoardSpace);
        }

        internal void Unselect()
        {
            VisualStateManager.GoToState(this, "Unselected", true);
        }

        internal void Select()
        {
            VisualStateManager.GoToState(this, "Selected", true);
        }

        /// <summary> 
        /// Updates the visual state of the space based on the initial binding values. 
        /// </summary> 
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateSpaceState(false);
        }

        /// <summary> 
        /// Gets or sets the visual state of the space. 
        /// </summary> 
        public BoardSpaceState SpaceState
        {
            get { return (BoardSpaceState)GetValue(SpaceStateProperty); }
            set { SetValue(SpaceStateProperty, value); }
        }

        /// <summary> 
        /// Identifier for the SpaceState dependency property. 
        /// </summary> 
        public static readonly DependencyProperty SpaceStateProperty =
            DependencyProperty.Register("SpaceState",
            typeof(BoardSpaceState), typeof(BoardSpace),
            new PropertyMetadata(BoardSpaceState.None, SpaceStateChanged));

        /// <summary> 
        /// Updates the visual state of the space to match the changed SpaceState value. 
        /// </summary> 
        /// <param name="d">The source of the property change.</param> 
        /// <param name="e">Details about the property change.</param> 
        private static void SpaceStateChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            (d as BoardSpace).UpdateSpaceState(true);
        }

        /// <summary> 
        /// Updates the visual state of the space, optionally using animated transitions. 
        /// </summary> 
        /// <param name="useTransitions"></param> 
        private void UpdateSpaceState(bool useTransitions)
        {
            // TODO this entire function could be way more declarative
            if (SpaceState == BoardSpaceState.None)
            {
                VisualStateManager.GoToState(this, "None", true);
                return;
            }

            var isFriendlyPiece = SpaceState == BoardSpaceState.FriendlyPieceShort || SpaceState == BoardSpaceState.FriendlyPieceTall;
            VisualStateManager.GoToState(this, isFriendlyPiece ? "Friendly" : "Opponent", true);

            if (SpaceState == BoardSpaceState.OpponentPieceTall || SpaceState == BoardSpaceState.FriendlyPieceTall)
            {
                VisualStateManager.GoToState(this, "Tall", true);
            }
        }
    }
}