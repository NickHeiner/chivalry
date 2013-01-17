using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Live;

namespace chivalry
{
    class Auth
    {
        private LiveConnectClient connection;

        // Is there a way to make props async?
        public async Task<User> createUser()
        {
            LiveConnectClient connection = await ensureConnection();
            LiveOperationResult meResult = await connection.GetAsync("me");
            dynamic userData = meResult.Result;
            if (userData != null)
            {
                return new User { Name = userData.name };
            }
            throw new InvalidOperationException("couldn't get name");
        }

        private async Task<LiveConnectClient> ensureConnection()
        {
            if (connection != null)
            {
                return connection;
            }
            // Initialize access to the Live Connect SDK.
            LiveAuthClient LCAuth = new LiveAuthClient();
            LiveLoginResult LCLoginResult = await LCAuth.InitializeAsync();
            // Sign in to the user's Microsoft account with the required scope.
            //    
            //  This call will display the Microsoft account sign-in screen if the user 
            //  is not already signed in to their Microsoft account through Windows 8.
            // 
            //  This call will also display the consent dialog, if the user has 
            //  has not already given consent to this app to access the data described 
            //  by the scope.
            // 
            LiveLoginResult loginResult = await LCAuth.LoginAsync(new string[] { "wl.basic" });
            if (loginResult.Status == LiveConnectSessionStatus.Connected)
            {
                // Create a client session to get the profile data.
                connection = new LiveConnectClient(LCAuth.Session);
                return connection;
            }
            throw new InvalidOperationException("Couldn't connect to Live");
        }
    }
}
