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
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OTPPage) + " - " + ex.Message);
        }
    }
}