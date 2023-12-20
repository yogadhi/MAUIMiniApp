using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YAP.Libs.Logger;
using YAP.Libs.ViewModels;

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

        string _cTimer = string.Empty;
        public string cTimer
        {
            get { return _cTimer; }
            set { SetProperty(ref _cTimer, value); }
        }

        int _Progress = 40;
        public int Progress
        {
            get { return _Progress; }
            set
            {
                if(_Progress != value)
                {
                    _Progress = value;
                    OnPropertyChanged("Progress");
                }
            }
        }

        public OTPViewModel()
        {
            try
            {
                Title = "One-Time Password";
                Random generator = new Random();
                CurrentOTP = generator.Next(0, 1000000).ToString("D6");
                //OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
                LoadCommand.Execute(null);
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(OTPViewModel) + " - " + ex.Message);
            }
        }

        public ICommand OpenWebCommand { get; }


        ICommand _LoadCommand;
        public ICommand LoadCommand => _LoadCommand ?? (_LoadCommand = new Command(async () => await ExecuteLoadCommand()));
        async Task ExecuteLoadCommand()
        {
            IsBusy = true;

            try
            {
                timer = new System.Timers.Timer
                {
                    Interval = 1000
                };
                timer.Elapsed += t_Tick;
                TimeSpan ts = endTime - DateTime.Now;
                cTimer = ts.Seconds.ToString();
                timer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        void t_Tick(object sender, EventArgs e)
        {
            try
            {
                TimeSpan ts = endTime - DateTime.Now;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Code to run on the main thread  
                    cTimer = ts.Seconds.ToString();
                    var x = (decimal)ts.Seconds / (decimal)maxInterval;
                    var y = Math.Round(x, 2, MidpointRounding.AwayFromZero);

                    //if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
                    //{
                    //    Progress = (int)(1 - y);
                    //}
                    //else
                    //{
                    //    Progress = (int)(y * 100);
                    //}
                    Progress = (int)(y * 100);
                });


                if ((ts.TotalMilliseconds < 0) || (ts.TotalMilliseconds < 1000))
                {
                    timer.Stop();

                    Random generator = new Random();
                    CurrentOTP = generator.Next(0, 1000000).ToString("D6");
                    endTime = DateTime.Now.AddSeconds(30);
                    LoadCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }
    }
}
