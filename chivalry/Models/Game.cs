using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chivalry.Models
{
    public class Game : INotifyPropertyChanged
    {
        private string againstUserName;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string AgainstUserName
        {
            get
            {
                return againstUserName;
            }
            set
            {
                if (againstUserName != value)
                {
                    againstUserName = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
