using chivalry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chivalry.Utils;

namespace chivalry.Controllers
{
    public static class GameController
    {
        /// <summary>
        /// These should probably be static resources,
        /// but I don't want to couple GameController to App
        /// </summary>
        public static readonly string LABEL_WAITING_ON_USER = "Your turn";
        public static readonly string LABEL_WAITING_ON_OTHER = "Awaiting other player";
        public static readonly string LABEL_DONE = "Finished";

        public static readonly string STATUS_WIN = "You win!";
        public static readonly string STATUS_LOSE = "You lose!";

        public static string LabelOf(User user, Game game)
        {
            var waitingOnInitiator = game.WaitingOn == AbsolutePlayer.Initiator;
            var currentUserIsInitiator = user.Email == game.InitiatingPlayerEmail;

            return 
                GameValidator.GameWinner(game) != RelativePlayer.None ? LABEL_DONE :
                waitingOnInitiator && currentUserIsInitiator ? LABEL_WAITING_ON_USER :
                waitingOnInitiator && !currentUserIsInitiator ? LABEL_WAITING_ON_OTHER :
                !waitingOnInitiator && currentUserIsInitiator ? LABEL_WAITING_ON_OTHER :
                LABEL_WAITING_ON_USER;
        }

        public static string StatusMessageOf(User user, Game game)
        {
           switch (GameValidator.GameWinner(game))
           {
                case RelativePlayer.Friendly:
                    return STATUS_WIN;
                case RelativePlayer.Opponent:
                    return STATUS_LOSE;
                case RelativePlayer.None:
                    return LabelOf(user, game);
           }

           throw new InvalidOperationException();
        }

        public static void OnBoardSpaceClick(User user, Game game, Coord coordClicked)
        {
            if (GameValidator.IsValidMoveFor(user, game, coordClicked))
            {
                game.AddActiveMove(coordClicked);
            }
        }

        public static void ExecuteMoves(Game game)
        {
            if (game.ActiveMoves.Count() == 0)
            {
                return;
            }

            if (!(game.ActiveMoves.Count() == 2 && GameUtils.AreNeighbors(game.ActiveMoves.First(), game.ActiveMoves.Last())))
            {
                var opponentsLoc =
                    game.ActiveMoves
                        .Pairwise()
                        .Select((moves) => GameUtils.SpaceBetween(moves.Item1, moves.Item2))
                        .Where(loc => GameUtils.IsOpponent(game.GetPieceAt(loc)));

                foreach (var opponentLoc in opponentsLoc)
                {
                    game.CapturePiece(game.GetPieceAt(opponentLoc));
                    game.SetPieceLocation(opponentLoc, BoardSpaceState.None);
                }
            }

            GameUtils.MovePiece(game, game.ActiveMoves.First(), game.ActiveMoves.Last());

            game.ClearActiveMoves();

            game.Winner = GameValidator.GameWinner(game);

            game.WaitingOn = game.WaitingOn.Toggle();
        }

        public static Game WithStartingPieces(Game game)
        {
            /**
             * This isn't as declarative as I'd like, but it's tough in a language
             * without object literals
             * 
             * I'd love to be able to say "the enemy's gate is down" here, but unfortunately
             * that's not the spec
             */
            fillTeam(game, 5, 1, BoardSpaceState.OpponentPieceShort, BoardSpaceState.OpponentPieceTall);
            fillTeam(game, 10, -1, BoardSpaceState.FriendlyPieceShort, BoardSpaceState.FriendlyPieceTall);

            return game;
        }

        private static void fillTeam(Game game, int startRow, int nextRowOffset, BoardSpaceState shortPiece, BoardSpaceState tallPiece)
        {
            int nextRow = startRow + nextRowOffset;

            game.SetPieceLocation(new Coord() { Row = startRow, Col = 2 }, tallPiece);
            game.SetPieceLocation(new Coord() { Row = nextRow, Col = 3 }, tallPiece);
            game.SetPieceLocation(new Coord() { Row = startRow, Col = 9 }, tallPiece);
            game.SetPieceLocation(new Coord() { Row = nextRow, Col = 8 }, tallPiece);

            fillRow(game, shortPiece, startRow, 3, 8);
            fillRow(game, shortPiece, nextRow, 4, 7);
        }

        private static void fillRow(Game game, BoardSpaceState toFill, int row, int lowerColBoundInclusive, int upperColBoundInclusive)
        {
            foreach (int col in Enumerable.Range(lowerColBoundInclusive, upperColBoundInclusive - lowerColBoundInclusive + 1))
            {
                game.SetPieceLocation(new Coord() { Row = row, Col = col }, toFill);
            }
        }

        // TODO this should be going off of email instead of name since name isn't unique
        public static void SetOtherPlayerLabel(User user, Game game)
        {
            game.OtherPlayerName = user.Name == game.InitiatingPlayerName ? game.RecepientPlayerName : game.InitiatingPlayerName;
        }
    }
}
