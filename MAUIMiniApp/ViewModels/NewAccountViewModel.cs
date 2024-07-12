using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YAP.Libs.ViewModels;
using YAP.Libs.Logger;
using MAUIMiniApp.Models;
using MAUIMiniApp.Data;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using YAP.Libs.Alerts;
using MAUIMiniApp.Controllers;

namespace MAUIMiniApp.ViewModels
{
    public class NewAccountViewModel : BaseViewModel
    {
        #region Variables
        string _CompanyCode;
        public string CompanyCode
        {
            get => _CompanyCode;
            set => SetProperty(ref _CompanyCode, value);
        }

        string _AccountNo;
        public string AccountNo
        {
            get => _AccountNo;
            set => SetProperty(ref _AccountNo, value);
        }

        string _SecretKey;
        public string SecretKey
        {
            get => _SecretKey;
            set => SetProperty(ref _SecretKey, value);
        }
        #endregion

        public NewAccountViewModel()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(NewAccountViewModel), ex);
            }
        }

        #region Commands
        ICommand _AddNewAccountCommand;
        public ICommand AddNewAccountCommand => _AddNewAccountCommand ?? (_AddNewAccountCommand = new Command(async () => await ExecuteAddNewAccountCommand()));
        async Task ExecuteAddNewAccountCommand()
        {
            try
            {
                IsBusy = true;

                List<string> allFieldList = new List<string>();
                allFieldList.Add(AccountNo);
                allFieldList.Add(CompanyCode);
                allFieldList.Add(SecretKey);

                if (allFieldList.Any(x => string.IsNullOrEmpty(x)))
                {
                    Toasts.Show(Resources.Strings.AppResources.Fill_All_Fields);
                    return;
                }

                var obj = new Account
                {
                    Accode = AccountNo,
                    CompanyCode = CompanyCode,
                    SecretKey = SecretKey
                };

                var msg = string.Empty;
                var resBind = await CQAuth.BindUserNewAccount(obj);
                if (resBind)
                {
                    msg = Resources.Strings.AppResources.New_Account_Added;
                }
                else
                {
                    msg = Resources.Strings.AppResources.New_Account_Add_Failed;
                }

                Toasts.Show(msg);
                WeakReferenceMessenger.Default.Send(new MyMessage(new MessageContainer { Key = "ClosePopUp" }));
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteAddNewAccountCommand), ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
