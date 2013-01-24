using chivalry.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chivalry
{
    public class DataManager
    {
        private IMobileServiceTable<Game> gameTable = App.MobileService.GetTable<Game>();

        public async Task<User> withServerData(User user)
        {
            user = user ?? new User();

            //Game againstScott = new Game() { RecepientPlayerName = "Scott" };
            //againstScott.SetPieceLocation(5, 5, BoardSpaceState.FriendlyPieceShort);
            //againstScott.SetPieceLocation(5, 6, BoardSpaceState.FriendlyPieceTall);
            //againstScott.SetPieceLocation(4, 4, BoardSpaceState.OpponentPieceShort);    
            //againstScott.SetPieceLocation(5, 8, BoardSpaceState.OpponentPieceTall);
            
            //user.Games.Add(againstScott);
            //user.Games.Add(new Game() { RecepientPlayerName = "Dad" });

            //// return synchronously for now
            //var taskSource = new TaskCompletionSource<User>();
            //taskSource.SetResult(user);
            //return taskSource.Task;

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
                RecepientPlayerEmail = againstUserEmail
            });
        }
    }
}
