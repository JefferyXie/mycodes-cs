using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyPlainAPI.Utils
{
    public static class StringUtils
    {
        public const string Dash = "-";

        /// <summary>
        /// Check input id is valid guid format or not
        /// </summary>
        /// <param name="id">input text value</param>
        /// <returns></returns>
        internal static bool IsValidGUID(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Regex reg = new Regex(@"^[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$", RegexOptions.IgnoreCase);
                return reg.IsMatch(id);
            }

            return false;
        }

        /// <summary>
        /// Check input id is empty guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static bool IsEmptyGuid(string id)
        {
            return id == Guid.Empty.ToString();
        }

        /// <summary>
        /// Check input string is valid numberic
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static bool IsNumberic(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            else
            {
                double result;
                return double.TryParse(Trim(input), out result);
                //// Checks that an input string is a decimal number, with an optional +/- sign character.
                //Regex reg = new Regex(@"^\s*(\+|-)?((\d+(\.\d+)?)|(\.\d+))\s*$");
                ////Regex reg = new Regex(@"^[\d]+$");
                //return reg.IsMatch(Trim(input));
            }
        }

        /// <summary>
        /// ignore key case to get value
        /// </summary>
        /// <param name="queryStrings"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static string GetQueryString(IEnumerable<KeyValuePair<string, string>> queryStrings, string key, string defaultValue)
        {
            string data = defaultValue;

            if (queryStrings != null)
            {
                var res = queryStrings.FirstOrDefault(kv => string.Equals(kv.Key, key, StringComparison.OrdinalIgnoreCase));
                if (!res.Equals(default(KeyValuePair<string, string>)))
                {
                    data = res.Value;
                }
            }

            return data;
        }

        /// <summary>
        /// Get Value from IDictionary<string, string> 
        /// </summary>
        /// <param name="data">mapping data</param>
        /// <param name="key">key of data</param>
        /// <param name="defaultValue">default is null</param>
        /// <param name="isNumberic">is numberic format</isNumberic>
        /// <returns></returns>
        internal static string GetValue(IDictionary<string, string> data, string key, string defaultValue, bool isNumberic = false)
        {
            string value;
            if (data.TryGetValue(key, out value))
            {
                if (isNumberic)
                {
                    return IsNumberic(value) ? value : defaultValue;
                }
                else
                {
                    return value;
                }
            }

            return defaultValue;
        }

        internal static bool ContainsIgnoreCase(string strA, string strB)
        {
            if (string.IsNullOrEmpty(strA) || string.IsNullOrEmpty(strB))
            {
                return false;
            }
            else
            {
                return strA.IndexOf(strB, StringComparison.OrdinalIgnoreCase) != -1;
            }
        }

        internal static bool StartsWithIgnoreCase(string strA, string strB)
        {
            Debug.Assert(strA != null);

            return strA.StartsWith(strB, StringComparison.OrdinalIgnoreCase);
        }

        internal static bool EndsWithIgnoreCase(string strA, string strB)
        {
            Debug.Assert(strA != null);

            return strA.EndsWith(strB, StringComparison.OrdinalIgnoreCase);
        }

        internal static bool EqualsIgnoreCase(string strA, string strB)
        {
            return (string.Compare(strA, strB, true, CultureInfo.InvariantCulture) == 0);
        }

        internal static bool IsTrue(string text)
        {
            return !string.IsNullOrEmpty(text)
                && (text == "1" || string.Compare(text, "true", CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase) == 0);
        }

        internal static string Trim(string text)
        {
            return string.IsNullOrEmpty(text) ? text : text.Trim();
        }

        internal static string CalculateMD5(string text)
        {
            byte[] originalBytes = Encoding.UTF8.GetBytes(text);

            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                md5.Initialize();
                byte[] encodedBytes = md5.ComputeHash(originalBytes);
                // Convert encoded bytes back to a 'readable' string
                return BitConverter.ToString(encodedBytes);
            }
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        internal static string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, input);
            }
        }

        internal static string GetMd5HashUpper(string input)
        {
            return GetMd5Hash(input).ToUpperInvariant();
        }

        internal static IList<string> Split(string text, string separator)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(separator))
            {
                return null;
            }

            List<string> list = new List<string>();
            int pos = 0;
            while (pos != -1)
            {
                pos = text.IndexOf(separator);
                if (pos != -1)
                {
                    int index = pos + separator.Length;

                    list.Add(text.Substring(0, index));

                    text = text.Substring(index);
                }
            }

            //add last 
            if (!string.IsNullOrEmpty(text))
            {
                list.Add(text);
            }

            return list;
        }

        internal static string RemoveWhiteChars(string inputString)
        {
            if (inputString == null)
                return null;

            return Regex.Replace(inputString, @"[\s]+", "");
        }

        internal static bool IsHttpShortDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return false;
            }
            else
            {
                Regex reg = new Regex(@"^(19|20)\d\d([- /.])(0[1-9]|1[012])\2(0[1-9]|[12][0-9]|3[01])$");
                return reg.IsMatch(Trim(date));
            }
        }

        internal static string GetShortId(string idWithUniverse)
        {
            if (string.IsNullOrWhiteSpace(idWithUniverse))
            {
                return idWithUniverse;
            }
            string[] splitStrings = idWithUniverse.Split(new char[] { ';' });
            if (splitStrings.Length > 0)
            {
                return splitStrings[0];
            }
            return null;
        }

    }
}
