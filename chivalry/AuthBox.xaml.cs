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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace chivalry
{
    public sealed partial class AuthBox : UserControl
    {
        public AuthBox()
        {
            this.InitializeComponent();
            loadDataContext();
        }

        private async void loadDataContext()
        {
            //((App)Application.Current).getUser().ContinueWith(user =>
            //    {
            //        DataContext = user.;
            //        if (user == null)
            //        {
            //            notLoggedInMessage.Visibility = Visibility.Visible;
            //        }
            //    });
            DataContext = await ((App)Application.Current).getUser();
            if (DataContext == null)
            {
                notLoggedInMessage.Visibility = Visibility.Visible;
            }

        }
    }
}
