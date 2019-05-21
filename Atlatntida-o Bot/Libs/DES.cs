using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Atlatntida_o_Bot.Libs
{
    class DES
    {
        private static Encoding _encoding;

        public static Encoding Encoding
        {
            get { return _encoding ?? (_encoding = Encoding.UTF8); }

            set { _encoding = value; }
        }

        /// <summary>
        /// 3DES ENCrypt
        /// </summary>
        /// <param name="strString">Строка шифрования</param>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public static string Encrypt3Des(string strString, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider
            {
                Key = Encoding.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros
            };

            ICryptoTransform desEncrypt = des.CreateEncryptor();

            byte[] buffer = _encoding.GetBytes(strString);

            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        /// <summary>
        /// 3DES DECrypt
        /// </summary>
        /// <param name="strString">Строка шифрования</param>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public static string Decrypt3Des(string strString, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros
            };

            ICryptoTransform desDecrypt = des.CreateDecryptor();

            byte[] buffer = Convert.FromBase64String(strString);
            return Encoding.UTF8.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }
    }
}
