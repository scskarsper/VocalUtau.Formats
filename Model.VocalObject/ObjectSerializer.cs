using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    public class ObjectSerializer<T>
    {
        public ObjectSerializer()
        {
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
        public void SerializeToFile(T Object, string FileName)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
            FileStream msObj = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            js.WriteObject(msObj, Object);
            msObj.Close();
        }
        public void SerializeToZipFile(T Object, string FileName)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
            FileStream msObj = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            ICSharpCode.SharpZipLib.GZip.GZipOutputStream bzo = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(msObj);
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
        public T DeserializeFromFile(string FileName)
        {
            T ret = default(T);
            string toDes = "";
            using(FileStream fs=new FileStream(FileName,FileMode.Open,FileAccess.ReadWrite,FileShare.ReadWrite))
            {
                using(StreamReader sr=new StreamReader(fs))
                {
                    toDes=sr.ReadToEnd();
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
        public T DeserializeFromZipFile(string FileName)
        {
            T ret = default(T);
            string toDes = "";
            using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (ICSharpCode.SharpZipLib.GZip.GZipInputStream bzi = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(fs))
                {
                    using (StreamReader sr = new StreamReader(bzi))
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
