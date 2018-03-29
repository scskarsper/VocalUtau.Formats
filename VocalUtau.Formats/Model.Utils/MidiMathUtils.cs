using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.Utils
{
    public class MidiMathUtils
    {
        public static double Tick2Time(long Tick, double Tempo)
        {
            double TickPerSecond = 8 * Tempo;
            return Tick / TickPerSecond;
        }
        public static long Time2Tick(double Time, double Tempo)
        {
            double TickPerSecond = 8 * Tempo;
            return (long)Math.Round(Time * TickPerSecond);
        }
    }
}
