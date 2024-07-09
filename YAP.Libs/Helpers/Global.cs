using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
