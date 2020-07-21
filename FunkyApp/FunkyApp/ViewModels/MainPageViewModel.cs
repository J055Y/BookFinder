using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using FunkyApp.Annotations;
using Xamarin.Forms;

namespace FunkyApp
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {

            MsgCollection = new ObservableCollection<MsgModel>();

            GiveUserMsgCommand = new Command(() =>
            {
                var msg = new MsgModel
                {
                    MsgTitle = "Sup :P",
                    MsgContent = "You pressed the button!"
                };

                MsgCollection.Add(msg);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<MsgModel> MsgCollection { get; }
        public Command GiveUserMsgCommand { get; }
    }
}
