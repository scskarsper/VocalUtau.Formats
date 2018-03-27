using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using VocalUtau.Formats.Model.BaseObject;

namespace VocalUtau.Formats.Model.VocalObject
{
    public class ObjectSerializer<T>
    {
        public enum SerializeType
        {
            JSON,
            Binary
        }
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
        public void SerializeToFile(T Object, string FileName, SerializeType Type, bool isZip)
        {
            FileStream msObj = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            BasicFileInformation bfi = GetBasicFileInformation(Object);
            string pwd = bfi.SavePassword;
            WriteBasicInformation(Object, msObj);
            Stream TargetStream;
            if (pwd.Length > 0)
            {
                CryptoStream cs = DesCooler.CreateEncryptStream(pwd, msObj);
                if (isZip)
                {
                    TargetStream = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(cs);
                }
                else
                {
                    TargetStream = cs;
                }
            }
            else
            {
                if (isZip)
                {
                    TargetStream = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(msObj);
                }
                else
                {
                    TargetStream = msObj;
                }
            }
            try
            {
                switch (Type)
                {
                    case SerializeType.JSON: Serialize_JSON(Object, TargetStream); break;
                    case SerializeType.Binary: Serialize_Binary(Object, TargetStream); break;
                }
            }
            catch { ;}
            TargetStream.Flush();
            TargetStream.Close();
            msObj.Close();
        }
        private void Serialize_JSON(T Object, Stream TargetStream)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
            js.WriteObject(TargetStream, Object);
        }
        private void Serialize_Binary(T Object, Stream TargetStream)
        {
            BinaryFormatter Formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
            Formatter.Serialize(TargetStream, Object);
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
        public T DeserializeFromFile(string FileName, BasicFileInformation FileInfo, ObjectSerializer<T>.SerializeType Type, bool isZip)
        {
            T ret = default(T);
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Seek(BasicInformationSize(FileInfo), SeekOrigin.Begin);
            Stream SourceStream;
            if (FileInfo.SavePassword.Length > 0)
            {
                CryptoStream cs = DesCooler.CreateDecryptStream(FileInfo.SavePassword, fs);
                if (isZip)
                {
                    SourceStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(cs);
                }
                else
                {
                    SourceStream = cs;
                }
            }
            else
            {
                if (isZip)
                {
                    SourceStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(fs);
                }
                else
                {
                    SourceStream = fs;
                }
            }
            try
            {
                switch (Type)
                {
                    case ObjectSerializer<T>.SerializeType.JSON: ret = Deserialize_JSON(SourceStream); break;
                    case ObjectSerializer<T>.SerializeType.Binary: ret = Deserialize_Binary(SourceStream); break;
                }
            }
            catch (Exception e) { fs.Close(); throw e; return ret; }
            try
            {
                SourceStream.Close();
            }
            catch { ;}
            fs.Close();
            if (ret != null)
            {
                BasicFileInformation bfi = GetBasicFileInformation(ret);
                if (bfi != null)
                {
                    bfi.VersionString = FileInfo.VersionString;
                    bfi.IntroduceText = FileInfo.IntroduceText;
                    bfi.ProjectFilePath = (new System.IO.FileInfo(FileName)).FullName;
                }
            }
            return ret;
        }

        private T Deserialize_JSON(Stream SourceStream)
        {
            T ret = default(T);
            bool isZip = SourceStream is ICSharpCode.SharpZipLib.BZip2.BZip2InputStream;
            string toDes = "";
            using (StreamReader sr = new StreamReader(SourceStream))
            {
                try
                {
                    toDes = sr.ReadToEnd();
                }
                catch (ICSharpCode.SharpZipLib.GZip.GZipException e) { throw new Exception("Password Error or File Broken"); return default(T); }
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
                catch { if (!isZip) { throw new Exception("Password Error or File Broken"); return default(T); };}
            }
            return ret;
        }
        private T Deserialize_Binary(Stream SourceStream)
        {
            T ret = default(T);
            bool isZip = SourceStream is ICSharpCode.SharpZipLib.BZip2.BZip2InputStream;

            try
            {
                BinaryFormatter Formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
                object ob = Formatter.Deserialize(SourceStream);
                if (ob is T)
                {
                    return (T)ob;
                }
            }
            catch (ICSharpCode.SharpZipLib.BZip2.BZip2Exception e) { throw new Exception("Password Error or File Broken"); return default(T); }

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
