using chivalry.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace chivalry
{
    public class DataManager
    {
        private IMobileServiceTable<Game> gameTable = App.MobileService.GetTable<Game>();

        public async Task<User> withServerData(User user)
        {
            user = user ?? new User();

            if (((App)Application.Current).OFFLINE_MODE)
            {
                Game againstScott = new Game() { RecepientPlayerName = "Scott" };
                againstScott.SetPieceLocation(new Coord() { Row = 5, Col = 5 }, BoardSpaceState.FriendlyPieceShort);
                againstScott.SetPieceLocation(new Coord() { Row = 5, Col = 6 }, BoardSpaceState.FriendlyPieceTall);
                againstScott.SetPieceLocation(new Coord() { Row = 4, Col = 4 }, BoardSpaceState.OpponentPieceShort);
                againstScott.SetPieceLocation(new Coord() { Row = 5, Col = 8 }, BoardSpaceState.OpponentPieceTall);

                user.Games.Add(againstScott);
                user.Games.Add(withStartingPieces(new Game() { RecepientPlayerName = "Dad" }));

                return user;
            }

            user.Games.Clear();
            var games = await gameTable.ToListAsync();
            foreach (var game in games)
            {
                updateWithUserData(game, user);
                user.Games.Add(game);
            }

            return user;
        }

        // visible for testing
        public void updateWithUserData(Game game, User user)
        {
            game.Transformation = game.RecepientPlayerEmail == user.Email ? BoardCoord.Transformation.FLIP : BoardCoord.Transformation.NO_FLIP;
        }

        internal async void AddNewGame(string initiatingPlayerName, string initiatingPlayerEmail, string againstUserName, string againstUserEmail)
        {
            await gameTable.InsertAsync(withStartingPieces(new Game() 
            { 
                InitiatingPlayerName = initiatingPlayerName,
                InitiatingPlayerEmail = initiatingPlayerEmail,
                RecepientPlayerName = againstUserName, 
                RecepientPlayerEmail = againstUserEmail,
            }));
        }

        private Game withStartingPieces(Game game)
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

        private void fillTeam(Game game, int startRow, int nextRowOffset, BoardSpaceState shortPiece, BoardSpaceState tallPiece)
        {
            int nextRow = startRow + nextRowOffset;

            game.SetPieceLocation(new Coord() { Row = startRow, Col = 2 }, tallPiece);
            game.SetPieceLocation(new Coord() { Row = nextRow, Col = 3 }, tallPiece);
            game.SetPieceLocation(new Coord() { Row = startRow, Col = 9 }, tallPiece);
            game.SetPieceLocation(new Coord() { Row = nextRow, Col = 8 }, tallPiece);

            fillRow(game, shortPiece, startRow, 3, 8);
            fillRow(game, shortPiece, nextRow, 4, 7);
        }

        private void fillRow(Game game, BoardSpaceState toFill, int row, int lowerColBoundInclusive, int upperColBoundInclusive)
        {
            foreach (int col in Enumerable.Range(lowerColBoundInclusive, upperColBoundInclusive - lowerColBoundInclusive + 1))
            {
                game.SetPieceLocation(new Coord() { Row = row, Col = col }, toFill);
            }
        }
    }
}
