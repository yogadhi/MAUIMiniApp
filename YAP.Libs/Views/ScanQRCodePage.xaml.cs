namespace YAP.Libs.Views;

public partial class ScanQRCodePage : ContentPage
{
	public ScanQRCodePage()
	{
		InitializeComponent();
	}

    private void BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {

    }
}