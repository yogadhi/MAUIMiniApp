using MAUIMiniApp.Models;
using System.Windows.Input;
using YAP.Libs.Logger;
using YAP.Libs.ViewModels;
using YAP.Libs.Alerts;
using MAUIMiniApp.Views;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;

namespace MAUIMiniApp.ViewModels
{
    public class OTPViewModel : BaseViewModel
    {
        decimal maxInterval = 30m;
        System.Timers.Timer timer;
        DateTime endTime = DateTime.Now.AddSeconds(30);

        string _CurrentOTP = string.Empty;
        public string CurrentOTP
        {
            get { return _CurrentOTP; }
            set { SetProperty(ref _CurrentOTP, value); }
        }

        int _cTimerInt = 0;
        public int cTimerInt
        {
            get { return _cTimerInt; }
            set
            {
                if (_cTimerInt != value)
                {
                    _cTimerInt = value;
                    OnPropertyChanged("cTimerInt");

                    _cTimer = _cTimerInt.ToString();
                    OnPropertyChanged("cTimer");

                    if (_cTimerInt > 15)
                    {
                        _ProgressColor = Colors.Green;
                    }
                    else if (_cTimerInt <= 15 && _cTimerInt > 10)
                    {
                        _ProgressColor = Colors.Yellow;
                    }
                    else if (_cTimerInt <= 10)
                    {
                        _ProgressColor = Colors.Red;
                    }
                    OnPropertyChanged("ProgressColor");

                    var x = (decimal)_cTimerInt / (decimal)maxInterval;
                    var y = Math.Round(x, 2, MidpointRounding.AwayFromZero);

                    if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
                    {
                        _Progress = 1 - y;
                    }
                    else
                    {
                        _Progress = (int)(y * 100);
                    }
                    OnPropertyChanged("Progress");

                }
            }
        }

        string _cTimer = string.Empty;
        public string cTimer
        {
            get { return _cTimer; }
            set
            {
                if (_cTimer != value)
                {
                    _cTimer = value;
                }
            }
        }

        decimal _Progress;
        public decimal Progress
        {
            get { return _Progress; }
            set
            {
                if (_Progress != value)
                {
                    _Progress = value;
                }
            }
        }

        Color _ProgressColor = Colors.Green;
        public Color ProgressColor
        {
            get { return _ProgressColor; }
            set
            {
                if (_ProgressColor != value)
                {
                    _ProgressColor = value;
                }
            }
        }

        List<OTPItem> _OTPItemList;
        public List<OTPItem> OTPItemList
        {
            get { return _OTPItemList; }
            set
            {
                if (_OTPItemList != value)
                {
                    _OTPItemList = value;
                    OnPropertyChanged("OTPItemList");
                }
            }
        }

        public OTPViewModel()
        {
            try
            {
                Title = "One-Time Password";

                //OpenWebCommand.Execute("https://aka.ms/xamarin-quickstart");
                LoadCommand.Execute(null);
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(OTPViewModel) + " - " + ex.Message);
            }
        }

        ICommand _LoadCommand;
        public ICommand LoadCommand => _LoadCommand ?? (_LoadCommand = new Command(async () => await ExecuteLoadCommand()));
        async Task ExecuteLoadCommand()
        {
            IsBusy = true;

            try
            {
                Random generator = new Random();

                OTPItemList = new List<OTPItem>()
                {
                    new OTPItem() { Account = "yogadhiprananda@gmail.com", OTP = generator.Next(0, 1000000).ToString("D6") },
                    new OTPItem() { Account = "yogadhipra93@gmail.com", OTP = generator.Next(0, 1000000).ToString("D6") },
                };

                timer = new System.Timers.Timer
                {
                    Interval = 1000
                };
                timer.Elapsed += t_Tick;
                TimeSpan ts = endTime - DateTime.Now;
                cTimerInt = ts.Seconds;
                timer.Start();
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteLoadCommand) + " - " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        ICommand _SelectionCommand;
        public ICommand SelectionCommand => _SelectionCommand ?? (_SelectionCommand = new Command<OTPItem>(async (x) => await ExecuteSelectionCommand(x)));
        async Task ExecuteSelectionCommand(OTPItem obj)
        {
            try
            {
                await Clipboard.Default.SetTextAsync(obj.OTP);
                Toasts.Show(obj.OTP + " copied");
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteSelectionCommand) + " - " + ex.Message);
            }
        }

        void t_Tick(object sender, EventArgs e)
        {
            try
            {
                TimeSpan ts = endTime - DateTime.Now;
                cTimerInt = ts.Seconds;

                if ((ts.TotalMilliseconds < 0) || (ts.TotalMilliseconds < 1000))
                {
                    timer.Stop();

                    Random generator = new Random();

                    OTPItemList.Select(c => { c.OTP = generator.Next(0, 1000000).ToString("D6"); return c; }).ToList();

                    endTime = DateTime.Now.AddSeconds(30);

                    LoadCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(t_Tick) + " - " + ex.Message);
            }
        }

        ICommand _CheckToSCommand;
        public ICommand CheckToSCommand => _CheckToSCommand ?? (_CheckToSCommand = new Command(async () => await ExecuteCheckToSCommand()));
        async Task ExecuteCheckToSCommand()
        {
            try
            {
                var hasAuth = await SecureStorage.GetAsync("hasAuth");
                var hasAcceptToS = await SecureStorage.GetAsync("hasAcceptToS");

                if (!string.IsNullOrEmpty(hasAuth) && string.IsNullOrEmpty(hasAcceptToS))
                {
                    WeakReferenceMessenger.Default.Send(new MyMessage("hasAcceptToS"));
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteCheckToSCommand) + " - " + ex.Message);
            }
        }
    }
}
