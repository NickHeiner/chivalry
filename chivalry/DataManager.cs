using chivalry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chivalry
{
    class DataManager
    {
        public Task<User> withServerData(User user)
        {
            user = user ?? new User();

            Game againstScott = new Game() { AgainstUserName = "Scott" };
            againstScott.SetPieceLocation(5, 5, BoardSpaceState.FriendlyPieceShort);
            againstScott.SetPieceLocation(5, 6, BoardSpaceState.FriendlyPieceTall);
            againstScott.SetPieceLocation(4, 4, BoardSpaceState.OpponentPieceShort);
            againstScott.SetPieceLocation(5, 8, BoardSpaceState.OpponentPieceTall);
            
            user.Games.Add(againstScott);
            user.Games.Add(new Game() { AgainstUserName = "Dad" });

            // return synchronously for now
            var taskSource = new TaskCompletionSource<User>();
            taskSource.SetResult(user);
            return taskSource.Task;
        }
    }
}
