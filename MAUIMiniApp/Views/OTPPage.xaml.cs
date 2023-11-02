using System.Diagnostics;

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
            Debug.WriteLine(ex);
        }
    }
}