using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using VocalUtau.Formats.Model.Utils;
using VocalUtau.Formats.Model.VocalObject;
using VocalUtau.Formats.Model.VocalObject.ParamTranslater;

namespace VocalUtau.Formats.Model.USTs.Original
{
    public class USTOriginalSerializer
    {
        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileString(
            string lpAppName, // points to section name
            string lpKeyName, // points to key name
            string lpDefault, // points to default string
            byte[] lpReturnedString, // points to destination buffer
            uint nSize, // size of destination buffer
            string lpFileName  // points to initialization filename
        );
        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileString(
            string lpAppName, // points to section name
            string lpKeyName, // points to key name
            string lpDefault, // points to default string
            StringBuilder lpReturnedString, // points to destination buffer
            uint nSize, // size of destination buffer
            string lpFileName  // points to initialization filename
        );


        /// <summary>
        /// 读取section
        /// </summary>
        /// <param name="Strings"></param>
        /// <returns></returns>

        private static List<string> ReadSections(string iniFilename)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[65536];
            uint len = GetPrivateProfileString(null, null, null, buf, (uint)buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }
        public static string IniReadValue(string Section, string Key,string Default, string iniFilename)
        {
            StringBuilder temp = new StringBuilder(255); 
            uint i = GetPrivateProfileString(Section, Key, Default, temp, (uint)255, iniFilename);
            return temp.ToString();
        }
        public static double IniReadDouble(string Section, string Key, double Default, string iniFilename)
        {
            double t = Default;
            string RValue=IniReadValue(Section, Key, Default.ToString(), iniFilename);
            if (RValue.Trim() == "") 
                return double.NaN;
            double.TryParse(RValue, out t);
            return t;
        }
        /// <summary>
        /// 读取指定区域Keys列表。
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Strings"></param>
        /// <returns></returns>

