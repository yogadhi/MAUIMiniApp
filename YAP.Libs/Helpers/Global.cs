using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YAP.Libs.Helpers
{
    public class Global
    {
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
