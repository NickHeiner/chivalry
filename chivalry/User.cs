using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace chivalry
{
    // adapted from http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-1
    public class User : INotifyPropertyChanged
    {
        private string name;
        private string profilePicSource;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ProfilePicSource
        {
            get
            {
                return profilePicSource;
            }
            set
            {
                if (value != profilePicSource)
                {
                    value = profilePicSource;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
