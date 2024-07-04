using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MAUIMiniApp.Models
{
    public class OTPItem : ObservableObject
    {
        string _OTP;
        public string OTP
        {
            get => _OTP;
            set => SetProperty(ref _OTP, value);
        }

        string _Account;
        public string Account
        {
            get => _Account;
            set => SetProperty(ref _Account, value);
        }

        int _TimerClock;
        public int TimerClock
        {
            get => _TimerClock;
            set => SetProperty(ref _TimerClock, value);
        }

        Color _TimerColor = Colors.Green;
        public Color TimerColor
        {
            get => _TimerColor;
            set => SetProperty(ref _TimerColor, value);
        }
    }
}
