using YAP.Libs.Interfaces;
using YAP.Libs.Models;
using YAP.Libs.ViewModels;

namespace YAP.Libs.Views;

public partial class LoginPage : ContentPage
{
    LoginViewModel vm { get; set; }

    public LoginPage(RootItem rootItem)
	{
		InitializeComponent();
        vm = BindingContext as LoginViewModel;

        vm.RootItem = rootItem;
        vm.AlertSvc = vm.RootItem.Provider.GetService<IAlertService>();
    }

    protected override bool OnBackButtonPressed()
    {
        Application.Current.Quit();
        return true;
    }
}