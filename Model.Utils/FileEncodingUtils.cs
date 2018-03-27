using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mozilla.NUniversalCharDet;

namespace VocalUtau.Formats.Model.Utils
{
    public class FileEncodingUtils
    {
        public static Encoding GetEncoding(string File)
        {
            Encoding enc = Encoding.Default;
            System.IO.FileStream fs = new System.IO.FileStream(File, System.IO.FileMode.Open);
            enc = GetEncoding(fs);
            fs.Close();
            return enc;
        }
        public static Encoding GetEncoding(Stream DataStream)
        {
            string EncStr = "default";
            Encoding enc = Encoding.Default;
            long pos = DataStream.Position;
            DataStream.Seek(0, SeekOrigin.Begin);
            byte[] DetectBuff=new byte[8192];
            int RLen=DataStream.Read(DetectBuff, 0, 8192);
            EncStr = DetectEncoding_Bytes(DetectBuff, RLen);
            DataStream.Seek(pos, SeekOrigin.Begin);
            switch (EncStr) 
            {
                case "SHIFT_JIS": return Encoding.GetEncoding("Shift-JIS"); break;
                case "GB18030": return Encoding.GetEncoding("GB18030"); break;
                case "GB2312": return Encoding.GetEncoding("GB2312"); break;
                case "HZ-GB-2312": return Encoding.GetEncoding("GB2312"); break;
                case "BIG5": return Encoding.GetEncoding("BIG5"); break;
                case "UTF-8": return Encoding.UTF8;
                default: try { return Encoding.GetEncoding(EncStr); }
                    catch { return Encoding.Default; }
            }
        }

        public static string DefaultToEncoding(string String,Encoding Encoding)
        {
            byte[] bt = System.Text.Encoding.Default.GetBytes(String);
            string Str = Encoding.GetString(bt);
            return Str;
        }
        public static string EncodingToDefault(string String, Encoding Encoding)
        {
            byte[] bt = Encoding.GetBytes(String);
            string Str = System.Text.Encoding.Default.GetString(bt);
            return Str;
        }

        private static string DetectEncoding_Bytes(byte[] DetectBuff,int DetectLen)
        {
            UniversalDetector Det = new UniversalDetector(null);
            Det.HandleData(DetectBuff, 0, DetectLen);
            Det.DataEnd();
            if (Det.GetDetectedCharset() != null)
            {
                return Det.GetDetectedCharset();
            }
            return "default";
        }
    }
}
