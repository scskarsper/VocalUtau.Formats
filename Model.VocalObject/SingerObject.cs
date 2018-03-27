using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VocalUtau.Formats.Model.Utils;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class SingerObject:ICloneable
    {
        string _GUID = "";

        [DataMember]
        public string GUID
        {
            get { return _GUID; }
            set { _GUID = value; }
        }
        public string getGuid()
        {
            return GUID;
        }

        public SingerObject()
        {
            _GUID = Guid.NewGuid().ToString();
        }
        public SingerObject(string VocalName)
        {
            _VocalName = VocalName;
            _GUID = Guid.NewGuid().ToString();
        }

        string _VocalName = "";

        [DataMember]
        public string VocalName
        {
            get { return _VocalName; }
            set { _VocalName = value; }
        }
                      
        string _SingerFolder = "";

        [DataMember]
        public string SingerFolder
        {
            get { return _SingerFolder; }
            set { _SingerFolder = value; }
        }

        public string getRealSingerFolder()
        {
            return PathUtils.AbsolutePath(SingerFolder);
        }
        public void setRealSingerFolder(string value)
        {
            SingerFolder = PathUtils.RelativePath(value);
        }

        string _PartResampler = "";

        [DataMember]
        public string PartResampler
        {
            get { return _PartResampler; }
            set { _PartResampler = value; }
        }

        public string getRealResamplerPath()
        {
            return PathUtils.AbsolutePath(PartResampler);
        }
        public void setRealResamplerPath(string value)
        {
            PartResampler = PathUtils.RelativePath(value);
        }

        string _Flags = "";

        [DataMember]
        public string Flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }

        string _Avatar = "";

        [DataMember]
        public string Avatar
        {
            get
            {
                return _Avatar;
            }
            set { _Avatar = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj is SingerObject)
            {
                bool isSame = false;
                SingerObject sobj=(SingerObject)obj;
                isSame = isSame || (this.GUID == sobj.GUID);
                isSame = isSame || (this._VocalName == sobj._VocalName);
                return isSame;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        bool _isSystemSinger = false;
        [IgnoreDataMember]
        public bool IsSystemSinger
        {
            get { return _isSystemSinger; }
            set { _isSystemSinger = value; }
        }

        public object Clone()
        {
            return Force.DeepCloner.DeepClonerExtensions.DeepClone<SingerObject>(this);
        }
    }
}
