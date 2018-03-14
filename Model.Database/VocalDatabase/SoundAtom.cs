using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.Utils;

namespace VocalUtau.Formats.Model.Database.VocalDatabase
{
    public class SoundAtom
    {
        public SoundAtom()
        {
            _PreutterOverlapsArgs = new PreUtterOverlapArgs();
        }
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
        public class PreUtterOverlapArgs
        {
            public PreUtterOverlapArgs()
            {

            }
            public PreUtterOverlapArgs(int Overlap, int PreUtterance)
            {
                this._OverlapMs = OverlapMs;
                this._PreUtterance = PreUtterance;
            }
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

            public PreUtterOverlapArgs Clone()
            {
                PreUtterOverlapArgs ret = new PreUtterOverlapArgs();
                ret._OverlapMs = _OverlapMs;
                ret._PreUtterance = _PreUtterance;
                return ret;
            }
        }
        PreUtterOverlapArgs _PreutterOverlapsArgs = new PreUtterOverlapArgs();

        public PreUtterOverlapArgs PreutterOverlapsArgs
        {
            get { return _PreutterOverlapsArgs; }
        }
        #endregion

    }
}
