using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.Database.VocalDatabase;
using VocalUtau.Formats.Model.USTs.Otos;

namespace Debuger
{
    class Program
    {
        static void Main(string[] args)
        {
            SplitDictionary sd=Presamp2DictSerializer.DeSerialize(@"D:\VocalUtau\VocalUtau\bin\Debug\voicedb\YongQi_CVVChinese_Version2\presamp.ini");

            List<VocalUtau.Formats.Model.Database.VocalDatabase.SplitDictionary.SplitAtom> sal=sd.GetCurrentNoteAtom("xxxx", "lao", "{R}");

        }
    }
}
