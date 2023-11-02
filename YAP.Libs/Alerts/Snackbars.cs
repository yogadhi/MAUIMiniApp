using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace YAP.Libs.Alerts
{
    public class Snackbars
    {
        public static async void Show(string text, string actionButtonText = "OK", double durationSecond = 3)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.DimGray,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.LightGreen,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
                //CharacterSpacing = 0.5
            };

            Action action = async () => await Application.Current.MainPage.DisplayAlert("Snackbar ActionButton Tapped", "The user has tapped the Snackbar ActionButton", "OK");
            TimeSpan duration = TimeSpan.FromSeconds(durationSecond);

            var snackbar = Snackbar.Make(text, action, actionButtonText, duration, snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }
    }
}
