using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAP.Libs.Logger;

namespace YAP.Libs.Helpers
{
    public class NavigationServices
    {
        public static async Task<bool> PushModalAsync(INavigation navigation, Page page, bool animated = true)
        {
            try
            {
                if (navigation == null)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await navigation.PushModalAsync(page, animated);
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(PushModalAsync), ex);
                return false;
            }
        }

        public static async Task<bool> PopModalAsync(INavigation navigation, bool animated = true)
        {
            try
            {
                if (navigation == null)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await navigation.PopModalAsync(animated);
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(PopModalAsync), ex);
                return false;
            }
        }

        public static async Task<bool> PushAsync(INavigation navigation, Page page, bool animated = true)
        {
            try
            {
                if (navigation == null)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await navigation.PushAsync(page, animated);
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(PushAsync), ex);
                return false;
            }
        }

        public static async Task<bool> PopAsync(INavigation navigation, bool animated = true)
        {
            try
            {
                if (navigation == null)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await navigation.PopAsync(animated);
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(PopAsync), ex);
                return false;
            }
        }

        public static async Task<bool> PopToRootAsync(INavigation navigation, bool animated = true)
        {
            try
            {
                if (navigation == null)
                {
                    return false;
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await navigation.PopToRootAsync(animated);
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(PopToRootAsync), ex);
                return false;
            }
        }
    }
}
