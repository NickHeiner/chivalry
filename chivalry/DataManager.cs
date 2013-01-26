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
                againstScott.SetPieceLocation(5, 5, BoardSpaceState.FriendlyPieceShort);
                againstScott.SetPieceLocation(5, 6, BoardSpaceState.FriendlyPieceTall);
                againstScott.SetPieceLocation(4, 4, BoardSpaceState.OpponentPieceShort);
                againstScott.SetPieceLocation(5, 8, BoardSpaceState.OpponentPieceTall);

                user.Games.Add(againstScott);
                user.Games.Add(withStartingPieces(new Game() { RecepientPlayerName = "Dad" }));

                return user;
            }

            var games = await gameTable.ToListAsync();
            foreach (var game in games)
            {
                user.Games.Add(game);
            }

            return user;
        }

        internal async void AddNewGame(string initiatingPlayerName, string initiatingPlayerEmail, string againstUserName, string againstUserEmail)
        {
            await gameTable.InsertAsync(new Game() 
            { 
                InitiatingPlayerName = initiatingPlayerName,
                InitiatingPlayerEmail = initiatingPlayerEmail,
                RecepientPlayerName = againstUserName, 
                RecepientPlayerEmail = againstUserEmail,
            });
        }

        private Game withStartingPieces(Game game)
        {
            //  This isn't as declarative as I'd like, but it's tough in a language
            //  without object literals
            fillTeam(game, 5, 1, BoardSpaceState.FriendlyPieceShort, BoardSpaceState.FriendlyPieceTall);
            fillTeam(game, 10, -1, BoardSpaceState.OpponentPieceShort, BoardSpaceState.OpponentPieceTall);

            return game;
        }

        private void fillTeam(Game game, int startRow, int nextRowOffset, BoardSpaceState shortPiece, BoardSpaceState tallPiece)
        {
            int nextRow = startRow + nextRowOffset;

            game.SetPieceLocation(startRow, 2, tallPiece);
            game.SetPieceLocation(nextRow, 3, tallPiece);
            game.SetPieceLocation(startRow, 9, tallPiece);
            game.SetPieceLocation(nextRow, 8, tallPiece);

            fillRow(game, shortPiece, startRow, 3, 8);
            fillRow(game, shortPiece, nextRow, 4, 7);
        }

        private void fillRow(Game game, BoardSpaceState toFill, int row, int lowerColBoundInclusive, int upperColBoundInclusive)
        {
            foreach (int col in Enumerable.Range(lowerColBoundInclusive, upperColBoundInclusive - lowerColBoundInclusive + 1))
            {
                game.SetPieceLocation(row, col, toFill);
            }
        }
    }
}
