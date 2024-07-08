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

        DateTime _endTime = DateTime.Now.AddSeconds(30);
        public DateTime endTime
        {
            get { return _endTime; }
            set { SetProperty(ref _endTime, value); }
        }

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

        ObservableCollection<OTPItem> _OTPItemList = new ObservableCollection<OTPItem>();
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
                endTime = DateTime.Now;
                timer.Start();
                //LoadCommand.Execute(null);
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
            try
            {
                if (IsBusy)
                {
                    return;
                }

                var hasAcceptToS = await SecureStorage.GetAsync("hasAcceptToS");
                if (string.IsNullOrEmpty(hasAcceptToS))
                {
                    return;
                }

                IsBusy = true;

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
                    }
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Log.Write(Log.LogEnum.Error, nameof(ExecuteLoadCommand), ex);
            }
        }

        ICommand _SelectionCommand;
        public ICommand SelectionCommand => _SelectionCommand ?? (_SelectionCommand = new Command<OTPItem>(async (x) => await ExecuteSelectionCommand(x)));
        async Task ExecuteSelectionCommand(OTPItem obj)
        {
            try
            {
                if (obj != null)
                {
                    string action = await Application.Current.MainPage.DisplayActionSheet((string)obj.Account, "Cancel", null, new string[] { "Copy", "Rename", "Remove", "Unbind" });
                    if (!string.IsNullOrEmpty(action))
                    {
                        if (action == "Copy")
                        {
                            await Clipboard.Default.SetTextAsync((string)obj.OTP);
                            Toasts.Show((string)(obj.OTP + " copied"));
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
                    endTime = DateTime.Now;
                    LoadCommand.Execute(null);
                }
                else
                {
                    if (OTPItemList != null)
                    {
                        if (OTPItemList.Count > 0)
                        {
                            OTPItemList.Select(c =>
                            {
                                c.TimerClock = cTimerInt;
                                c.TimerColor = ProgressColor;
                                return c;
                            }).ToList();
                        }
                    }
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
            try
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.ShowPopupAsync(new NewAccountPage());
                });
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteNewAccountCommand), ex);
            }
        }

        ICommand _DecodeScanResultCommand;
        public ICommand DecodeScanResultCommand => _DecodeScanResultCommand ?? (_DecodeScanResultCommand = new Command<string>(async (x) => await ExecuteDecodeScanResultCommand(x)));
        async Task ExecuteDecodeScanResultCommand(string scanResult)
        {
            try
            {
                if (!string.IsNullOrEmpty(scanResult))
                {
                    var strSplit = scanResult.Split("~:~");
                    if (strSplit != null)
                    {
                        if (strSplit.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(strSplit[0]))
                            {
                                var strSplitZero = strSplit[0].Split("&");
                                if (strSplitZero != null)
                                {
                                    if (strSplitZero.Length == 3)
                                    {
                                        var acctNo = strSplitZero[0].Split("cq2faauth://totp/")[1].Split("?")[0];
                                        var companyCode = strSplitZero[2].Split("companycode=")[1];
                                        var secretKey = strSplitZero[0].Split("secret=")[1];

                                        var obj = new Account
                                        {
                                            AccountNo = acctNo.ToUpper(),
                                            CompanyCode = companyCode,
                                            SecretKey = secretKey
                                        };

                                        var resSave = await AccountDatabase.SaveItemAsync(obj);
                                        if (resSave == 1)
                                        {
                                            WeakReferenceMessenger.Default.Send(new MyMessage(new MessageContainer { Key = "RefreshList" }));
                                        }
                                    }
                                }
                                //await Application.Current.MainPage.Navigation.PopModalAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteDecodeScanResultCommand), ex);
            }
        }
        #endregion
    }
}
