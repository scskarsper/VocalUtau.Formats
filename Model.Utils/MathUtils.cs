using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.Utils
{
    class MathUtils
    {
        internal static long GetNormalizeTick(long Tick)
        {
            int Step = 5;
            long sTick = ((long)Tick / Step) * Step;
            return sTick;
        }
        internal static double Tick2Time(long Tick, double Tempo)
        {
            //Tempo=Beats Per Minutes
            //     = 480 Per Mintues
            //1Minutes=480*Tempo
            //1s=480/60*Tempo=8*Tempo;
            double TickPerSecond = 8 * Tempo;
            return Tick / TickPerSecond;
        }
        internal static long Time2Tick(double Time, double Tempo)
        {
            double TickPerSecond = 8 * Tempo;
            return (long)Math.Round(Time * TickPerSecond);
        }
    }
}
