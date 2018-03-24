using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.Database.VocalDatabase;
using VocalUtau.Formats.Model.USTs.Original;
using VocalUtau.Formats.Model.Utils;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Demo
{
    static class Program_Console
    { 
        static void Main1(string[] args)
        {
            long rt = (int)(MidiMathUtils.Tick2Time((long)(480), 180) * 1000);
            long rb = UtauToolUtils.Resampler_SortNear50((int)rt);

            /*
             连续音符A，B,C
             
             WavTool.A.Length= A.Tick@A.Temp - B.PreUtter + B.Overlap
             WavTool.B.Length= B.Tick@B.Temp + B.PreUtter - C.PreUtter + C.Overlap
             WavTool.C.Length= C.Tick@C.Temp + C.PreUTter
             */

            SoundAtom.PreUtterOverlapArgs ou = new SoundAtom.PreUtterOverlapArgs();
            ou.OverlapMs = 50;
            ou.PreUtterance = 100;

            SoundAtom sa = new SoundAtom();
            sa.WavFile = @"D:\VocalUtau\VocalUtau.DebugExampleFiles\UTAUKernel\voice\uta\偁.wav";
            sa.PhonemeSymbol = "a";
            sa.SoundStartMs = 6;
            sa.FixedConsonantLengthMs = 52;
            sa.FixedReleasingLengthMs = 69;
            sa.PreutterOverlapsArgs.PreUtterance = 0;
            sa.PreutterOverlapsArgs.OverlapMs = 0;

            VocalUtau.Formats.Model.Utils.UtauRendCommanderUtils.ResamplerArgs rarg = new VocalUtau.Formats.Model.Utils.UtauRendCommanderUtils.ResamplerArgs(sa,@"D:\OUT.wav", 120, 1920, "D4");
            rarg.ThisPreutterOverlapsArgs = ou;
            rarg.NextPreutterOverlapsArgs = ou.Clone();
            string arp = UtauRendCommanderUtils.GetResamplerArg(rarg);
            VocalUtau.Formats.Model.Utils.UtauRendCommanderUtils.WavtoolArgs rab = new VocalUtau.Formats.Model.Utils.UtauRendCommanderUtils.WavtoolArgs();
            rab.TickLength = 960;
            rab.Tempo = 120;
            rab.StartPointMs = 0;
            rab.InputWavfile = "in.wav";
            rab.OutputWavfile = "out.wav";
            rab.ThisPreutterOverlapsArgs = ou;
            rab.NextPreutterOverlapsArgs = ou.Clone();
            rab.EnvlopePoints.Add(100, 200);
            rab.EnvlopePoints.Add(110, 200);
            rab.EnvlopePoints.Add(190, 200);
            rab.EnvlopePoints.Add(2000, 400);
            string arb = UtauRendCommanderUtils.GetWavtoolArgs(rab);
        }
    }
}
