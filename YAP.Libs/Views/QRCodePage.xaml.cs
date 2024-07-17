using CommunityToolkit.Maui.Views;
using YAP.Libs.Helpers;

namespace YAP.Libs.Views;

public partial class QRCodePage : Popup
{
    public enum QRCodeMode
    {
        Image,
        QRCode
    }

    public QRCodePage(QRCodeMode mode, string value)
    {
        try
        {
            InitializeComponent();
            Globals.InitPopUpPageDisplay(mainFrame, this, btnClose, true);

            if (mode == QRCodeMode.Image)
            {
                imgQRCode.IsVisible = true;
                barcodeGenerator.IsVisible = false;

                var imageBytes = Convert.FromBase64String(value);

                MemoryStream imageDecodeStream = new(imageBytes);
                imgQRCode.Source = ImageSource.FromStream(() => imageDecodeStream);
            }
            else if (mode == QRCodeMode.QRCode)
            {
                imgQRCode.IsVisible = false;
                barcodeGenerator.IsVisible = true;
                barcodeGenerator.Value = value;
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(QRCodePage), ex);
        }
    }

    private void btnClose_Clicked(object sender, EventArgs e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(async () => { await CloseAsync(); });
        }
        catch (Exception ex)
        {
            Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(btnClose_Clicked), ex);
        }
    }
}