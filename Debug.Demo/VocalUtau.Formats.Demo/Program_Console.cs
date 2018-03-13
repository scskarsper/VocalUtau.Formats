using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.Database.VocalDatabase;
using VocalUtau.Formats.Model.USTs.Original;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Demo
{
    static class Program_Console
    {
        static void Main(string[] args)
        {
            SoundAtom sa = new SoundAtom();
            sa.WavFile = @"D:\VocalUtau\VocalUtau.DebugExampleFiles\UTAUKernel\voice\uta\偁.wav";
            sa.PhonemeSymbol = "a";
            sa.SoundStartMs = 6;
            sa.FixedConsonantLengthMs = 52;
            sa.FixedReleasingLengthMs = 69;
            sa.PreUtterance = 0;
            sa.OverlapMs = 0;

            SoundAtom.ResamplerArgs rarg = new SoundAtom.ResamplerArgs();
            rarg.Tempo = 120.00;
            rarg.TickLength = 1920;
            rarg.OutputFile = @"D:\OUT.wav";
            rarg.Note = "G4";
            rarg.Moduration = 0;
            rarg.Intensity = 100;

            string arp=sa.GetResamplerArg(rarg);

        }
    }
}
