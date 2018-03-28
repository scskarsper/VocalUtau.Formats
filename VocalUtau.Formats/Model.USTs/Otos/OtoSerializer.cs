using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocalUtau.Formats.Model.Database.VocalDatabase;
using VocalUtau.Formats.Model.Utils;

namespace VocalUtau.Formats.Model.USTs.Otos
{
    public class OtoSerializer
    {
        //http://tsuro.lofter.com/
        private static List<SoundAtom> getOto(FileInfo fi, System.IO.DirectoryInfo basedir)
        {
            List<SoundAtom> sa = new List<SoundAtom>();
            Encoding FileEnc=FileEncodingUtils.GetEncodingJIS(fi.FullName);
            using(System.IO.FileStream fs=new FileStream(fi.FullName,FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, FileEnc))
                {
                    while (!sr.EndOfStream)
                    {
                        string WavFile = "";

                        string line = sr.ReadLine();
                        string[] AL1=line.Split('=');
                        if (AL1.Length > 1)
                        {
                            WavFile = AL1[0];
                            string[] AL2 = AL1[1].Split(',');
                            if (AL2.Length >= 6)
                            {
                                string NWavFile = PathUtils.AbsolutePath(fi.Directory.FullName,WavFile);
                                if (!System.IO.File.Exists(NWavFile))
                                {
                                    string oldChar = FileEncodingUtils.EncodingToDefault(WavFile, FileEnc);
                                    string N2WavFile = PathUtils.AbsolutePath(fi.Directory.FullName, oldChar);
                                    if (System.IO.File.Exists(N2WavFile))
                                    {
                                        NWavFile = N2WavFile;
                                    }
                                }
                                SoundAtom satom = new SoundAtom();
                                satom.WavFile = PathUtils.RelativePath(basedir.FullName,NWavFile);
                                satom.PhonemeSymbol = AL2[0];
                                satom.SoundStartMs = double.Parse(AL2[1]);
                                satom.FixedConsonantLengthMs = double.Parse(AL2[2]);
                                satom.FixedReleasingLengthMs = double.Parse(AL2[3]);
                                satom.PreutterOverlapsArgs.PreUtterance = double.Parse(AL2[4]);
                                satom.PreutterOverlapsArgs.OverlapMs = double.Parse(AL2[5]);
                                sa.Add(satom);
                            }
                        }
                    }
                }
            }
            return sa;
        }
        private static List<List<SoundAtom>> getFolderOto(System.IO.DirectoryInfo dir, System.IO.DirectoryInfo basedir, List<string> callbackHashTable)
        {
            List<List<SoundAtom>> ret = new List<List<SoundAtom>>();
            FileInfo[] otofile = dir.GetFiles("oto.ini");
            foreach (FileInfo fi in otofile)
            {
                List<SoundAtom> otolist=getOto(fi,basedir);
                callbackHashTable.Add(PathUtils.RelativePath(basedir.FullName,fi.FullName));
                ret.Add(otolist);
            }
            DirectoryInfo[] dis=dir.GetDirectories();
            Object locker=new Object();
            Parallel.For(0,dis.Length,(i)=>{
                DirectoryInfo di=dis[i];
                List<List<SoundAtom>> fs = getFolderOto(di, basedir, callbackHashTable);
                lock(locker)
                {
                    ret.AddRange(fs.ToArray());
                }
            });
            return ret;
        }
        public static List<SoundAtom> DeSerialize(string Folder,List<string> callbackHashTable)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Folder);
            if (!dir.Exists) return new List<SoundAtom>();
            List<List<SoundAtom>> otodatas = getFolderOto(dir, dir, callbackHashTable);
            List<SoundAtom> Otos = new List<SoundAtom>();
            foreach (List<SoundAtom> lst in otodatas)
            {
                foreach (SoundAtom sa in lst)
                {
                    if (!Otos.Contains(sa))
                    {
                        Otos.Add(sa);
                    }
                }
            }
            return Otos;
        }
    }
}
