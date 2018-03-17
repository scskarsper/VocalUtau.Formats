using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.Database.VocalDatabase;

namespace VocalUtau.Formats.Model.Utils
{
    public class UtauRendCommanderUtils
    {

        public class ResamplerArgs
        {
            public ResamplerArgs(SoundAtom basedAtom,string OutputFilePath, double Tempo, long TickLength, string Note, int Moduration = 0, int Intensity = 100)
            {
                this.Tempo = Tempo;
                this.TickLength = TickLength;
                this.OutputFile = OutputFilePath;
                this.Note = Note;
                this.Moduration = Moduration;
                this.Intensity = Intensity;
                //Based
                this._inputWavfile = basedAtom.WavFile;
                this._FixedConsonantLengthMs = basedAtom.FixedConsonantLengthMs;
                this._FixedReleasingLengthMs = basedAtom.FixedReleasingLengthMs;
                this._SoundStartMs = basedAtom.SoundStartMs;
            }

            string _inputWavfile = "";

            public string InputWavfile
            {
                get { return _inputWavfile; }
                set { _inputWavfile = value; }
            }

            string _outputFile = "";

            public string OutputFile
            {
                get { return _outputFile; }
                set { _outputFile = value; }
            }

            string _note = "";

            public string Note
            {
                get { return _note; }
                set { _note = value; }
            }

            long _tickLength = 0;

            public long TickLength
            {
                get { return _tickLength; }
                set { _tickLength = value; }
            }

            string _Flags = "";

            public string Flags
            {
                get { return _Flags; }
                set { _Flags = value; }
            }

            int _Intensity = 100;

            public int Intensity
            {
                get { return _Intensity; }
                set { _Intensity = value; }
            }

            int _Moduration = 0;

            public int Moduration
            {
                get { return _Moduration; }
                set { _Moduration = value; }
            }

            double _Tempo = 120;

            public double Tempo
            {
                get { return _Tempo; }
                set { _Tempo = value; }
            }

            List<int> _PitchValues = new List<int>();

            public List<int> PitchValues
            {
                get { return _PitchValues; }
                set { _PitchValues = value; }
            }
            
            #region
            int _SoundStartMs = 0;
            /// <summary>
            ///发音开始（偏移,Offset）,距离音频时轴0的毫秒数
            /// </summary>
            public int SoundStartMs
            {
                get { return _SoundStartMs; }
                set { _SoundStartMs = value; }
            }
            int _FixedReleasingLengthMs = 0;
            /// <summary>
            /// 发音结束,距离音频末尾的毫秒数
            /// </summary>
            public int FixedReleasingLengthMs
            {
                get { return _FixedReleasingLengthMs; }
                set { _FixedReleasingLengthMs = value; }
            }

            int _FixedConsonantLengthMs = 0;
            /// <summary>
            /// 辅音（子音部），距离发音开始点时轴(SoundStartMs)的毫秒数
            /// </summary>
            public int FixedConsonantLengthMs
            {
                get { return _FixedConsonantLengthMs; }
                set { _FixedConsonantLengthMs = value; }
            }
            #endregion


            VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs _thisPreutterOverlapsArgs = null;

            public VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs ThisPreutterOverlapsArgs
            {
                get { if (_thisPreutterOverlapsArgs == null)_thisPreutterOverlapsArgs = new SoundAtom.PreUtterOverlapArgs(0, 0); return _thisPreutterOverlapsArgs; }
                set { _thisPreutterOverlapsArgs = value; }
            }

            VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs _nextPreutterOverlapsArgs = null;

