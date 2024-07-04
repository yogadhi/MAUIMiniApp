using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YAP.Libs.Alerts;
using YAP.Libs.Flyouts;
using YAP.Libs.Interfaces;
using YAP.Libs.Logger;
using YAP.Libs.Models;

namespace YAP.Libs.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        IAlertService _AlertSvc;
        public IAlertService AlertSvc
        {
            get { return _AlertSvc; }
            set
            {
                if (_AlertSvc != value)
                {
                    _AlertSvc = value;
                    OnPropertyChanged("AlertSvc");
                }
            }
        }

        RootItem _RootItem;
        public RootItem RootItem
        {
            get { return _RootItem; }
            set
            {
                if (_RootItem != value)
                {
                    _RootItem = value;
                    OnPropertyChanged("RootItem");
                }
            }
        }

        string _Username;
        public string Username
        {
            get { return _Username; }
            set
            {
                if (_Username != value)
                {
                    _Username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        public LoginViewModel()
        {

        }

        ICommand _LoginCommand;
        public ICommand LoginCommand => _LoginCommand ?? (_LoginCommand = new Command(async () => await ExecuteLoginCommand()));
        async Task ExecuteLoginCommand()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;
                await Task.Delay(3000);

                if (Username == "admin" && Password == "123456")
                {
                    Toasts.Show("Login success");
                    await SecureStorage.SetAsync("hasAuth", "true");

                    var hasAcceptToS = await SecureStorage.GetAsync("hasAcceptToS");
                    if (string.IsNullOrEmpty(hasAcceptToS))
                    {
                        WeakReferenceMessenger.Default.Send(new MyMessage(new MessageContainer { Key = "hasAcceptToS" }));
                    }
                    else
                    {
                        Application.Current.MainPage = new AppFlyout(RootItem);
                    }
                }
                else
                {
                    await AlertSvc.ShowAlertAsync("Login failed", "Username or password is invalid", "Try again");
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteLoginCommand), ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
