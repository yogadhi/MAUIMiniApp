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
using System;
using System.Globalization;
using MAUIMiniApp.Controllers;

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
            get => _OTPItemList;
            set => SetProperty(ref _OTPItemList, value);
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

        string _DeviceID;
        public string DeviceID
        {
            get => _DeviceID;
            set => SetProperty(ref _DeviceID, value);
        }
        #endregion

        public OTPViewModel(INavigation navigation)
        {
            try
            {
                Navigation = navigation;
                Title = Resources.Strings.AppResources.App_Title;
                DeviceID = new GetDeviceInfo().GetDeviceID();
                timer.Elapsed += t_Tick;
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
                if (IsBusy) { return; }
                IsBusy = true;

                var strLastUnbindAccount = await SecureStorage.GetAsync("lastUnbindAccount");
                if (!string.IsNullOrEmpty(strLastUnbindAccount))
                {
                    var list = strLastUnbindAccount.Split("|");
                    if (list != null)
                    {
                        if (list.Length > 0)
                        {
                            var listDistinct = list.Distinct().ToList();
                            foreach (var index in listDistinct)
                            {
                                var objAccount = await AccountDatabase.GetItemByAccodeAsync(index);
                                if (objAccount != null)
                                {
                                    var checkBind = await CQAuth.CheckUserBinded(objAccount);
                                    if (checkBind == 0)
                                    {
                                        var resDelete = await AccountDatabase.DeleteItemAsync(objAccount);
                                    }
                                }
                            }
                            SecureStorage.Remove("lastUnbindAccount");
                        }
                    }
                }

                OTPItemList = new ObservableCollection<OTPItem>();

                //await AccountDatabase.TruncateItemAsync();
                var resList = await AccountDatabase.GetItemsAsync();
                if (resList == null)
                {
                    timer.Enabled = false;
                    return;
                }

                if (resList.Count == 0)
                {
                    timer.Enabled = false;
                    return;
                }

                foreach (var index in resList)
                {
                    OTPItemList.Add(new OTPItem
                    {
                        Account = index.Accode,
                        SecretKey = index.SecretKey,
                        OTP = YAP.Libs.Helpers.Globals.GetFuturePIN(index.SecretKey),
                    });
                }

                if (!timer.Enabled)
                {
                    timer.Enabled = true;
                }
                endTime = DateTime.Now.AddSeconds(30);
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

        ICommand _RefreshCommand;
        public ICommand RefreshCommand => _RefreshCommand ?? (_RefreshCommand = new Command(async () => await ExecuteRefreshCommand()));
        async Task ExecuteRefreshCommand()
        {
            try
            {
                IsRefreshing = true;

                if (OTPItemList == null)
                {
                    return;
                }

                if (OTPItemList.Count == 0)
                {
                    return;
                }

                IsBusy = false;
                if (!timer.Enabled)
                {
                    timer.Enabled = true;
                }
                endTime = DateTime.Now;
                OTPItemList = new ObservableCollection<OTPItem>();
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteRefreshCommand), ex);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        ICommand _SelectionCommand;
        public ICommand SelectionCommand => _SelectionCommand ?? (_SelectionCommand = new Command<OTPItem>(async (x) => await ExecuteSelectionCommand(x)));
        async Task ExecuteSelectionCommand(OTPItem obj)
        {
            try
            {
                SelectedOTP = obj;
                if (obj != null)
                {
                    var action = await Application.Current.MainPage.DisplayActionSheet(obj.Account, "Cancel", null, new string[] { "Copy", "Rename", "Remove", "Unbind" });
                    if (!string.IsNullOrEmpty(action))
                    {
                        if (action == "Copy")
                        {
                            await Clipboard.Default.SetTextAsync((string)obj.OTP);
                            Toasts.Show(obj.OTP + " copied");
                        }
                        else if (action == "Remove")
                        {
                            var remove = await App.AlertSvc.ShowConfirmationAsync("Remove " + obj.Account, "Are you sure you want to remove account " + obj.Account, "Yes", "No");
                            if (!remove)
                            {
                                return;
                            }

                            var account = await AccountDatabase.GetItemByAccodeAsync(obj.Account);
                            if (account == null)
                            {
                                return;
                            }

                            var resDelete = await AccountDatabase.DeleteItemAsync(account);
                            if (resDelete == 1)
                            {
                                endTime = DateTime.Now;
                            }
                        }
                        else if (action == "Rename")
                        {

                        }
                        else if (action == "Unbind")
                        {
                            string Lang = string.Empty;

                            var remove = await App.AlertSvc.ShowConfirmationAsync("Unbind " + obj.Account, "Are you sure you want to unbind account " + obj.Account, "Yes", "No");
                            if (!remove)
                            {
                                return;
                            }

                            var account = await AccountDatabase.GetItemByAccodeAsync(obj.Account);
                            if (account == null)
                            {
                                return;
                            }

                            var currCul = CultureInfo.InstalledUICulture;
                            if (currCul.Name.Contains("zh"))
                            {
                                Lang = "ZH";
                            }
                            else
                            {
                                Lang = "EN";
                            }

                            IsBusy = true;

                            var strLastUnbindAccount = await SecureStorage.GetAsync("lastUnbindAccount");
                            if (!string.IsNullOrEmpty(strLastUnbindAccount))
                            {
                                List<string> lastUnbindAccountList = new List<string>();
                                lastUnbindAccountList.Add(strLastUnbindAccount);
                                lastUnbindAccountList.Add(obj.Account);
                                await SecureStorage.SetAsync("lastUnbindAccount", string.Join("|", lastUnbindAccountList));
                            }
                            else
                            {
                                await SecureStorage.SetAsync("lastUnbindAccount", obj.Account);
                            }

                            var resUnbind = await CQAuth.Delink(account, new ReqDelink { SysID = Convert.ToInt32(account.CompanyCode), Username = account.Accode });
                            if (resUnbind != null)
                            {
                                if (resUnbind.data)
                                {
                                    endTime = DateTime.Now;
                                }
                            }
                            //Uri uri = new Uri(String.Format("https://cq2fa.cyberquote.com.hk/registration//Unbind?CompanyCode={0}&lang={1}&Accode={2}", account.CompanyCode, Lang, account.Accode));
                            //await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                        }
                    }
                }
                SelectedOTP = null;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteSelectionCommand), ex);
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
                cTimerInt = ts.Seconds;

                if ((ts.TotalMilliseconds < 0) || (ts.TotalMilliseconds < 1000))
                {
                    if (!IsBusy)
                    {
                        LoadCommand.Execute(null);
                    }
                }
                else
                {
                    if (!IsBusy)
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
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(t_Tick), ex);
            }
        }

        ICommand _DecodeScanResultCommand;
        public ICommand DecodeScanResultCommand => _DecodeScanResultCommand ?? (_DecodeScanResultCommand = new Command<string>(async (x) => await ExecuteDecodeScanResultCommand(x)));
        async Task ExecuteDecodeScanResultCommand(string scanResult)
        {
            try
            {
                IsBusy = true;

                if (string.IsNullOrEmpty(scanResult))
                {
                    Toasts.Show(Resources.Strings.AppResources.No_QRCode_Result);
                    return;
                }

                var strSplit = scanResult.Split("~:~");
                if (strSplit == null)
                {
                    Toasts.Show(Resources.Strings.AppResources.Wrong_QRCode_Format);
                    return;
                }

                if (strSplit.Length == 0)
                {
                    Toasts.Show(Resources.Strings.AppResources.Wrong_QRCode_Format);
                    return;
                }

                if (!string.IsNullOrEmpty(strSplit[0]))
                {
                    var strSplitZero = strSplit[0].Split("&");
                    if (strSplitZero != null)
                    {
                        if (strSplitZero.Length == 3)
                        {
                            var acCode = strSplitZero[0].Split("cq2faauth://totp/")[1].Split("?")[0];
                            var companyCode = strSplitZero[2].Split("companycode=")[1];

                            var secretKey = strSplitZero[0].Split("secret=")[1];
                            if (String.IsNullOrEmpty(YAP.Libs.Helpers.Globals.Base32Decode(secretKey)))
                            {
                                Toasts.Show(Resources.Strings.AppResources.Wrong_QRCode_Format);
                                return;
                            }

                            var obj = new Account
                            {
                                Accode = acCode.ToUpper(),
                                CompanyCode = companyCode,
                                SecretKey = secretKey.ToUpper()
                            };

                            var resBind = await Controllers.CQAuth.BindUserNewAccount(obj);
                            if (resBind)
                            {
                                Toasts.Show(Resources.Strings.AppResources.New_Account_Added);
                                if (!timer.Enabled)
                                {
                                    timer.Enabled = true;
                                }
                                endTime = DateTime.Now;
                            }
                            else
                            {
                                Toasts.Show(Resources.Strings.AppResources.New_Account_Add_Failed);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteDecodeScanResultCommand), ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
