using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using VocalUtau.Formats.Model.BaseObject;

namespace VocalUtau.Formats.Model.VocalObject
{
    public class ObjectSerializer<T>
    {
        public ObjectSerializer()
        {
        }
        private BasicFileInformation GetBasicFileInformation(T Object)
        {
            try
            {
                Type type = typeof(T);
                System.Reflection.PropertyInfo pi = type.GetProperty("BasicData");
                object res=pi.GetValue(Object, null);
                if (res is BasicFileInformation)
                {
                    return (BasicFileInformation)res;
                }
            }
            catch { ; }
            return new BasicFileInformation();
        }
        public string Serialize(T Object)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
            MemoryStream msObj = new MemoryStream();
            //将序列化之后的Json格式数据写入流中
            js.WriteObject(msObj, Object);
            msObj.Position = 0;
            //从0这个位置开始读取流中的数据
            StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
            string json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            return json;
        }
        public void WriteBasicInformation(T Object, Stream target)
        {
            BasicFileInformation bfi = GetBasicFileInformation(Object);
            byte[] itext=System.Text.UTF8Encoding.UTF8.GetBytes(bfi.IntroduceText);
            byte[] iver=System.Text.UTF8Encoding.UTF8.GetBytes(bfi.VersionString);

            string pwd = bfi.SavePassword;

            BinaryWriter sw = new BinaryWriter(target);
            sw.Write(new char[] { 'V', 'U', 'P', 'J' });
            sw.Write(iver.Length);
            sw.Write(iver);
            sw.Write(itext.Length);
            sw.Write(itext);
            sw.Write(pwd.Length > 0?1:0);
            sw.Write(new char[] { 'D', 'A', 'T', 'A' });
            sw.Flush();
        }
        public void SerializeToFile(T Object, string FileName)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
            FileStream msObj = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            BasicFileInformation bfi = GetBasicFileInformation(Object);
            string pwd = bfi.SavePassword;
            WriteBasicInformation(Object, msObj);
            if (pwd.Length > 0)
            {
                CryptoStream cs = DesCooler.CreateEncryptStream(pwd, msObj);
                js.WriteObject(cs, Object);
            }
            else
            {
                js.WriteObject(msObj, Object);
            }
            msObj.Close();
        }
        public void SerializeToZipFile(T Object, string FileName)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
            FileStream msObj = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            BasicFileInformation bfi = GetBasicFileInformation(Object);
            string pwd = bfi.SavePassword;
            WriteBasicInformation(Object, msObj);
            ICSharpCode.SharpZipLib.GZip.GZipOutputStream bzo;
            if (pwd.Length > 0)
            {
                CryptoStream cs = DesCooler.CreateEncryptStream(pwd, msObj);
                bzo = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(cs);
            }else
            {
                bzo = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(msObj);
            }
            js.WriteObject(bzo, Object);

            bzo.Close();
            msObj.Close();
        }
    }
    public class ObjectDeserializer<T>
    {
        public ObjectDeserializer()
        {
        }
        private void SetBasicFileInformation(T Object, BasicFileInformation Data)
        {
            try
            {
                if (Data == null) Data = new BasicFileInformation();
                Type type = typeof(T);
                System.Reflection.PropertyInfo pi = type.GetProperty("BasicData");
                pi.SetValue(Object, Data, null);
            }
            catch { ; }
        }
        private BasicFileInformation GetBasicFileInformation(T Object)
        {
            try
            {
                Type type = typeof(T);
                System.Reflection.PropertyInfo pi = type.GetProperty("BasicData");
                object res = pi.GetValue(Object, null);
                if (res is BasicFileInformation)
                {
                    return (BasicFileInformation)res;
                }
            }
            catch { ; }
            return new BasicFileInformation();
        }
        public T Deserialize(string Json)
        {
            T ret = default(T);
            string toDes = Json;
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(toDes)))
            {
                DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(T));
                T model = (T)deseralizer.ReadObject(ms);// 
                ret = model;
            }
            return ret;
        }
        private BasicFileInformation ReadBasicInformation(Stream target)
        {
            BasicFileInformation ret = new BasicFileInformation();
            byte[] tmp;
            BinaryReader br = new BinaryReader(target);
            tmp = br.ReadBytes(4);
            string ts1 = System.Text.ASCIIEncoding.ASCII.GetString(tmp);
            if (ts1 == "VUPJ")
            {
                int verlen = br.ReadInt32();
                byte[] verarr = br.ReadBytes(verlen);
                string ver = System.Text.UTF8Encoding.UTF8.GetString(verarr);
                int txtlen = br.ReadInt32();
                byte[] txtarr = br.ReadBytes(txtlen);
                string txt = System.Text.UTF8Encoding.UTF8.GetString(txtarr);
                bool havePwd = br.ReadInt32()==1?true:false;
                ret.SavePassword = havePwd?"There is a lovely fox jumped up the pig":"";
                ret.IntroduceText = txt;
                ret.VersionString = ver;
            }
            return ret;
        }
        private int BasicInformationSize(BasicFileInformation FileInfo)
        {
            if (FileInfo == null) return 0;
            byte[] itext = System.Text.UTF8Encoding.UTF8.GetBytes(FileInfo.IntroduceText);
            byte[] iver = System.Text.UTF8Encoding.UTF8.GetBytes(FileInfo.VersionString);
            int ret = 4 + 4 + 4 * 3 + itext.Length + iver.Length;
            return ret;
        }
        public BasicFileInformation ReadBasicInformation(string FileName)
        {
            BasicFileInformation ret = new BasicFileInformation();
            using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                ret=ReadBasicInformation(fs);
            }
            return ret;
        }
        public T DeserializeFromFile(string FileName,BasicFileInformation FileInfo)
        {
            T ret = default(T);
            string toDes = "";
            using(FileStream fs=new FileStream(FileName,FileMode.Open,FileAccess.ReadWrite,FileShare.ReadWrite))
            {
                fs.Seek(BasicInformationSize(FileInfo), SeekOrigin.Begin);
                if (FileInfo.SavePassword.Length > 0)
                {
                    CryptoStream cs = DesCooler.CreateDecryptStream(FileInfo.SavePassword, fs);
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        toDes = sr.ReadToEnd();
                    }
                }
                else
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        toDes = sr.ReadToEnd();
                    }
                }
            }
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(toDes)))
            {
                DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(T));
                T model = (T)deseralizer.ReadObject(ms);// 
                ret = model;
            }
            return ret;
        }
        public T DeserializeFromZipFile(string FileName, BasicFileInformation FileInfo)
        {
            T ret = default(T);
            string toDes = "";
            using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fs.Seek(BasicInformationSize(FileInfo), SeekOrigin.Begin);
                ICSharpCode.SharpZipLib.GZip.GZipInputStream bzi;
                if(FileInfo.SavePassword.Length>0)
                {
                    CryptoStream cs = DesCooler.CreateDecryptStream(FileInfo.SavePassword, fs);
                    bzi = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(cs);
                }else
                {
                    bzi = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(fs);
                }
                using (StreamReader sr = new StreamReader(bzi))
                {
                    try
                    {
                        toDes = sr.ReadToEnd();
                    }
                    catch (ICSharpCode.SharpZipLib.GZip.GZipException e) { try { bzi.Close(); } catch { ;}; throw new Exception("Password Error or File Broken"); return default(T); }
                }
                bzi.Close();
            }
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(toDes)))
            {
                try
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(T));
                    T model = (T)(deseralizer.ReadObject(ms));// 
                    ProjectCooler.FitableProject(model);
                    ret = model;
                }
                catch { ;}
            }
            if (ret != null)
            {
                BasicFileInformation bfi = GetBasicFileInformation(ret);
                if (bfi != null)
                {
                    bfi.VersionString = FileInfo.VersionString;
                    bfi.IntroduceText = FileInfo.VersionString;
                }
            }
            return ret;
        }
    }

    [Serializable]
    [DataContract]
    public class SerializeableObject<T>
    {
        private static ObjectSerializer<T> _Serializer;

        [IgnoreDataMember]
        public static ObjectSerializer<T> Serializer
        {
            get { return _Serializer; }
        }

        private static ObjectDeserializer<T> _Deserializer;

        [IgnoreDataMember]
        public static ObjectDeserializer<T> Deserializer
        {
            get { return SerializeableObject<T>._Deserializer; }
        }

        public SerializeableObject()
        {
            _Serializer = new ObjectSerializer<T>();
            _Deserializer = new ObjectDeserializer<T>();
        }
    }
}
