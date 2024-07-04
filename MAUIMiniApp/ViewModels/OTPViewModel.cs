using MAUIMiniApp.Models;
using System.Windows.Input;
using YAP.Libs.Logger;
using YAP.Libs.ViewModels;
using YAP.Libs.Alerts;
using MAUIMiniApp.Views;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using MAUIMiniApp.Data;

namespace MAUIMiniApp.ViewModels
{
    public class OTPViewModel : BaseViewModel
    {
        #region Variables
        decimal maxInterval = 30m;
        DateTime endTime = DateTime.Now.AddSeconds(30);

        System.Timers.Timer _timer = new System.Timers.Timer { Interval = 1000 };
        public System.Timers.Timer timer
        {
            get { return _timer; }
            set { SetProperty(ref _timer, value); }
        }

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

        int _Progress;
        public int Progress
        {
            get => _Progress;
            set => SetProperty(ref _Progress, value);
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

        ObservableCollection<OTPItem> _OTPItemList;
        public ObservableCollection<OTPItem> OTPItemList
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

        OTPItem _SelectedOTP;
        public OTPItem SelectedOTP
        {
            get { return _SelectedOTP; }
            set
            {
                if (_SelectedOTP != value)
                {
                    _SelectedOTP = value;
                    OnPropertyChanged("SelectedOTP");
                }
            }
        }
        #endregion

        public OTPViewModel()
        {
            try
            {
                Title = "CQ Authenticator";
                timer.Elapsed += t_Tick;
                LoadCommand.Execute(null);
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(OTPViewModel), ex);
            }
        }

        #region Commands
        ICommand _LoadCommand;
        public ICommand LoadCommand => _LoadCommand ?? (_LoadCommand = new Command(async () => await ExecuteLoadCommand()));
        async Task ExecuteLoadCommand()
        {
            IsBusy = true;

            try
            {
                //await AccountDatabase.TruncateItemAsync();

                var resList = await AccountDatabase.GetItemsAsync();
                if (resList != null)
                {
                    if (resList.Count > 0)
                    {
                        Random generator = new Random();

                        OTPItemList = new ObservableCollection<OTPItem>();
                        foreach (var index in resList)
                        {
                            OTPItemList.Add(new OTPItem
                            {
                                Account = index.AccountNo,
                                OTP = generator.Next(0, 1000000).ToString("D6")
                            });
                        }

                        endTime = DateTime.Now.AddSeconds(30);
                        TimeSpan ts = endTime - DateTime.Now;
                        cTimerInt = ts.Seconds;
                        timer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteLoadCommand), ex);
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
                var objOTPItemSeelcted = SelectedOTP;
                if (objOTPItemSeelcted != null)
                {
                    string action = await App.Current.MainPage.DisplayActionSheet(objOTPItemSeelcted.Account, "Cancel", null, new string[] { "Copy", "Rename", "Remove", "Unbind" });
                    if (!string.IsNullOrEmpty(action))
                    {
                        if (action == "Copy")
                        {
                            await Clipboard.Default.SetTextAsync(objOTPItemSeelcted.OTP);
                            Toasts.Show(objOTPItemSeelcted.OTP + " copied");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteSelectionCommand), ex);
            }
            finally
            {
                SelectedOTP = null;
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
                    LoadCommand.Execute(null);
                }
                else
                {
                    OTPItemList.Select(c =>
                    {
                        c.TimerClock = cTimerInt;
                        c.TimerColor = ProgressColor;
                        return c;
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(t_Tick), ex);
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
                    Application.Current.MainPage = new ToSPage();
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteCheckToSCommand), ex);
            }
        }

        ICommand _NewAccountCommand;
        public ICommand NewAccountCommand => _NewAccountCommand ?? (_NewAccountCommand = new Command(async () => await ExecuteNewAccountCommand()));
        async Task ExecuteNewAccountCommand()
        {
            IsBusy = true;

            try
            {
                //await App.Current.MainPage.Navigation.PushAsync(new NewAccountPage());
                await App.Current.MainPage.ShowPopupAsync(new NewAccountPage());
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteNewAccountCommand), ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
