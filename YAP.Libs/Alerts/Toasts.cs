using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace YAP.Libs.Alerts
{
    public class Toasts
    {
        public static void Show(string text, ToastDuration duration = ToastDuration.Short)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            double fontSize = 14;

            var toast = Toast.Make(text, duration, fontSize);
            MainThread.BeginInvokeOnMainThread(async () => { await toast.Show(cancellationTokenSource.Token); });
        }
    }
}
