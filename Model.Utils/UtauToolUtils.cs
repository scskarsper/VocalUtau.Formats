using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.Utils
{
    public class UtauToolUtils
    {
        public static int Resampler_SortNear50(int BeBoundInteger)
        {
            int BaseInteger = BeBoundInteger + 50;
            double RoundInteger = BaseInteger / 50;
            double RoundFloat = BaseInteger % 50;
            int Return = (int)RoundInteger * 50;
            if (RoundFloat >= 25)
            {
                Return += 50;
            }
            return Return;
        }

        public static long Global_GenerateGlobalPlusTimeMs(VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs thisAtom,VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs nextAtom)
        {
            /*
             连续音符A，B,C
             
             WavTool.A.Length= A.Tick@A.Temp - B.PreUtter + B.Overlap
             WavTool.B.Length= B.Tick@B.Temp + B.PreUtter - C.PreUtter + C.Overlap
             WavTool.C.Length= C.Tick@C.Temp + C.PreUTter
             */
            long tpu = 0;
            long npu = 0;
            long nol = 0;
            try
            {
                tpu = thisAtom.PreUtterance;
            }
            catch { ;}
            try
            {
                npu = nextAtom.PreUtterance;
            }
            catch { ;}
            try
            {
                nol = nextAtom.OverlapMs;
            }
            catch { ;}
            return tpu - npu + nol;
        }
    }
}
