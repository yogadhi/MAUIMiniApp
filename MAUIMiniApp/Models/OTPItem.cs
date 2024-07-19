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

        string _Account = string.Empty;
        public string Account
        {
            get { return _Account; }
            set
            {
                if (_Account != value)
                {
                    _Account = value;
                    OnPropertyChanged("Account");

                    _MaskAccount = YAP.Libs.Helpers.Globals.MaskString(_Account);
                    OnPropertyChanged("MaskAccount");
                }
            }
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

        string _MaskAccount = string.Empty;
        public string MaskAccount
        {
            get { return _MaskAccount; }
            set
            {
                if (_MaskAccount != value)
                {
                    _MaskAccount = value;
                }
            }
        }

        string _SecretKey;
        public string SecretKey
        {
            get => _SecretKey;
            set => SetProperty(ref _SecretKey, value);
        }

        double _TimerProgress = 0;
        public double TimerProgress
        {
            get { return _TimerProgress; }
            set
            {
                if (_TimerProgress != value)
                {
                    _TimerProgress = value;
                    OnPropertyChanged("TimerProgress");
                }
            }
        }
    }
}