            public VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs NextPreutterOverlapsArgs
            {
                get { return _nextPreutterOverlapsArgs; }
                set { _nextPreutterOverlapsArgs = value; }
            }
        }
        public static string GetResamplerArg(ResamplerArgs Args)
        {
            int PointCount = (int)Math.Ceiling((double)Args.TickLength / 5.0);
            List<int> PointVs = new List<int>();
            PointVs.AddRange(Args.PitchValues.ToArray());
            for (int i = PointVs.Count == 0 ? 0 : PointVs.Count - 1; i < PointCount; i++)
            {
                PointVs.Add(0);
            }


            long PreUttrOverlapsMs = UtauToolUtils.Global_GenerateGlobalPlusTimeMs(Args.ThisPreutterOverlapsArgs, Args.NextPreutterOverlapsArgs);
            long FixedMillisecLength = UtauToolUtils.Resampler_SortNear50((int)(MidiMathUtils.Tick2Time((long)(Args.TickLength), Args.Tempo) * 1000 + PreUttrOverlapsMs));
            string[] resampler_arg_suffix = new string[]{
                        "\"" + Args.InputWavfile +"\"",
                        "\"" + Args.OutputFile+"\"",
                        "" + Args.Note + "",
                        "100",//<==VEL
                        "\"" + Args.Flags + "\"",
                        (Args.SoundStartMs).ToString("0.0###") + "",
                        FixedMillisecLength.ToString() + "",
                        Args.FixedConsonantLengthMs.ToString("0.0###") + "",
                        Args.FixedReleasingLengthMs.ToString("0.0###") + "",
                        Args.Intensity.ToString() + "",
                        Args.Moduration.ToString() + "",
                        "!"+Args.Tempo.ToString()+ "" ,
                        PitchEncoderUtils.Encode(PointVs)};
            return String.Join(" ", resampler_arg_suffix);
        }

        public class WavtoolArgs
        {
            string _outputWavfile = "";

            public string OutputWavfile
            {
                get { return _outputWavfile; }
                set { _outputWavfile = value; }
            }

            string _inputWavfile = "";

            public string InputWavfile
            {
                get { return _inputWavfile; }
                set { _inputWavfile = value; }
            }

            long _startPoint = 0;

            public long StartPointMs
            {
                get { return _startPoint; }
                set { _startPoint = value; }
            }

            private double _tickLength;

            public double TickLength
            {
                get { return _tickLength; }
                set { _tickLength = value; }
            }

            double _tempo = 0;

            public double Tempo
            {
                get { return _tempo; }
                set { _tempo = value; }
            }

            long _fadeInLengthMs = 5;

            public long FadeInLengthMs
            {
                get { return _fadeInLengthMs; }
                set { _fadeInLengthMs = value; }
            }

            long _fadeOutLengthMs = 35;

            public long FadeOutLengthMs
            {
                get { return _fadeOutLengthMs; }
                set { _fadeOutLengthMs = value; }
            }

            long _volumePercentInt = 100;

            public long VolumePercentInt
            {
                get { return _volumePercentInt; }
                set { _volumePercentInt = value; }
            }

            SortedDictionary<long, long> _EnvlopePoints = new SortedDictionary<long, long>();

            public SortedDictionary<long, long> EnvlopePoints
            {
                get
                {
                    if (_EnvlopePoints == null) _EnvlopePoints = new SortedDictionary<long, long>();
                    return _EnvlopePoints;
                }
                set { _EnvlopePoints = value; }
            }

            VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs _thisPreutterOverlapsArgs=null;

            public VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs ThisPreutterOverlapsArgs
            {
                get { if (_thisPreutterOverlapsArgs == null)_thisPreutterOverlapsArgs = new SoundAtom.PreUtterOverlapArgs(0,0); return _thisPreutterOverlapsArgs; }
                set { _thisPreutterOverlapsArgs = value; }
            }

            VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs _nextPreutterOverlapsArgs = null;

