using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.Database.VocalDatabase
{
    public class CharacterAtom
    {
        public CharacterAtom()
        {
        }
        public CharacterAtom(string FilePath)
        {
            ReadAvatarFromText(FilePath);
        }
        bool _isLoaded = false;

        public bool IsLoaded
        {
            get { return _isLoaded; }
        }

        string _avatar = "";
        string _dbname = "";

        public string Dbname
        {
            get { return _dbname; }
            set { _dbname = value; }
        }

        public string Avatar
        {
            get { return _avatar; }
            set { _avatar = value; }
        }
        public void ReadAvatarFromText(string FilePath)
        {
            if (System.IO.File.Exists(FilePath))
            {
                string[] Datas = System.IO.File.ReadAllLines(FilePath, Encoding.GetEncoding("Shift-JIS"));
                for (int i = 0; i < Datas.Length; i++)
                {
                    if (Datas[i].Length > 6 && Datas[i].Substring(0, 6).ToLower() == "image=")
                    {
                        string filename = Datas[i].Substring(6);
                        string Dir = (new System.IO.DirectoryInfo(FilePath)).Parent.FullName;
                        if (System.IO.File.Exists(Dir + "\\" + filename))
                        {
                            this._avatar = Dir + "\\" + filename;
                        }
                    }
                    if (Datas[i].Length > 5 && Datas[i].Substring(0, 5).ToLower() == "name=")
                    {
                        this._dbname = Datas[i].Substring(5);
                    }
                    if (_avatar != "" && _dbname != "") break;
                }
            }
        }
    }
}
