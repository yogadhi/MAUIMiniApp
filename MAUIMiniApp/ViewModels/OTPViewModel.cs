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
                if (IsBusy)
                    return;

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
                            foreach (var index in listDistinct.ToList())
                            {
                                var objAccount = await AccountDatabase.GetItemByAccodeAsync(index);
                                if (objAccount == null)
                                    continue;

                                var checkBind = await CQAuth.CheckUserBinded(objAccount);
                                if (checkBind != 0)
                                    continue;

                                var resDelete = await AccountDatabase.DeleteItemAsync(objAccount);
                                if (resDelete != 1)
                                    continue;

                                listDistinct.Remove(index);
                                if (listDistinct.Count > 0)
                                {
                                    await SecureStorage.SetAsync("lastUnbindAccount", string.Join("|", listDistinct));
                                }
                                else
                                {
                                    SecureStorage.Remove("lastUnbindAccount");
                                }
                            }
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

                await Task.Delay(100);

                if (OTPItemList == null)
                    return;

                if (OTPItemList.Count == 0)
                    return;

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

                if (obj == null)
                    return;

                var account = await AccountDatabase.GetItemByAccodeAsync(obj.Account);
                if (account == null)
                    return;

                List<string> menuList = new List<string>
                {
                    "Copy", "Rename", "Remove", "Unbind"
                };

#if DEBUG
                menuList.Add("Show QR Code");
#endif

                menuList.Sort();

                var action = await Application.Current.MainPage.DisplayActionSheet(obj.Account, "Cancel", null, menuList.ToArray());
                if (string.IsNullOrEmpty(action))
                    return;

                if (action == "Copy")
                {
                    await Clipboard.Default.SetTextAsync(obj.OTP);
                    Toasts.Show(obj.OTP + " copied");
                }
                else if (action == "Remove")
                {
                    var remove = await App.AlertSvc.ShowConfirmationAsync("Remove Account", "Are you sure you want to remove account " + obj.Account, "Yes", "No");
                    if (!remove)
                        return;

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

                    var remove = await App.AlertSvc.ShowConfirmationAsync("Unbind Account", "Are you sure you want to unbind account " + obj.Account, "Yes", "No");
                    if (!remove)
                        return;

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
                        List<string> lastUnbindAccountList = new List<string>
                        {
                            strLastUnbindAccount,
                            obj.Account
                        };
                        await SecureStorage.SetAsync("lastUnbindAccount", string.Join("|", lastUnbindAccountList));
                    }
                    else
                    {
                        await SecureStorage.SetAsync("lastUnbindAccount", obj.Account);
                    }

                    Uri uri = new Uri(String.Format("https://cq2fa.cyberquote.com.hk/registration//Unbind?CompanyCode={0}&lang={1}&Accode={2}", account.CompanyCode, Lang, account.Accode));
                    var boolBrowser = await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                    if (boolBrowser)
                    {
                        var resUnbind = await CQAuth.Delink(account, new ReqDelink { SysID = Convert.ToInt32(account.CompanyCode), Username = account.Accode });
                        if (resUnbind != null)
                        {
                            if (resUnbind.data)
                            {
                                endTime = DateTime.Now;
                            }
                        }
                    }
                }
                else if (action == "Show QR Code")
                {
                    IsBusy = true;

                    var resQRCode = await Controllers.CQAuth.GetQRCode(account, new Models.ReqGetQRCode { SysID = Convert.ToInt32(account.CompanyCode), Username = account.Accode });
                    if (resQRCode == null)
                        return;

                    if (resQRCode.data == null)
                        return;

                    if (string.IsNullOrEmpty(resQRCode.data.Based64QRImg))
                        return;

                    IsBusy = false;

                    var strSplit = resQRCode.data.Based64QRImg.Split("data:image/png; base64,");
                    await Application.Current.MainPage.ShowPopupAsync(new YAP.Libs.Views.QRCodePage(YAP.Libs.Views.QRCodePage.QRCodeMode.Image, strSplit[1]));
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteSelectionCommand), ex);
            }
            finally
            {
                SelectedOTP = null;
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
                    if (IsBusy)
                        return;

                    if (OTPItemList == null)
                        return;

                    if (OTPItemList.Count == 0)
                        return;

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
