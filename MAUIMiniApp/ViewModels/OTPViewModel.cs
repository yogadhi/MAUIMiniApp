using MAUIMiniApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YAP.Libs.Logger;
using YAP.Libs.ViewModels;
using YAP.Libs.Alerts;

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

        ICommand _OpenWebCommand;
        public ICommand OpenWebCommand => _OpenWebCommand ?? (_OpenWebCommand = new Command<string>(async (x) => await ExecuteOpenWebCommand(x)));
        async Task ExecuteOpenWebCommand(string url)
        {
            try
            {
                await Browser.OpenAsync(url);
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteOpenWebCommand) + " - " + ex.Message);
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
    }
}
