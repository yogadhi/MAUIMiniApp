using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using YAP.Libs.Logger;
using MAUIMiniApp.ViewModels;
using Microsoft.Maui.Controls;
using MAUIMiniApp.Controllers;

namespace MAUIMiniApp.Views;

public partial class ToSPage : ContentPage
{
    ToSViewModel vm;

    public ToSPage()
    {
        try
        {
            InitializeComponent();
            vm = BindingContext as ToSViewModel;
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(ToSPage), ex);
        }
    }

    protected override async void OnAppearing()
    {
        try
        {
            base.OnAppearing();
            vm.LoadCommand.Execute(null);
            var deviceID = new GetDeviceInfo().GetDeviceID();

            var res = await CQAuth.Authenticate(new Models.ReqAuthenticate
            {
                UserLogin = "CQApp",
                Password = "cq2fa02pws",
                Accode = "asdfgh",
                CompanyCode = "1",
            });
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OnAppearing), ex);
        }
    }

    private void btnReject_Clicked(object sender, EventArgs e)
    {
        try
        {
            Application.Current.Quit();
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnReject_Clicked), ex);
        }
    }
}