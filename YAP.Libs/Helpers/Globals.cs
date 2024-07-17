using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using YAP.Libs.Logger;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using System.Text.RegularExpressions;

namespace YAP.Libs.Helpers
{
    public class Globals
    {
        public static void NumericOnly_TextChanged(object sender, TextChangedEventArgs e)
        {
            // If the text field is empty or null then leave.
            string regex = e.NewTextValue;
            if (String.IsNullOrEmpty(regex))
                return;

            // If the text field only contains numbers then leave.
            if (!Regex.Match(regex, "^[0-9]+$").Success)
            {
                // This returns to the previous valid state.
                var entry = sender as Entry;
                entry.Text = (string.IsNullOrEmpty(e.OldTextValue)) ? string.Empty : e.OldTextValue;
            }
        }

        public static void AlphabetOnly_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                var textFiltered = new string(e.NewTextValue.Where(char.IsLetter).ToArray());
                ((Entry)sender).Text = textFiltered;
            }
        }

        public static void InitPopUpPageDisplay(Frame main, Popup mainPopup, ImageButton imgBtnClose, bool isSquare = false)
        {
            double width = 0;

            try
            {
                if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
                {
                    imgBtnClose.Source = ImageSource.FromFile("close_light.png");
                    width = DeviceDisplay.Current.MainDisplayInfo.Width / (isSquare ? 6 : 4);
                    main.WidthRequest = width;
                    if (isSquare)
                    {
                        main.HeightRequest = width;
                    }
                }
                else
                {
                    width = DeviceDisplay.Current.MainDisplayInfo.Width / (isSquare ? 5 : 3);
                    mainPopup.Size = new Microsoft.Maui.Graphics.Size(width, isSquare ? width : 0);
                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(InitPopUpPageDisplay), ex);
            }
        }

        public static bool SetAppTheme()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var userAppTheme = await SecureStorage.GetAsync("userAppTheme");
                    if (!string.IsNullOrEmpty(userAppTheme))
                    {
                        if (userAppTheme == "Dark")
                        {
                            Application.Current.UserAppTheme = AppTheme.Dark;
                        }
                        else if (userAppTheme == "Light")
                        {
                            Application.Current.UserAppTheme = AppTheme.Light;
                        }
                    }
                    else
                    {
                        Application.Current.UserAppTheme = Application.Current.PlatformAppTheme;
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(SetAppTheme), ex);
                return false;
            }
        }
        public static Dictionary<string, string> GetVersionInfoList()
        {
            Dictionary<string, string> InfoList = new Dictionary<string, string>();

            try
            {
                var IsFirst = VersionTracking.Default.IsFirstLaunchEver.ToString();
                InfoList.Add("IsFirst", IsFirst);

                var CurrentVersionIsFirst = VersionTracking.Default.IsFirstLaunchForCurrentVersion.ToString();
                InfoList.Add("CurrentVersionIsFirst", CurrentVersionIsFirst);

                var CurrentBuildIsFirst = VersionTracking.Default.IsFirstLaunchForCurrentBuild.ToString();
                InfoList.Add("CurrentBuildIsFirst", CurrentBuildIsFirst);

                var CurrentVersion = VersionTracking.Default.CurrentVersion.ToString();
                InfoList.Add("CurrentVersion", CurrentVersion);

                var CurrentBuild = VersionTracking.Default.CurrentBuild.ToString();
                InfoList.Add("CurrentBuild", CurrentBuild);

                var FirstInstalledVer = VersionTracking.Default.FirstInstalledVersion.ToString();
                InfoList.Add("FirstInstalledVer", FirstInstalledVer);

                var FirstInstalledBuild = VersionTracking.Default.FirstInstalledBuild.ToString();
                InfoList.Add("FirstInstalledBuild", FirstInstalledBuild);

                var VersionHistory = String.Join(',', VersionTracking.Default.VersionHistory);
                InfoList.Add("VersionHistory", VersionHistory);

                var BuildHistory = String.Join(',', VersionTracking.Default.BuildHistory);
                InfoList.Add("BuildHistory", BuildHistory);

                // These two properties may be null if this is the first version
                var PreviousVersion = VersionTracking.Default.PreviousVersion?.ToString() ?? "none";
                InfoList.Add("PreviousVersion", PreviousVersion);

                var PreviousBuild = VersionTracking.Default.PreviousBuild?.ToString() ?? "none";
                InfoList.Add("PreviousBuild", PreviousBuild);
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(GetVersionInfoList), ex);
            }
            return InfoList;
        }

        private static byte[] Decode(String encoded)
        {
            Dictionary<char, int> CHAR_MAP = new Dictionary<char, int>();
            string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var DIGITS = ALPHABET.ToCharArray();
            int MASK = DIGITS.Length - 1;

            for (int i = 0; i < DIGITS.Length; i++)
            {
                CHAR_MAP[DIGITS[i]] = i;
            }

            // Remove whitespace and separators
            encoded = encoded.Trim().Replace("-", "").Replace(" ", "");

            // Remove padding. Note: the padding is used as hint to determine how many
            // bits to decode from the last incomplete chunk (which is commented out
            // below, so this may have been wrong to start with).
            encoded = encoded.Replace("[=]*$", "");

            // Canonicalize to all upper case
            encoded = encoded.ToUpper();

            if (encoded.Length == 0)
            {
                //return new byte[0];
                return Array.Empty<byte>();
            }

            int SHIFT = NumberOfTrailingZeros(32);
            int encodedLength = encoded.Length;
            int outLength = encodedLength * SHIFT / 8;
            byte[] result = new byte[outLength];
            int buffer = 0;
            int next = 0;
            int bitsLeft = 0;

            foreach (char c in encoded.ToCharArray())
            {
                if (!CHAR_MAP.ContainsKey(c))
                {
                    throw new Exception("Illegal character: " + c);
                }
                buffer <<= SHIFT;
                buffer |= CHAR_MAP[c] & MASK;
                bitsLeft += SHIFT;
                if (bitsLeft >= 8)
                {
                    result[next++] = (byte)(buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }
            // We'll ignore leftover bits for now.
            //
            // if (next != outLength || bitsLeft >= SHIFT) {
            //  throw new DecodingException("Bits left: " + bitsLeft);
            // }
            return result;
        }

        public static int NumberOfTrailingZeros(int num)
        {
            int left_part_zeros;

            if (num == 0)
                return 0;

            left_part_zeros = NumberOfTrailingZeros(num >> 1);
            if ((num & 1) == 0)
                return left_part_zeros + 1;
            else
                return left_part_zeros;
        }

        public static string Base32Decode(string data)
        {
            try
            {
                byte[] decode = Decode(data);
                String Result = Encoding.UTF8.GetString(decode);
                return Result;
            }
            catch (Exception ex)
            {
                Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(Base32Decode), ex);
                return string.Empty;
            }
        }

        public static string MaskString(string val)
        {
            try
            {
                val = val.ToUpper();
                var maskedString = val.Aggregate(string.Empty, (value, next) =>
                {
                    if (val.Length <= 5)
                    {
                        if (value.Length >= 1 && value.Length < val.Length - 1)
                        {
                            next = 'X';
                        }
                    }
                    else
                    {
                        if (value.Length >= 1 && value.Length < val.Length - 3)
                        {
                            next = 'X';
                        }
                    }
                    return value + next;
                });

                return maskedString;
            }
            catch (Exception ex)
            {
                Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(Base32Decode), ex);
                return val;
            }
        }

        #region OTP
        public static TimeSpan DefaultClockDriftTolerance { get; set; }
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static bool UseManagedSha1Algorithm { get; set; }
        public static bool TryUnmanagedAlgorithmOnFailure { get; set; }

        public static string GetFuturePIN(string privateKey)
        {
            string[] tempCodes = GetCurrentPINs(privateKey);
            return tempCodes[tempCodes.Length - 1];
        }

        public static string[] GetCurrentPINs(string accountSecretKey)
        {
            return GetCurrentPINs(accountSecretKey, DefaultClockDriftTolerance);
        }

        public static string[] GetCurrentPINs(string accountSecretKey, TimeSpan timeTolerance)
        {
            List<string> codes = new List<string>();
            long iterationCounter = GetCurrentCounter();
            int iterationOffset = 0;

            if (timeTolerance.TotalSeconds > 30)
            {
                iterationOffset = Convert.ToInt32(timeTolerance.TotalSeconds / 30.00);
            }


            //long iterationStart = iterationCounter;
            long iterationStart = iterationCounter - iterationOffset;
            long iterationEnd = iterationCounter + iterationOffset;

            for (long counter = iterationStart; counter <= iterationEnd; counter++)
            {
                codes.Add(GeneratePINAtInterval(accountSecretKey, counter));
            }

            return codes.ToArray();
        }

        private static long GetCurrentCounter()
        {
            return GetCurrentCounter(DateTime.UtcNow, _epoch, 30);
        }

        private static long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep)
        {
            return (long)(now - epoch).TotalSeconds / timeStep;
        }

        public static string GeneratePINAtInterval(string accountSecretKey, long counter, int digits = 6)
        {
            return GenerateHashedCode(accountSecretKey, counter, digits);
        }

        internal static string GenerateHashedCode(string secret, long iterationNumber, int digits = 6)
        {
            //byte[] key = Convert.FromBase64String(secret);

            byte[] key = Encoding.UTF8.GetBytes(secret);
            return GenerateHashedCode(key, iterationNumber, digits);
        }

        internal static string GenerateHashedCode(byte[] key, long iterationNumber, int digits = 6)
        {
            byte[] counter = BitConverter.GetBytes(iterationNumber);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counter);
            }

            HMACSHA1 hmac = GetHMACSha1Algorithm(key);

            byte[] hash = hmac.ComputeHash(counter);

            int offset = hash[hash.Length - 1] & 0xf;

            // Convert the 4 bytes into an integer, ignoring the sign.
            int binary =
                ((hash[offset] & 0x7f) << 24)
                | (hash[offset + 1] << 16)
                | (hash[offset + 2] << 8)
                | (hash[offset + 3]);

            int password = binary % (int)Math.Pow(10, digits);
            return password.ToString(new string('0', digits));
        }

        private static HMACSHA1 GetHMACSha1Algorithm(byte[] key)
        {
            HMACSHA1 hmac = null;

            try
            {
                hmac = new HMACSHA1(key, UseManagedSha1Algorithm);
            }
            catch (InvalidOperationException ioe)
            {
                if (UseManagedSha1Algorithm && TryUnmanagedAlgorithmOnFailure)
                {
                    try
                    {
                        hmac = new HMACSHA1(key, false);
                    }
                    catch (InvalidOperationException ioe2)
                    {
                        //throw ioe2;
                        var ex = ioe2.ToString();
                    }
                }
                else
                {
                    var ex = ioe.ToString();
                    //throw ioe;
                }
            }

            return hmac;
        }
        #endregion
    }
}
