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
            Game againstScott = new Game() { AgainstUserName = "Scott" };
            againstScott.SetPieceLocation(5, 5, BoardSpaceState.FriendlyPieceShort);
            
            user.Games.Add(againstScott);
            user.Games.Add(new Game() { AgainstUserName = "Dad" });

            // return synchronously for now
            var taskSource = new TaskCompletionSource<User>();
            taskSource.SetResult(user);
            return taskSource.Task;
        }
    }
}
