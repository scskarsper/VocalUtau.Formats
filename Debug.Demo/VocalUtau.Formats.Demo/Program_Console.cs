using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.USTs.Original;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Demo
{
    static class Program_Console
    {
        static void Main(string[] args)
        {
            USTOriginalProject USTPO=USTOriginalSerializer.Deserialize(@"D:\VocalUtau\IncludeLib\UTAUMixer\DemoUSTS\Sakurane2.Tracks\Track-4b158252-eb7f-4223-b7b0-d78f32e044ec.ust");
            PartsObject pro = USTOriginalSerializer.UST2Parts(USTPO);

            ProjectObject po = new ProjectObject();
            po.ProjectName = "AAA";
            po.InitEmpty();
            po.BackerList[1].WavPartList.Add(0, new WaveObject("A1"));
            po.TrackerList[1].PartList.Add(0, new PartsObject("Pt1"));
            po.TrackerList[1].PartList[0].NoteList.Add(0, new NoteObject());
            po.TrackerList[1].PartList[0].NoteList[0].PhonemeAtoms[0].Flags = "B0Y0";
            po.TrackerList[1].PartList[0].NoteList[0].Lyric = "你";
            po.TrackerList[1].PartList[0].PitchBends.Add(12, 12);
            string json = ProjectObject.Serializer.Serialize(po);

            ProjectObject ro = ProjectObject.Deserializer.Deserialize(json);
        }
    }
}
