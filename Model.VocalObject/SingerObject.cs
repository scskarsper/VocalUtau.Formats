using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class SingerObject
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

        string _PartResampler = "";

        [DataMember]
        public string PartResampler
        {
            get { return _PartResampler; }
            set { _PartResampler = value; }
        }

        string _Flags = "";

        [DataMember]
        public string Flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }

        VocalUtau.Formats.Model.Database.VocalDatabase.CharacterAtom charatom =null;
        string _Avatar = "";

        [DataMember]
        public string Avatar
        {
          get { 
          if (System.IO.File.Exists(_Avatar)) return _Avatar;
          if (System.IO.File.Exists(_SingerFolder + "\\" + _Avatar)) return (new System.IO.FileInfo(_SingerFolder + "\\" + _Avatar)).FullName;
          if (System.IO.File.Exists(_SingerFolder + "\\character.txt"))
          {
              if (charatom == null) charatom = new Database.VocalDatabase.CharacterAtom();
              if(!charatom.IsLoaded)charatom.ReadAvatarFromText(_SingerFolder + "\\character.txt");
              return charatom.Avatar;
          }
          return "";
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
    }
}
