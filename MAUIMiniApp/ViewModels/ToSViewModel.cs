using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YAP.Libs.ViewModels;
using YAP.Libs.Logger;
using MAUIMiniApp.Models;
using MAUIMiniApp.Data;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using YAP.Libs.Alerts;
using CommunityToolkit.Maui.Views;
using YAP.Libs.Views;
using MAUIMiniApp.Controllers;

namespace MAUIMiniApp.ViewModels
{
    public class ToSViewModel : BaseViewModel
    {
        #region Variables
        HtmlWebViewSource _HtmlSource;
        public HtmlWebViewSource HtmlSource
        {
            get => _HtmlSource;
            set => SetProperty(ref _HtmlSource, value);
        }
        #endregion

        public ToSViewModel(INavigation navigation)
        {
            try
            {
                Navigation = navigation;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ToSViewModel), ex);
            }
        }

        #region Commands
        ICommand _AcceptToSCommand;
        public ICommand AcceptToSCommand => _AcceptToSCommand ?? (_AcceptToSCommand = new Command(async () => await ExecuteAcceptToSCommand()));
        async Task ExecuteAcceptToSCommand()
        {
            try
            {
                await SecureStorage.SetAsync("hasAcceptToS", "true");
                await YAP.Libs.Helpers.NavigationServices.PopModalAsync(Navigation);

                //if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
                //{
                //    await YAP.Libs.Helpers.NavigationServices.PopAsync(Navigation, true);
                //}
                //else if (DeviceInfo.Current.Platform == DevicePlatform.Android || DeviceInfo.Current.Platform == DevicePlatform.iOS)
                //{
                //    await YAP.Libs.Helpers.NavigationServices.PopModalAsync(Navigation, true);
                //}
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteAcceptToSCommand), ex);
            }
        }

        ICommand _LoadCommand;
        public ICommand LoadCommand => _LoadCommand ?? (_LoadCommand = new Command(async () => await ExecuteLoadCommand()));
        async Task ExecuteLoadCommand()
        {
            try
            {
                HtmlSource = new HtmlWebViewSource();
                string html = "<HTML><body style=\"{htmlTheme}\"> \r\n<h2><strong>Terms and Condition for Mobile Security Key for Online Trading Services</strong></h2>\r\n<p>&nbsp;</p>\r\n<p><strong>PLEASE READ AND UNDERSTAND THESE TERMS AND CONDITIONS BEFORE YOU REGISTER FOR MOBILE SECURITY KEY AUTHENTICATION FOR INVESTMENT SERVICES. IF YOU DO NOT ACCEPT THESE TERMS AND CONDITIONS, PLEASE DO NOT REGISTER FOR MOBILE SECURITY KEY AUTHENTICATION.</strong></p>\r\n<p>&nbsp;</p>\r\n<ol>\r\n<li><strong>Definitions and Interpretation</strong></li>\r\n</ol>\r\n<p>In these Terms and Conditions, the following words shall have the following meanings:</p>\r\n<p>&nbsp;</p>\r\n<p>&ldquo;CQ&rdquo; means&nbsp;CyberQuote (HK) Limited；</p>\r\n<p>&nbsp;</p>\r\n<p>&ldquo;CyberQuote (HK) Limited&rdquo; means a&nbsp;wholly-owned subsidiary of&nbsp;Phillip Securities (Hong Kong) Limited&nbsp;</p>\r\n<p>&nbsp;</p>\r\n<p>&ldquo;You&rdquo; means the user of CQ Authenticator application;</p>\r\n<p>&nbsp;</p>\r\n<p>&ldquo;App&rdquo; means the CQ Authenticator application (as updated from time to time) which can be downloaded to any mobile device which runs an operating system supported by us;</p>\r\n<p>&nbsp;</p>\r\n<p>\"Mobile Security Key\" means a feature within the App which is a software based Security Device used to generate a one-time Security Code, as CQ&nbsp;may provide from time to time pursuant to these Terms and Conditions;</p>\r\n<p>&nbsp;</p>\r\n<p>\"Permitted Mobile Device\" means such Apple device and compatible Android device running an operating system version as CQ&nbsp;specify from time to time, or any other electronic devices or equipment which CQ&nbsp;may enable for using Mobile Security Key Authentication from time to time; and</p>\r\n<p>&nbsp;</p>\r\n<p>&nbsp;</p>\r\n<ol start=\"2\">\r\n<li><strong>What is 2FA Login?</strong></li>\r\n</ol>\r\n<p>Two-factor authentication (2FA) strengthens cybersecurity by requiring a combination of two different types of authentication to login to your online trading account. The first layer of authentication is to provide the login ID and password followed by the second layer, using the &ldquo;Mobile Security Key&rdquo; to confirm a login request.</p>\r\n<p>&nbsp;</p>\r\n<p>&nbsp;</p>\r\n<ol start=\"3\">\r\n<li><strong>Mobile Security Key</strong></li>\r\n</ol>\r\n<p>3.1 Mobile Security Key provides an alternative means of verifying your identity for accessing&nbsp;CQ&nbsp;approved online trading services. You may register the Mobile Security Key on your Permitted Mobile Device by completing the steps specified by CQ. Once successfully registered, you have to use Mobile Security Key to generate a one-time Security Code as a second verification to confirm your identity for accessing CQ&nbsp;approved online trading services.</p>\r\n<p>&nbsp;</p>\r\n<p>3.2 Our provision and your use of Mobile Security Key are subject to these Terms and Conditions. Once you register for Mobile Security Key, you will be regarded as having accepted and will be bound by these Terms and Conditions. If you do not accept these Terms and Conditions, please do not register for Mobile Security Key.</p>\r\n<p>&nbsp;</p>\r\n<p>3.3 CQ has the right to specify or vary from time to time the scope and features of Mobile Security Key without prior notice.</p>\r\n<p>&nbsp;</p>\r\n<p>3.4 You can only set up a Mobile Security Key on one Permitted Mobile Device at a time.</p>\r\n<p>&nbsp;</p>\r\n<ol start=\"4\">\r\n<li><strong>Your confirmation and responsibility</strong></li>\r\n</ol>\r\n<p>4.1 You confirm and authorize us to verify your identity by the Mobile Security Key registered on your Permitted Mobile Device as a second verification for logon CQ&nbsp;approved online trading platform or performing online transactions.</p>\r\n<p>&nbsp;</p>\r\n<p>4.2 In order to use Mobile Security Key: (a) you must be a valid user of the CQ&nbsp;approved online trading services; and (b) you must install the App using your Permitted Mobile Device.</p>\r\n<p>&nbsp;</p>\r\n<p>4.3 You should take all reasonable security measures to prevent unauthorized or fraudulent use of Mobile Security Key, including the following measures: (a) you should take reasonable precautions to keep safe and prevent loss or fraudulent use of your Permitted Mobile Device, login username and password, and the Mobile Security Key. You should observe the security recommendations provided by us from time to time about the use of Mobile Security Key; (b) you must not use the App or Mobile Security Key on any mobile device or operating system that has been modified outside the mobile device or operating system vendor supported or warranted configurations. This includes devices that have been \"jail-broken\" or \"rooted\". A jail-broken or rooted device means one that has been freed from the limitations imposed on it by your mobile service provider and the phone manufacturer without their approval. The use of the App or Mobile Security Key on a jail-broken or rooted device may compromise security and lead to fraudulent transactions. Download and use of the App or Mobile Security Key in a jail-broken or rooted device is entirely at your own risk and CQ&nbsp;will not be liable for any losses or any other consequences suffered or incurred by you as a result; (c) if you are aware of or suspect any unauthorized use of your Permitted Mobile Device or login username and password or the Mobile Security Key, you should notify us as soon as reasonably practicable.&nbsp;CQ&nbsp;may require you to change your username and password, re-register Mobile Security Key, or cease to use Mobile Security Key.</p>\r\n<p>&nbsp;</p>\r\n<p>4.4 All instructions received by us with your identity verified through the Mobile Security Key shall be binding on you.</p>\r\n<p>&nbsp;</p>\r\n<ol start=\"5\">\r\n<li><strong>Limitation of our liability</strong></li>\r\n</ol>\r\n<p>5.1 Mobile Security Key is provided on an \"as is\" and \"as available\" basis. CQ does not warrant that Mobile Security Key will be available at all times, or that it will function with any electronic equipment, software or system or other services that CQ may offer from time to time.</p>\r\n<p>&nbsp;</p>\r\n<p>5.2 CQ is not liable for any loss, damages or expenses of any kind incurred or suffered by you arising from or in connection with your use of or inability to use Mobile Security Key unless it is caused solely and directly by the willful default on our part or on the part of our employees or agents.</p>\r\n<p>&nbsp;</p>\r\n<p>5.3 Under no circumstances CQ is liable for any indirect, special, incidental, consequential, punitive or exemplary loss or damages, including loss of profits, loss due to business interruption or loss of any programme or data in your Permitted Mobile Device.</p>\r\n<p>&nbsp;</p>\r\n<ol start=\"6\">\r\n<li><strong>Modification, suspension and termination of Mobile Security Key Authentication</strong></li>\r\n</ol>\r\n<p>CQ has the right to modify, suspend or terminate Mobile Security Key or its use by you at any time without giving prior notice or reason where CQ&nbsp;reasonably consider necessary or advisable to do so. These cases may include actual or suspected breach of security.</p>\r\n<p>&nbsp;</p>\r\n<ol start=\"7\">\r\n<li><strong>Revision of these Terms and Conditions</strong></li>\r\n</ol>\r\n<p>CQ has the right to revise these Terms and Conditions and/or introduce additional terms and conditions (including fees and charges) from time to time by giving prior notice. CQ may give notice by display, advertisement or other means as CQ consider appropriate. You will be bound by any variation if you use Mobile Security Key on or after the effective date of the variation.</p>\r\n<p>&nbsp;</p>\r\n<ol start=\"8\">\r\n<li><strong>Miscellaneous</strong></li>\r\n</ol>\r\n<p>8.1 Each provision of these Terms and Conditions is severable from the others. If at any time any provision is or becomes illegal, invalid or unenforceable in any respect under Hong Kong law or the laws of any other jurisdiction, the legality, validity or enforceability of the remaining provisions shall not be affected in any way.</p>\r\n<p>&nbsp;</p>\r\n<p>8.2 CQ may assign or transfer all or any of our rights and obligations under these Terms and Conditions to any member of Phillip Group without your prior consent.</p>\r\n<p>&nbsp;</p>\r\n<p>8.3 The English version of these Terms and Conditions shall prevail wherever there is any inconsistency between the English and the Chinese versions.</p>\r\n<p>&nbsp;</p>\r\n<p>&nbsp;</p>\r\n</body></HTML>";

                if (Application.Current.UserAppTheme == AppTheme.Dark || Application.Current.UserAppTheme == AppTheme.Unspecified)
                {
                    html = html.Replace("{htmlTheme}", "background-color:black; color:white");
                }
                else if (Application.Current.UserAppTheme == AppTheme.Light)
                {
                    html = html.Replace("{htmlTheme}", "background-color:white; color:black");
                }

                HtmlSource.Html = html;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ExecuteLoadCommand), ex);
            }
        }
        #endregion
    }
}
