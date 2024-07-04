using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using YAP.Libs.Interfaces;
using YAP.Libs.Alerts;
using MAUIMiniApp.Views;
using MAUIMiniApp.Data;
using MAUIMiniApp.ViewModels;
using ZXing.Net.Maui.Controls;

namespace MAUIMiniApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Services.AddSingleton<IAlertService, AlertService>();

            builder.ConfigureEssentials(essentials =>
            {
                essentials.UseVersionTracking();
            });

            builder.UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
            });

            return builder.Build();
        }
    }
}