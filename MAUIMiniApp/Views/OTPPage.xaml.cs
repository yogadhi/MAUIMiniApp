using System.Drawing;
using YAP.Libs.Logger;

namespace MAUIMiniApp.Views;

public partial class OTPPage : ContentPage
{
    public OTPPage()
    {
        try
        {
            InitializeComponent();

            ToolbarItem item = new ToolbarItem
            {
                Text = "Add Account",
                Order = ToolbarItemOrder.Primary,
                IconImageSource = ImageSource.FromFile("account_plus_outline.svg")
            };

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                //https://svgtrace.com/svg-to-png
                item.IconImageSource = ImageSource.FromFile("account_plus_outline.png");
            }

            this.ToolbarItems.Add(item);
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OTPPage) + " - " + ex.Message);
        }
    }
}