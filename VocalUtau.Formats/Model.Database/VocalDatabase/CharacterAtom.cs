using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.Database.VocalDatabase
{
    [Serializable]
    [DataContract]
    public class CharacterAtom
    {
        public CharacterAtom()
        {
        }

        string _avatar = "";
        string _dbname = "";
        string _author = "";

        [DataMember]
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        [DataMember]
        public string Dbname
        {
            get { return _dbname; }
            set { _dbname = value; }
        }

        [DataMember]
        public string Avatar
        {
            get { return _avatar; }
            set { _avatar = value; }
        }
    }
}
