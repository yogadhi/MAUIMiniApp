using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YAP.Libs.Logger;
using YAP.Libs.Views;
using ZXing.Net.Maui;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;

namespace YAP.Libs.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        bool _IsRefreshing = false;
        public bool IsRefreshing
        {
            get { return _IsRefreshing; }
            set { SetProperty(ref _IsRefreshing, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        Boolean _IsDetecting = true;
        public Boolean IsDetecting
        {
            get { return _IsDetecting; }
            set
            {
                if (_IsDetecting != value)
                {
                    _IsDetecting = value;
                    OnPropertyChanged("IsDetecting");
                }
            }
        }

        INavigation _Navigation;
        public INavigation Navigation
        {
            get { return _Navigation; }
            set { SetProperty(ref _Navigation, value); }
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
                Log.Write(Log.LogEnum.Error, nameof(ExecuteOpenWebCommand), ex);
            }
        }

        ICommand _ScanQRCodeCommand;
        public ICommand ScanQRCodeCommand => _ScanQRCodeCommand ?? (_ScanQRCodeCommand = new Command(async () => await ExecuteScanQRCodeCommand()));
        async Task ExecuteScanQRCodeCommand()
        {
            try
            {
                var resPermission = await YAP.Libs.Helpers.Permission.CheckAndRequestCamera();
                if (resPermission == PermissionStatus.Granted)
                {
                    MainThread.BeginInvokeOnMainThread(async () => { await Application.Current.MainPage.Navigation.PushModalAsync(new ScanQRCodePage()); });
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteScanQRCodeCommand), ex);
            }
        }

        ICommand _ProcessScanQRCodeCommand;
        public ICommand ProcessScanQRCodeCommand => _ProcessScanQRCodeCommand ?? (_ProcessScanQRCodeCommand = new Command<BarcodeResult>(async (x) => await ExecuteProcessScanQRCode(x)));
        async Task ExecuteProcessScanQRCode(BarcodeResult scanResult)
        {
            try
            {
                if (scanResult != null)
                {
                    var format = scanResult.Format;
                    var val = scanResult.Value;
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        MainThread.BeginInvokeOnMainThread(async () => { await Application.Current.MainPage.Navigation.PopModalAsync(); });
                        WeakReferenceMessenger.Default.Send(new MyMessage(new MessageContainer { Key = "ScanResult", CustomObject = val }));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteProcessScanQRCode), ex);
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
