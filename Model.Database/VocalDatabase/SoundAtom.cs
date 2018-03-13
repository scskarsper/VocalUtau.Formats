using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.Utils;

namespace VocalUtau.Formats.Model.Database.VocalDatabase
{
    public class SoundAtom
    {
        string _wavFile = "";
        /// <summary>
        /// 文件名
        /// </summary>
        public string WavFile
        {
            get { return _wavFile; }
            set { _wavFile = value; }
        }

        string _phonemeSymbol = "";
        /// <summary>
        /// 辅助记号
        /// </summary>
        public string PhonemeSymbol
        {
            get { return _phonemeSymbol; }
            set { _phonemeSymbol = value; }
        }
        /// <summary>
        /// 主要发音属性：<P0>-（SoundStartMs）-<P1>-(FixedConsonantLengthMs)-<P2>-(延长部)-<P3>-(FixedReleasingLengthMs)-<Pn>
        /// </summary>
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

        #region
        private int _OverlapMs = 0;
        /// <summary>
        /// 重叠，与前一个音符重叠部分的长度，（包络的起点）
        /// </summary>
        public int OverlapMs
        {
            get { return _OverlapMs; }
            set { _OverlapMs = value; }
        }

        private int _PreUtterance = 0;
        /// <summary>
        /// 先行发音，整体音符前移长度。
        /// RealOverlap = p.Oto.msPreUtterance + oa_next.msOverlap - oa_next.msPreUtterance;
        /// </summary>
        public int PreUtterance
        {
            get { return _PreUtterance; }
            set { _PreUtterance = value; }
        }
        #endregion

        public class ResamplerArgs
        {
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

        }
        public string GetResamplerArg(ResamplerArgs Args)
        {
            int PointCount = (int)Math.Ceiling((double)Args.TickLength/5.0);
            List<int> PointVs = new List<int>();
            PointVs.AddRange(Args.PitchValues.ToArray());
            for (int i = PointVs.Count == 0 ? 0 : PointVs.Count - 1; i < PointCount; i++)
            {
                PointVs.Add(0);
            }
            //*1000)

            /*
             1920 --- 0.0255
             1440 --- 0.034
             960 --- 0.05
             480 --- 0.1
             240 --- 0.2
             180 --- 0.338
             120 --- 0.6
             100 --- 0.44
             75  --- 0.44
             70  --- 0.38
             ... --- 0.6
             */

            int millisec = (int)(MidiMathUtils.Tick2Time((long)(Args.TickLength*1.0255),Args.Tempo) * 1000);//60`36
            string[] resampler_arg_suffix = new string[]{
                        "\"" + WavFile+"\"",
                        "\"" + Args.OutputFile+"\"",
                        "\"" + Args.Note + "\"",
                        "100",//<==VEL
                        "\"" + Args.Flags + "\"",
                        (_SoundStartMs).ToString() + "",
                        millisec.ToString() + "",
                        FixedConsonantLengthMs.ToString() + "",
                        _FixedReleasingLengthMs.ToString() + "",
                        Args.Intensity.ToString() + "",
                        Args.Moduration.ToString() + "",
                        "!"+Args.Tempo.ToString()+ "" ,
                        PitchEncoderUtils.Encode(PointVs)};
            return String.Join(" ",resampler_arg_suffix);
        }

    }
}
