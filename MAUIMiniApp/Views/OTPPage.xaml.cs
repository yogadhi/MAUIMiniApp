using MAUIMiniApp.ViewModels;
using YAP.Libs.Logger;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using MAUIMiniApp.Models;

namespace MAUIMiniApp.Views;
//https://svgtrace.com/svg-to-png

public partial class OTPPage : ContentPage
{
    OTPViewModel vm { get; set; }

    public OTPPage()
    {
        try
        {
            InitializeComponent();
            vm = BindingContext as OTPViewModel;

            ToolbarItem item = new ToolbarItem
            {
                Text = "Add Account",
                Order = ToolbarItemOrder.Primary,
                IconImageSource = ImageSource.FromFile("add_unpressed.png"),
                Command = vm.NewAccountCommand
            };

            ToolbarItems.Add(item);

            //vm.CheckToSCommand.Execute(null);

            WeakReferenceMessenger.Default.Register<MyMessage>(this, async (r, m) =>
            {
                if (m.Value != null)
                {
                    if (m.Value.Key == "ClosePopUp")
                    {
                        vm.timer.Stop();
                        vm.LoadCommand.Execute(null);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OTPPage) + " - " + ex.Message);
        }
    }

    protected override void OnAppearing()
    {
        try
        {
            base.OnAppearing();
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OnAppearing) + " - " + ex.Message);
        }
    }
}