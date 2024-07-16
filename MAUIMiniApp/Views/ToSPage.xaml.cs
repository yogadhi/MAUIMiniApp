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
            BindingContext = vm = new ToSViewModel(Navigation);
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(ToSPage), ex);
        }
    }

    protected override void OnAppearing()
    {
        try
        {
            base.OnAppearing();
            vm.LoadCommand.Execute(null);
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OnAppearing), ex);
        }
    }

    protected override bool OnBackButtonPressed()
    {
        try
        {
            Helpers.Globals.HandleExitApp();
            return true;
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OnBackButtonPressed), ex);
            return false;
        }
    }

    private void btnReject_Clicked(object sender, EventArgs e)
    {
        try
        {
            Helpers.Globals.HandleExitApp();
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnReject_Clicked), ex);
        }
    }
}