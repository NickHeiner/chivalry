﻿using System;
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
    public class Auth
    {
        private MobileServiceUser mobileServiceUser;
        public event EventHandler LoginFailed;
        private LiveConnectClient connection;

        public async Task<User> CreateUser()
        {
            if (((App)Application.Current).OFFLINE_MODE)
            {
                return new User() { Email = "nth23@cornell.edu", Name = "Nick Heiner" };
            }

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

            return user;
        }

        private async Task<LiveConnectClient> ensureConnection()
        {
            if (connection != null)
            {
                return connection;
            }
            // Initialize access to the Live Connect SDK.
            LiveAuthClient LCAuth = new LiveAuthClient("https://chivalry.azure-mobile.net/");
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
                mobileServiceUser = await App.MobileService.LoginAsync(loginResult.Session.AuthenticationToken);
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
