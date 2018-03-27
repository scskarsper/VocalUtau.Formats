using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.Database.VocalDatabase;
using VocalUtau.Formats.Model.Utils;

namespace VocalUtau.Formats.Model.USTs.Otos
{
    public class CharacterSerializer
    {
        public static CharacterAtom DeSerialize(string FilePath)
        {
            CharacterAtom ret = new CharacterAtom();
            if (System.IO.File.Exists(FilePath))
            {
                Encoding FileEnc = FileEncodingUtils.GetEncoding(FilePath);
                string[] Datas = System.IO.File.ReadAllLines(FilePath, FileEnc);
                for (int i = 0; i < Datas.Length; i++)
                {
                    if (Datas[i].Length > 6 && Datas[i].Substring(0, 6).ToLower() == "image=")
                    {
                        string filename = Datas[i].Substring(6);
                        string Dir = (new System.IO.DirectoryInfo(FilePath)).Parent.FullName;
                        if (System.IO.File.Exists(Dir + "\\" + filename))
                        {
                            ret.Avatar = filename;// Dir + "\\" + filename;
                        }
                    }
                    if (Datas[i].Length > 5 && Datas[i].Substring(0, 5).ToLower() == "name=")
                    {
                        ret.Dbname = Datas[i].Substring(5);
                    }
                    if (Datas[i].Length > 7 && Datas[i].Substring(0, 7).ToLower() == "author=")
                    {
                        ret.Author = Datas[i].Substring(7);
                    }
                }
            }
            return ret;
        }
    }
}