            public VocalUtau.Formats.Model.Database.VocalDatabase.SoundAtom.PreUtterOverlapArgs NextPreutterOverlapsArgs
            {
                get { return _nextPreutterOverlapsArgs; }
                set { _nextPreutterOverlapsArgs = value; }
            }

        }
        public static string GetWavtoolArgs(WavtoolArgs Args)
        {
            long PreUttrOverlapsMs=UtauToolUtils.Global_GenerateGlobalPlusTimeMs(Args.ThisPreutterOverlapsArgs, Args.NextPreutterOverlapsArgs);
            long TotalLength = (long)(MidiMathUtils.Tick2Time((long)Args.TickLength, Args.Tempo) * 1000 + PreUttrOverlapsMs);
            long EnvStart = Args.FadeInLengthMs;
            long EnvEnd = TotalLength-Args.FadeOutLengthMs;
            SortedDictionary<long,long> TargetEnvlope=new SortedDictionary<long,long>();
            TargetEnvlope.Add(0, 0);
            TargetEnvlope.Add(TotalLength,0);
            long LastVol=100;
            double vpcp = (double)Args.VolumePercentInt / 100.0;
            foreach (KeyValuePair<long, long> sortEnv in Args.EnvlopePoints)
            {
                if (sortEnv.Key == EnvStart)
                {
                    TargetEnvlope.Add(sortEnv.Key, (long)(sortEnv.Value * vpcp));
                }
                else if (sortEnv.Key > EnvStart && sortEnv.Key <= EnvEnd)
                {
                    if (!TargetEnvlope.ContainsKey(EnvStart))
                    {
                        TargetEnvlope.Add(EnvStart, (long)(LastVol * vpcp));
                    }
                    TargetEnvlope.Add(sortEnv.Key, (long)(sortEnv.Value * vpcp));
                }
                else if (sortEnv.Key > EnvEnd)
                {
                    break;
                }
                LastVol = sortEnv.Value;
            }
            if (!TargetEnvlope.ContainsKey(EnvStart))
            {
                if (Args.EnvlopePoints.Count == 0 || TargetEnvlope.Count==0)
                {
                    TargetEnvlope.Add(EnvStart, (long)(LastVol * vpcp));
                }
                else
                {
                    TargetEnvlope.Add(EnvStart, TargetEnvlope[TargetEnvlope.Keys.ToArray()[0]]);
                }
            }
            if (!TargetEnvlope.ContainsKey(EnvEnd))
            {
                TargetEnvlope.Add(EnvEnd, (long)(LastVol * vpcp));
            }
            List<string> wavtool_arg_suffix = new List<string>{
                        "\"" + Args.InputWavfile+"\"",
                        "\"" + Args.OutputWavfile+"\"",
                        "" + Args.StartPointMs.ToString() + "",
                        "" + Args.TickLength.ToString() + "@" +Args.Tempo.ToString()+(PreUttrOverlapsMs>=0?"+":"-")+Math.Abs(PreUttrOverlapsMs).ToString(),
                        //P1,P2,P3
                        "0",Args.FadeInLengthMs.ToString(),Args.FadeOutLengthMs.ToString(),
                        //V1,V2,V3,V4
                        "0",TargetEnvlope[Args.FadeInLengthMs].ToString(),TargetEnvlope[TotalLength - Args.FadeOutLengthMs].ToString(),"0",
                        Args.ThisPreutterOverlapsArgs.OverlapMs.ToString(),
                        //P4
                        "0"
            };
            long lastMs = Args.FadeInLengthMs;
            foreach (KeyValuePair<long, long> sortEnv in TargetEnvlope)
            {
                if (sortEnv.Key >= EnvEnd) break;
                if (sortEnv.Key <= EnvStart)
                {
                    lastMs = sortEnv.Key;
                    continue;
                }
                long dert = sortEnv.Key - lastMs;
                lastMs = sortEnv.Key;
                wavtool_arg_suffix.Add(dert.ToString());
                wavtool_arg_suffix.Add(((long)(sortEnv.Value * vpcp)).ToString());
            }
            return String.Join(" ", wavtool_arg_suffix);
        }
    }
}