        public List<string> ReadSingleSection(string Section, string iniFilename)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[65536];
            uint lenf = GetPrivateProfileString(Section, null, null, buf, (uint)buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < lenf; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }
        public static USTOriginalProject Deserialize(string ustfile)
        {
            USTOriginalProject Original = new USTOriginalProject();
            Encoding FileEnc = FileEncodingUtils.GetEncoding(ustfile);
            List<string> sectionList = ReadSections(ustfile);
            for (int i = 0; i < sectionList.Count; i++)
            {
                int Cdx=0;
                string sectionName = sectionList[i];
                if (sectionName.ToUpper() == "#SETTING")
                {
                    Original.Tempo = IniReadDouble(sectionName, "Tempo", 120.0, ustfile);
                    Original.Tracks = (int)IniReadDouble(sectionName, "Tracks", 1, ustfile);
                    Original.ProjectName = FileEncodingUtils.DefaultToEncoding(IniReadValue(sectionName, "ProjectName", "", ustfile), FileEnc);
                    Original.VoiceDir = IniReadValue(sectionName, "VoiceDir", "", ustfile);
                    Original.OutFile = IniReadValue(sectionName, "OutFile", "", ustfile);
                    Original.CacheDir = IniReadValue(sectionName, "CacheDir", "", ustfile);
                    Original.Tool1= IniReadValue(sectionName, "Tool1", "wavtool.exe", ustfile);
                    Original.Tool2 = IniReadValue(sectionName, "Tool2", "resampler.exe", ustfile);
                    Original.Flags = IniReadValue(sectionName, "Flags", "", ustfile);
                    bool m2 = false;
                    bool.TryParse(IniReadValue(sectionName, "Mode2", "false", ustfile), out m2);
                }
                else if (sectionName[0] == '#' && int.TryParse(sectionName.Substring(1), out Cdx))
                {
                    USTOriginalNote UNote = new USTOriginalNote();
                    UNote.Length = (long)IniReadDouble(sectionName, "Length", 0, ustfile);
                    UNote.Lyric = FileEncodingUtils.DefaultToEncoding(IniReadValue(sectionName, "Lyric", "", ustfile),FileEnc);
                    UNote.NoteNum = (int)IniReadDouble(sectionName, "NoteNum", 60, ustfile);
                    UNote.PreUtterance = (int)IniReadDouble(sectionName, "PreUtterance", double.NaN, ustfile);
                    UNote.Overlap = (int)IniReadDouble(sectionName, "VoiceOverlap", double.NaN, ustfile);
                    UNote.Intensity = (int)IniReadDouble(sectionName, "Intensity", double.NaN, ustfile);
                    UNote.Modulation = (int)IniReadDouble(sectionName, "Modulation", double.NaN, ustfile);
                    UNote.Tempo = (long)IniReadDouble(sectionName, "Tempo", double.NaN, ustfile);
                    UNote.StartPoint = (int)IniReadDouble(sectionName, "StartPoint", double.NaN, ustfile);
                    UNote.Velocity = (int)IniReadDouble(sectionName, "Velocity", double.NaN, ustfile);
                    UNote.Flags = IniReadValue(sectionName, "Flags", "", ustfile);
                    UNote.Envelope = IniReadValue(sectionName, "Envelope", "", ustfile);
                    UNote.Apreuttr = (int)IniReadDouble(sectionName, "@preuttr", double.NaN, ustfile);
                    UNote.Aoverlap = (int)IniReadDouble(sectionName, "@overlap", double.NaN, ustfile);
                    UNote.Astpoint = (int)IniReadDouble(sectionName, "@stpoint", double.NaN, ustfile);
                    UNote.PBType = (int)IniReadDouble(sectionName, "PBType", 5, ustfile);
                    UNote.PitchBend = IniReadValue(sectionName, "PitchBend", "", ustfile);
                    UNote.PBStart = (int)IniReadDouble(sectionName, "PBStart", 0, ustfile);
                    UNote.VBR = IniReadValue(sectionName, "VBR", "", ustfile);
                    UNote.PBW = IniReadValue(sectionName, "PBW", "", ustfile);
                    UNote.PBY = IniReadValue(sectionName, "PBY", "", ustfile);
                    UNote.PBS = IniReadValue(sectionName, "PBS", "", ustfile);
                    Original.Notes.Add(Cdx,UNote);
                    Cdx = 0;
                }
            }
            return Original;
        }
        private static double FormatNan(double src)
        {
            if (src <= Int32.MinValue) return double.NaN;
            return src;
        }
        public static PartsObject UST2Parts(USTOriginalProject ust)
        {
            PartsObject po = new PartsObject(ust.ProjectName);
            po.Tempo = ust.Tempo;
            long TotalTick = 0;
//            po.TickLength = 0;
            po.PartResampler = ust.Tool2;
            po.Flags = ust.Flags;
            for (int i = 0; i < ust.Notes.Count; i++)
            {
                long stt = TotalTick;
                long len = ust.Notes[i].Length;
                if (ust.Notes[i].Lyric != "R")
                {
                    NoteObject no = new NoteObject(stt, len, ust.Notes[i].NoteNum);
                    no.Lyric = ust.Notes[i].Lyric;
                    no.PhonemeAtoms[0].Flags = ust.Notes[i].Flags;
                    no.PhonemeAtoms[0].Intensity = FormatNan(ust.Notes[i].Intensity);
                    no.PhonemeAtoms[0].Modulation = FormatNan(ust.Notes[i].Modulation);
                    no.PhonemeAtoms[0].Overlap = FormatNan(ust.Notes[i].Overlap);
                    no.PhonemeAtoms[0].PreUtterance = FormatNan(ust.Notes[i].PreUtterance);
                    no.PhonemeAtoms[0].StartPoint = FormatNan(ust.Notes[i].StartPoint);
                    no.PhonemeAtoms[0].Velocity = FormatNan(ust.Notes[i].Velocity);
                    List<KeyValuePair<double,double>> env=ust.Notes[i].EnvelopAnalyse();
                    try
                    {
                        no.PhonemeAtoms[0].FadeInLengthMs = (long)(env[2].Key);//P2
                        no.PhonemeAtoms[0].FadeOutLengthMs = (long)(env[3].Key);//P3
                        no.PhonemeAtoms[0].VolumePercentInt = (long)((env[2].Value + env[3].Value) / 2);//V2+V3/2
                    }
                    catch { ;}
                    po.NoteList.Add(no);
                    TotalTick += ust.Notes[i].Length;// -ust.Notes[i].Overlap;
                }
                else
                {
                    TotalTick += len;
                }
            }
            //po.TickLength = TotalTick;
            po.PitchCompiler.InitPitchBase();
            return po;
        }
    }
}
