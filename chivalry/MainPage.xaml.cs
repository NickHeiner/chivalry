using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Microsoft.Live;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace chivalry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void signInButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize access to the Live Connect SDK.
                LiveAuthClient LCAuth = new LiveAuthClient();
                LiveLoginResult LCLoginResult = await LCAuth.InitializeAsync();
                try
                {
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
                        LiveConnectClient connect = new LiveConnectClient(LCAuth.Session);

                        // Get the profile info of the user.
                        LiveOperationResult operationResult = await connect.GetAsync("me");

                        // Format the text to display and update the element in the layout.
                        dynamic result = operationResult.Result;
                        if (result != null)
                        {
                            this.userName.Text = string.Join(" ", "Hello", result.name, "!");
                        }
                        else
                        {
                            this.userName.Text = "Unable to get your name.";
                        }
                    }

                }
                catch (LiveAuthException exception)
                {
                    // handle the login, scope, or request exception
                }
            }
            catch (LiveAuthException exception)
            {
                // handle the initialization exception
            }
            catch (LiveConnectException exception)
            {
                // handle the Live Connect API exception
            }

        }
    }
}
