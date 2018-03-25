using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VocalUtau.Formats.Model.BaseObject
{
    public class DesCooler
    {
        private static byte[] _keyvi = new byte[] { 0x69, 0x6c, 0x6f, 0x76, 0x65, 0x78, 0x79, 0x21 };

        public static byte[] keyvi
        {
            get { return DesCooler._keyvi; }
            set { DesCooler._keyvi = value; }
        }
        private static string getKey(string SrcKey)
        {
            byte[] result = Encoding.Default.GetBytes(SrcKey);    //tbPass为输入密码的文本框
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string full=BitConverter.ToString(output).Replace("-", "");
            return ""+ full[ 4] + full[ 8] + full[12] + full[16] 
                     + full[20] + full[24] + full[28] + full[31];
        }
        public static CryptoStream CreateEncryptStream(string EncryptKey, Stream BasicStream)
        {
            var key = Encoding.UTF8.GetBytes(getKey(EncryptKey));
            var encry = new DESCryptoServiceProvider();
            CryptoStream cs = new CryptoStream(BasicStream, encry.CreateEncryptor(key, keyvi), CryptoStreamMode.Write);
            return cs;
        }

        public static CryptoStream CreateDecryptStream(string EncryptKey, Stream BasicStream)
        {
            var key = Encoding.UTF8.GetBytes(getKey(EncryptKey));
            var encry = new DESCryptoServiceProvider();
            CryptoStream cs = new CryptoStream(BasicStream, encry.CreateDecryptor(key, keyvi), CryptoStreamMode.Read);
            return cs;
        }
    }
}
