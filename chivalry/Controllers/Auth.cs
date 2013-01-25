using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Live;
using Windows.UI.Xaml.Media;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace chivalry
{
    // check this out for auth pain: http://www.windowsazure.com/en-us/develop/mobile/tutorials/single-sign-on-windows-8-dotnet/#register
    public class Auth
    {
        private MobileServiceUser mobileServiceUser;
        internal async Task Authenticate()
        {
            while (mobileServiceUser == null)
            {
                try
                {
                    mobileServiceUser = await App.MobileService
                        .LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
                }
                catch (InvalidOperationException) {
                }
                if (mobileServiceUser == null)
                {
                    await (new MessageDialog("Logging in is required to use the app", "Sorry").ShowAsync());
                }
            }
        }

        public event EventHandler LoginFailed;

        private LiveConnectClient connection;

        // Is there a way to make props async?
        public async Task<User> CreateUser()
        {
            if (((App)Application.Current).OFFLINE_MODE)
            {
                return new User() { Email = "nth23@cornell.edu", Name = "Nick Heiner" };
            }

            await Authenticate();

            LiveConnectClient connection = await ensureConnection();

            if (connection == null)
            {
                return null;
            }
            LiveOperationResult meResult = await connection.GetAsync("me");
            dynamic userData = meResult.Result;
            User user = new User();
            if (userData != null)
            {
                user.Name = userData.name;
                user.Email = userData.emails.preferred; // assume this will always be populated
            }

            LiveOperationResult picResult = await connection.GetAsync("me/picture");
            dynamic picData = picResult.Result;
            if (picData != null)
            {
                user.ProfilePicSource = picData.location;
            }

            //await Authenticate();

            return user;
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
            LiveLoginResult loginResult = await LCAuth.LoginAsync(new string[] { "wl.basic", "wl.emails" });
            if (loginResult.Status == LiveConnectSessionStatus.Connected)
            {
                // Create a client session to get the profile data.
                connection = new LiveConnectClient(LCAuth.Session);
                return connection;
            }
            if (LoginFailed != null)
            {
                LoginFailed(this, null);
            }
            return null;
        }
    }
}
