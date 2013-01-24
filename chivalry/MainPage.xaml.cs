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
using Windows.ApplicationModel.Contacts;
using chivalry.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace chivalry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private User user
        {
            get
            {
                return (User) DataContext;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            loadDataContext();
        }

        void Auth_LoginFailed(object sender, EventArgs e)
        {
            gamesView.Visibility = Visibility.Collapsed;
            loginFailedMessage.Visibility = Visibility.Visible;
        }

        private async void loadDataContext()
        {
            DataContext = await ((App)Application.Current).getUser();
            if (DataContext != null)
            {
                newGameButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void GridView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Frame.Navigate(typeof(PlayGame), e.AddedItems[0]);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var contact = await (new ContactPicker().PickSingleContactAsync());

            if (contact.Emails.Count() == 0)
            {
                new MessageDialog("Please select a player who has an email address listed in your contacts", "Email Address Required");
                return;
            }

            ((App)Application.Current).DataManager.AddNewGame(user.Name, user.Email, contact.Name, contact.Emails.First().Value);
        }
    }
}
