using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using YAP.Libs.Interfaces;
using YAP.Libs.Alerts;
using MAUIMiniApp.Views;
using MAUIMiniApp.Data;
using MAUIMiniApp.ViewModels;
using ZXing.Net.Maui.Controls;
using YAP.Libs.Logger;

namespace MAUIMiniApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            try
            {
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
                    })
                    .UseMauiCommunityToolkit();

                builder.UseSentry(options =>
                {
                    options.Dsn = "https://2fc98bf77f8f2bcd4e24a7080809cc03@o4507655734493184.ingest.us.sentry.io/4507655738687488";
                    options.Debug = true;
                    options.TracesSampleRate = 1.0;
                    options.ProfilesSampleRate = 1.0;
                    options.MaxBreadcrumbs = 1000;
                });

#if DEBUG
                builder.Logging.AddDebug();
#endif

                //                Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
                //                {
                //#if ANDROID
                //                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                //#endif
                //                });
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(CreateMauiApp), ex);
            }

            return builder.Build();
        }
    }
}