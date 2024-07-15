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
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var leave = await DisplayAlert("Exit Application?", "Are you sure you want to exit application?", "Yes", "No");
                if (leave)
                {
                    Application.Current.Quit();
                }
            });
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
            Application.Current.Quit();
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnReject_Clicked), ex);
        }
    }
}