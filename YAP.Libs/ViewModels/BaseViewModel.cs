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

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
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
                    await Application.Current.MainPage.Navigation.PushAsync(new ScanQRCodePage());
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteScanQRCodeCommand), ex);
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
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
